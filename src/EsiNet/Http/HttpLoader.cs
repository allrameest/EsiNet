using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EsiNet.Logging;
using EsiNet.Pipeline;

namespace EsiNet.Http
{
    public class HttpLoader : IHttpLoader
    {
        private readonly HttpClientFactory _httpClientFactory;
        private readonly Log _log;
        private readonly IReadOnlyCollection<IHttpLoaderPipeline> _pipelines;

        public HttpLoader(
            HttpClientFactory httpClientFactory,
            IEnumerable<IHttpLoaderPipeline> pipelines,
            Log log)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _log = log;
            _pipelines = pipelines?.Reverse().ToArray() ?? throw new ArgumentNullException(nameof(pipelines));
        }

        public async Task<HttpResponseMessage> Get(Uri uri, EsiExecutionContext executionContext)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            try
            {
                var response = await Execute(uri, executionContext);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (Exception ex)
            {
                _log.Error(() => $"Error when loading '{uri}'.", ex);
                throw;
            }
        }

        private Task<HttpResponseMessage> Execute(Uri uri, EsiExecutionContext executionContext)
        {
            Task<HttpResponseMessage> Send(Uri u) => ExecuteRequest(uri, executionContext);

            return _pipelines
                .Aggregate(
                    (HttpLoadDelegate) Send,
                    (next, pipeline) => async u => await pipeline.Handle(u, next))(uri);
        }

        private Task<HttpResponseMessage> ExecuteRequest(Uri uri, EsiExecutionContext executionContext)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("X-Esi", "true");

            var httpClient = _httpClientFactory(uri);
            return httpClient.SendAsync(request);
        }
    }
}