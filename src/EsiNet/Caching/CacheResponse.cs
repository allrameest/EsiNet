using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace EsiNet.Caching
{
    public class CacheResponse<T>
    {
        public CacheResponse(T value, CacheControlHeaderValue cacheControl, IReadOnlyCollection<string> vary)
        {
            Value = value;
            CacheControl = cacheControl;
            Vary = vary ?? throw new ArgumentNullException(nameof(vary));
        }

        public T Value { get; }
        public CacheControlHeaderValue CacheControl { get; }
        public IReadOnlyCollection<string> Vary { get; }
    }

    public static class CacheResponse
    {
        public static CacheResponse<T> Create<T>(
            T value, CacheControlHeaderValue cacheControl, IReadOnlyCollection<string> vary) =>
            new CacheResponse<T>(value, cacheControl, vary);
    }
}