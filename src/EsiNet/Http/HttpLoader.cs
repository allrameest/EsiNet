using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EsiNet.Pipeline;

namespace EsiNet.Http
{
    public class HttpLoader : IHttpLoader
    {
        private readonly HttpClient _httpClient;
        private readonly IReadOnlyCollection<IHttpLoaderPipeline> _pipelines;

        public HttpLoader(HttpClient httpClient, IEnumerable<IHttpLoaderPipeline> pipelines)
        {
            _httpClient = httpClient;
            _pipelines = pipelines.Reverse().ToArray();
        }

        public async Task<HttpResponseMessage> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-Esi", "true");

            var response = await ExecuteRequest(request);

            response.EnsureSuccessStatusCode();

            return response;
        }

        private Task<HttpResponseMessage> ExecuteRequest(HttpRequestMessage request)
        {
            Task<HttpResponseMessage> Send(HttpRequestMessage r) => _httpClient.SendAsync(r);

            return _pipelines
                .Aggregate(
                    (HttpLoadDelegate) Send,
                    (next, pipeline) => async r => await pipeline.Handle(r, next))(request);
        }
    }
}