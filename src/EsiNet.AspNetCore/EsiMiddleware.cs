using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EsiNet.AspNetCore
{
    public class EsiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EsiBodyParser _parser;

        public EsiMiddleware(RequestDelegate next, EsiBodyParser parser)
        {
            _next = next;
            _parser = parser;
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

            var esiFragment = _parser.Parse(body);
            var content = await esiFragment.Execute();
            await context.Response.WriteAsync(content);
        }
    }
}