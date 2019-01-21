using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EsiNet.AspNetCore.Internal
{
    public static class RequestDelegateExtensions
    {
        private static readonly ISet<string> TextContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "application/javascript",
            "application/json",
            "application/xhtml+xml",
            "application/xml"
        };

        public static async Task<string> TryIntercept(this RequestDelegate next, HttpContext context)
        {
            var originBody = context.Response.Body;

            try
            {
                using (var newBody = new MemoryStream())
                {
                    context.Response.Body = newBody;

                    await next(context);

                    newBody.Seek(0, SeekOrigin.Begin);

                    if (newBody.Length == 0)
                    {
                        return null;
                    }
                    else if (!ShouldIntercept(context.Response.ContentType))
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
            return TextContentTypes.Contains(parts.First());
        }
    }
}