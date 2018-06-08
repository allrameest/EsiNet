using System;
using System.Collections.Concurrent;
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
        private readonly Log _log;
        private readonly int _exceptionsAllowedBeforeBreaking;
        private readonly TimeSpan _durationOfBreak;
        private readonly Func<Uri, string> _breakerKeyFactory;

        private readonly ConcurrentDictionary<string, CircuitBreakerPolicy> _breakerPolicies =
            new ConcurrentDictionary<string, CircuitBreakerPolicy>();

        public CircuitBreakerHttpLoaderPipeline(
            Log log,
            int exceptionsAllowedBeforeBreaking,
            TimeSpan durationOfBreak,
            Func<Uri, string> breakerKeyFactory = null)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
            _durationOfBreak = durationOfBreak;
            _breakerKeyFactory = breakerKeyFactory ?? (uri => uri.ToString());
        }

        public Task<HttpResponseMessage> Handle(Uri uri, HttpLoadDelegate next)
        {
            var breakerPolicy = _breakerPolicies.GetOrAdd(_breakerKeyFactory(uri), _ => CreatePolicy());
            return breakerPolicy.ExecuteAsync(() => next(uri));
        }

        private CircuitBreakerPolicy CreatePolicy()
        {
            return Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .CircuitBreakerAsync(
                    _exceptionsAllowedBeforeBreaking,
                    _durationOfBreak,
                    (exception, time) =>
                    {
                        var message =
                            $"Error occured and circuit breaker will block calls for {time.TotalSeconds:0} seconds";
                        _log.Error(() => message, exception);
                    },
                    () => { _log.Information(() => "Circuit breaker restored"); });
        }
    }
}