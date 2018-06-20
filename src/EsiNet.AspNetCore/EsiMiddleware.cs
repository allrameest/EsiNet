using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EsiNet.Caching;
using EsiNet.Fragments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using CacheControlHeaderValue = System.Net.Http.Headers.CacheControlHeaderValue;

namespace EsiNet.AspNetCore
{
    public class EsiMiddleware
    {
        private static readonly ISet<string> ValidContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "application/javascript",
            "application/json",
            "application/xhtml+xml",
            "application/xml"
        };

        private readonly RequestDelegate _next;
        private readonly EsiBodyParser _parser;
        private readonly EsiFragmentExecutor _executor;
        private readonly IEsiFragmentCache _cache;

        public EsiMiddleware(
            RequestDelegate next,
            EsiBodyParser parser,
            EsiFragmentExecutor executor,
            IEsiFragmentCache cache)
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

            IEsiFragment fragment;
            var key = context.Request.GetDisplayUrl();
            var (found, cachedResponse) = await _cache.TryGet<FragmentPageResponse>(key);

            if (found)
            {
                foreach (var header in cachedResponse.Headers)
                {
                    if (context.Response.Headers.ContainsKey(header.Key)) continue;
                    context.Response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));
                }

                fragment = cachedResponse.Fragment;
            }
            else
            {
                context.Request.Headers[HeaderNames.AcceptEncoding] = StringValues.Empty;

                var body = await TryInterceptNext(context);
                if (body == null)
                {
                    return;
                }


                fragment = _parser.Parse(body);

                CacheControlHeaderValue.TryParse(
                    context.Response.Headers[HeaderNames.CacheControl], out var cacheControl);

                if (cacheControl?.Public ?? false)
                {
                    context.Response.Headers[HeaderNames.CacheControl] = StringValues.Empty;
                }

                if (ShouldSetCache(context))
                {
                    var headers = GetHeadersToForward(context.Response.Headers);
                    var cacheResponse = new FragmentPageResponse(fragment, headers);
                    await _cache.Set(key, cacheControl, cacheResponse);
                }
            }

            var content = await _executor.Execute(fragment);
            context.Response.ContentLength = null;

            foreach (var part in content)
            {
                await context.Response.WriteAsync(part);
            }
        }

        private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> GetHeadersToForward(
            IHeaderDictionary headers)
        {
            return headers
                .ToDictionary(
                    h => h.Key,
                    h => (IReadOnlyCollection<string>) h.Value.ToArray());
        }

        private async Task<string> TryInterceptNext(HttpContext context)
        {
            var originBody = context.Response.Body;

            try
            {
                using (var newBody = new MemoryStream())
                {
                    context.Response.Body = newBody;

                    await _next(context);

                    newBody.Seek(0, SeekOrigin.Begin);

                    if (!ShouldIntercept(context.Response.ContentType))
                    {
                        await newBody.CopyToAsync(originBody);
                        return null;
                    }

                    using (var streamReader = new StreamReader(newBody))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            finally
            {
                context.Response.Body = originBody;
            }
        }

        private static bool ShouldSetCache(HttpContext context)
        {
            return context.Response.StatusCode == 200;
        }

        private static bool ShouldIntercept(string contentType)
        {
            if (contentType == null)
            {
                return false;
            }

            if (contentType.StartsWith("text/"))
            {
                return true;
            }

            var parts = contentType.Split(';');
            return ValidContentTypes.Contains(parts.First());
        }
    }
}