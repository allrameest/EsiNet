using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Helpers
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<Uri, Func<HttpRequestMessage, HttpResponseMessage>> _urlResponseMap =
            new Dictionary<Uri, Func<HttpRequestMessage, HttpResponseMessage>>();

        public FakeHttpMessageHandler Configure(Uri requestUri, HttpStatusCode statusCode, string content)
        {
            return Configure(requestUri, _ => new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content)
            });
        }

        public FakeHttpMessageHandler Configure(Uri requestUri, Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _urlResponseMap[requestUri] = responseFactory;
            return this;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = _urlResponseMap[request.RequestUri](request);
            return Task.FromResult(response);
        }

        public HttpClient ToClient()
        {
            return new HttpClient(this);
        }
    }
}