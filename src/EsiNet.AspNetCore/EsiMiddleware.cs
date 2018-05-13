using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EsiNet.Caching;
using EsiNet.Fragments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace EsiNet.AspNetCore
{
    public class EsiMiddleware
    {
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
            _next = next;
            _parser = parser;
            _executor = executor;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Response.StatusCode == 304 || context.Request.Headers.ContainsKey("X-Esi"))
            {
                await _next(context);
                return;
            }

            var originBody = context.Response.Body;
  

            var response = await _cache.GetOrAddPageResponse(new Uri(context.Request.GetDisplayUrl()), async () =>
            {
                var body = await InvokeNext(context);
                var fragment = _parser.Parse(body);
                var pageResponse = new FragmentPageResponse(fragment, context.Response.ContentType);

                CacheControlHeaderValue.TryParse(
                    context.Response.Headers["Cache-Control"], out var cacheControl);
                return (pageResponse, cacheControl);
            });

            context.Response.Body = originBody;

            var writer = await _executor.Execute(response.Fragment);
            context.Response.ContentType = response.ContentType;
            await writer(context.Response.Body);
        }

        private async Task<string> InvokeNext(HttpContext context)
        {
            string body;
            using (var newBody = new MemoryStream())
            {
                context.Response.Body = newBody;

                await _next(context);

                newBody.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(newBody))
                {
                    body = streamReader.ReadToEnd();
                }
            }

            return body;
        }
    }
}