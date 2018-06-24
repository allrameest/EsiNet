using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EsiNet.Http;
using Microsoft.Net.Http.Headers;

namespace Benchmarks
{
    public class FakeStaticHttpLoader : IHttpLoader
    {
        private readonly Dictionary<string, (string, int?)> _urlContentMap;

        public FakeStaticHttpLoader(Dictionary<string, (string, int?)> urlContentMap)
        {
            _urlContentMap = urlContentMap;
        }

        public Task<HttpResponseMessage> Get(Uri uri)
        {
            var (content, maxAge) = _urlContentMap[uri.ToString()];
            var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(content)};
            var cacheHeader = maxAge.HasValue ? $"public,max-age={maxAge.Value}" : "private";
            response.Headers.Add(HeaderNames.CacheControl, cacheHeader);

            return Task.FromResult(response);
        }
    }
}