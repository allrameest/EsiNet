using System;
using System.Net.Http;
using System.Threading.Tasks;
using EsiNet.Logging;
using EsiNet.Pipeline;
using Polly;
using Polly.Retry;

namespace EsiNet.Polly
{
    public class RetryHttpLoaderPipeline : IHttpLoaderPipeline
    {
        private readonly RetryPolicy _retryPolicy;

        public RetryHttpLoaderPipeline(Log log, int retryCount)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Retry(retryCount,
                    (exception, _) => { log.Information(() => "Error making http call. Retrying.", exception); });
        }

        public Task<HttpResponseMessage> Handle(Uri uri, EsiExecutionContext executionContext, HttpLoadDelegate next)
        {
            return _retryPolicy.Execute(() => next(uri, executionContext));
        }
    }
}