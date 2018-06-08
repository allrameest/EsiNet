using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EsiNet.Pipeline;

namespace EsiNet.Http
{
    public class HttpLoader : IHttpLoader
    {
        private readonly HttpClientFactory _httpClientFactory;
        private readonly IReadOnlyCollection<IHttpLoaderPipeline> _pipelines;

        public HttpLoader(HttpClientFactory httpClientFactory, IEnumerable<IHttpLoaderPipeline> pipelines)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _pipelines = pipelines?.Reverse().ToArray() ?? throw new ArgumentNullException(nameof(pipelines));
        }

        public async Task<HttpResponseMessage> Get(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            var response = await Execute(uri);

            response.EnsureSuccessStatusCode();

            return response;
        }

        private Task<HttpResponseMessage> Execute(Uri uri)
        {
            Task<HttpResponseMessage> Send(Uri u) => ExecuteRequest(uri);

            return _pipelines
                .Aggregate(
                    (HttpLoadDelegate) Send,
                    (next, pipeline) => async u => await pipeline.Handle(u, next))(uri);
        }

        private Task<HttpResponseMessage> ExecuteRequest(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("X-Esi", "true");

            var httpClient = _httpClientFactory(uri);
            return httpClient.SendAsync(request);
        }
    }
}