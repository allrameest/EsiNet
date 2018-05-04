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
            var response = await ExecuteRequest(url);

            response.EnsureSuccessStatusCode();

            return response;
        }

        private Task<HttpResponseMessage> ExecuteRequest(string url)
        {
            Task<HttpResponseMessage> Send(string u) => Exec(url);

            return _pipelines
                .Aggregate(
                    (HttpLoadDelegate) Send,
                    (next, pipeline) => async u => await pipeline.Handle(u, next))(url);
        }

        private Task<HttpResponseMessage> Exec(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-Esi", "true");

            return _httpClient.SendAsync(request);
        }
    }
}