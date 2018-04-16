using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace EsiNet.AspNetCore
{
    public class EsiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EsiBodyParser _parser;
        private readonly EsiFragmentCache _cache;

        public EsiMiddleware(RequestDelegate next, EsiBodyParser parser, EsiFragmentCache cache)
        {
            _next = next;
            _parser = parser;
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

            context.Response.Body = originBody;

            var esiFragment = await _cache.GetOrAdd(context.Request.GetDisplayUrl(), () =>
            {
                var fragment = _parser.Parse(body);
                var result = (fragment, TimeSpan.FromMinutes(5));
                return Task.FromResult(result);
            });

            var content = await esiFragment.Execute();
            await context.Response.WriteAsync(content);
        }
    }
}