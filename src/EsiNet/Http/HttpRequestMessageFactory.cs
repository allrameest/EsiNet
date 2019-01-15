using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace EsiNet.Http
{
    public delegate HttpRequestMessage HttpRequestMessageFactory(Uri uri, EsiExecutionContext executionContext);

    public static class DefaultHttpRequestMessageFactory
    {
        private static readonly HashSet<string> SkipHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "X-Esi",
            "Host",
            "Connection",
            "Accept-Encoding",
            "Cache-Control",
            "Content-Length",
            "Content-Type",
            "Expect",
            "If-Match",
            "If-Modified-Since",
            "If-None-Match",
            "If-Range",
            "If-Unmodified-Since",
            "Range",
            "TE",
            "Upgrade-Insecure-Requests"
        };

        public static HttpRequestMessage Create(Uri uri, EsiExecutionContext executionContext)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("X-Esi", "true");

            foreach (var header in executionContext.RequestHeaders.Where(h => !SkipHeaders.Contains(h.Key)))
            {
                if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return request;
        }
    }
}