using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EsiNet.Fragments;
using Microsoft.Extensions.Caching.Memory;

namespace EsiNet.Caching
{
    public class MemoryEsiFragmentCache : IEsiFragmentCache
    {
        private readonly IMemoryCache _cache;

        public MemoryEsiFragmentCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<IEsiFragment> GetOrAddWithHeader(string url,
            Func<Task<(IEsiFragment, CacheControlHeaderValue)>> valueFactory)
        {
            if (_cache.TryGetValue<IEsiFragment>(url, out var cachedFragment))
            {
                return cachedFragment;
            }

            var (fragment, cacheControl) = await valueFactory();
            var maxAge = cacheControl.SharedMaxAge ?? cacheControl.MaxAge;
            if (!cacheControl.Public || cacheControl.NoCache || !maxAge.HasValue)
            {
                return fragment;
            }

            var absoluteExpiration = DateTimeOffset.UtcNow.Add(maxAge.Value);
            _cache.Set(url, fragment, absoluteExpiration);

            return fragment;
        }

        public async Task<IEsiFragment> GetOrAdd(string url,
            Func<Task<(IEsiFragment, TimeSpan)>> valueFactory)
        {
            if (_cache.TryGetValue<IEsiFragment>(url, out var cachedFragment))
            {
                return cachedFragment;
            }

            var (fragment, cacheExpiration) = await valueFactory();

            _cache.Set(url, fragment, cacheExpiration);

            return fragment;
        }
    }
}