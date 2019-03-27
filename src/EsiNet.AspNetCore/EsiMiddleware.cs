using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.AspNetCore.Internal;
using EsiNet.Caching;
using EsiNet.Fragments;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using CacheControlHeaderValue = System.Net.Http.Headers.CacheControlHeaderValue;

namespace EsiNet.AspNetCore
{
    public class EsiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EsiBodyParser _parser;
        private readonly EsiFragmentExecutor _executor;
        private readonly EsiFragmentCacheFacade _cache;

        public EsiMiddleware(
            RequestDelegate next,
            EsiBodyParser parser,
            EsiFragmentExecutor executor,
            EsiFragmentCacheFacade cache)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Response.StatusCode == 304 || context.Request.Headers.ContainsKey("X-Esi"))
            {
                await _next(context);
                return;
            }

            var executionContext = new EsiExecutionContext(
                context.Request.Headers.ToDictionary(),
                new Dictionary<string, string>
                {
                    ["HTTP_HOST"] = context.Request.Host.Host,
                    ["HTTP_REFERER"] = context.Request.Headers["Referer"].ToString()
                });
            var pageUri = GetPageUri(context.Request);
            var (found, cachedResponse) = await _cache.TryGet<FragmentPageResponse>(pageUri, executionContext);

            IEsiFragment fragment;
            if (found)
            {
                context.Response.CopyHeaders(cachedResponse.Headers);
                fragment = cachedResponse.Fragment;
            }
            else
            {
                var acceptEncoding = context.Request.Headers[HeaderNames.AcceptEncoding];
                context.Request.Headers[HeaderNames.AcceptEncoding] = StringValues.Empty;
                var body = await _next.TryIntercept(context);
                context.Request.Headers[HeaderNames.AcceptEncoding] = acceptEncoding;

                if (body == null)
                {
                    return;
                }

                fragment = _parser.Parse(body);

                await StoreFragmentInCache(context, pageUri, executionContext, fragment);
            }

            var content = await _executor.Execute(fragment, executionContext);

            await context.Response.WriteAllAsync(content);
        }

        private async Task StoreFragmentInCache(
            HttpContext context, Uri pageUri, EsiExecutionContext executionContext, IEsiFragment fragment)
        {
            CacheControlHeaderValue.TryParse(
                context.Response.Headers[HeaderNames.CacheControl], out var cacheControl);

            if (ShouldSetCache(context))
            {
                var headers = context.Response.Headers.ToDictionary();
                var pageResponse = new FragmentPageResponse(fragment, headers);

                var vary = context.Response.Headers[HeaderNames.Vary];
                var cacheResponse = CacheResponse.Create(pageResponse, cacheControl, vary);

                await _cache.Set(pageUri, executionContext, cacheResponse);
            }
        }

        private static bool ShouldSetCache(HttpContext context)
        {
            return context.Response.StatusCode == 200;
        }

        private static Uri GetPageUri(HttpRequest request)
        {
            var host = request.Host.Value ?? "unknown-host";
            return new Uri(
                request.Scheme + "://" + host + request.PathBase.Value + request.Path.Value + request.QueryString.Value);
        }
    }
}