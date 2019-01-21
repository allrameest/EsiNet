using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace EsiNet.AspNetCore.Internal
{
    public static class HttpResponseExtensions
    {
        public static async Task WriteAllAsync(this HttpResponse response, IEnumerable<string> contentParts)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (contentParts == null) throw new ArgumentNullException(nameof(contentParts));

            response.ContentLength = null;

            foreach (var part in contentParts)
            {
                await response.WriteAsync(part);
            }
        }

        public static void CopyHeaders(
            this HttpResponse response, IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            foreach (var header in headers)
            {
                if (response.Headers.ContainsKey(header.Key)) continue;
                response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));
            }
        }
    }
}