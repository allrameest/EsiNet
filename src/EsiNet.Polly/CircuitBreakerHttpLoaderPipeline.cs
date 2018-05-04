using System;
using System.Net.Http;
using System.Threading.Tasks;
using EsiNet.Logging;
using EsiNet.Pipeline;
using Polly;
using Polly.CircuitBreaker;

namespace EsiNet.Polly
{
    public class CircuitBreakerHttpLoaderPipeline : IHttpLoaderPipeline
    {
        private readonly CircuitBreakerPolicy _breakerPolicy;

        public CircuitBreakerHttpLoaderPipeline(
            Log log,
            int exceptionsAllowedBeforeBreaking,
            TimeSpan durationOfBreak)
        {
            _breakerPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking,
                    durationOfBreak,
                    (exception, time) =>
                    {
                        var message =
                            $"Error occured and circuit breaker will block calls for {time.TotalSeconds:0} seconds";
                        log.Error(() => message, exception);
                    },
                    () => { log.Information(() => "Circuit breaker restored"); });
        }

        public Task<HttpResponseMessage> Handle(Uri uri, HttpLoadDelegate next)
        {
            return _breakerPolicy.ExecuteAsync(() => next(uri));
        }
    }
}