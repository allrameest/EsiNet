using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EsiNet.Fragments;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace EsiNet.Caching
{
    public class TwoStageEsiFragmentCache : IEsiFragmentCache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ISerializer<IEsiFragment> _serializer;

        public TwoStageEsiFragmentCache(
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ISerializer<IEsiFragment> serializer)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _serializer = serializer;
        }

        public async Task<IEsiFragment> GetOrAddWithHeader(
            string url,
            Func<Task<(IEsiFragment, CacheControlHeaderValue)>> valueFactory)
        {
            var cachedFragment = await GetFromCache(url);
            if (cachedFragment != null)
            {
                return cachedFragment;
            }

            var (fragment, cacheControl) = await valueFactory();

            var maxAge = cacheControl.SharedMaxAge ?? cacheControl.MaxAge;
            if (!cacheControl.Public || cacheControl.NoCache || !maxAge.HasValue)
            {
                return fragment;
            }

            await SetCache(url, maxAge.Value, fragment);

            return fragment;
        }

        public async Task<IEsiFragment> GetOrAdd(string url,
            Func<Task<(IEsiFragment, TimeSpan)>> valueFactory)
        {
            var cachedFragment = await GetFromCache(url);
            if (cachedFragment != null)
            {
                return cachedFragment;
            }

            var (fragment, cacheExpiration) = await valueFactory();

            await SetCache(url, cacheExpiration, fragment);

            return fragment;
        }

        private async Task<IEsiFragment> GetFromCache(string url)
        {
            if (_memoryCache.TryGetValue<IEsiFragment>(url, out var fragment))
            {
                return fragment;
            }

            var bytes = await _distributedCache.GetAsync(url);
            return bytes != null
                ? _serializer.DeserializeBytes(bytes)
                : null;
        }

        private async Task SetCache(string url, TimeSpan maxAge, IEsiFragment fragment)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(maxAge)
            };
            var bytes = _serializer.SerializeBytes(fragment);
            await _distributedCache.SetAsync(url, bytes, options);

            var memoryMaxAge = GetMemoryCacheMaxAge(maxAge);
            if (memoryMaxAge.HasValue)
            {
                _memoryCache.Set(url, fragment, memoryMaxAge.Value);
            }
        }

        private static TimeSpan? GetMemoryCacheMaxAge(TimeSpan maxAge)
        {
            var minutes = (int) maxAge.TotalMinutes;

            if (minutes < 1)
            {
                return null;
            }

            return TimeSpan.FromMinutes(Math.Min(minutes, 5));
        }
    }
}