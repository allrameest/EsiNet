using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace EsiNet.Caching
{
    public class DistributedEsiFragmentCache : IEsiFragmentCache
    {
        private readonly IDistributedCache _cache;
        private readonly ISerializer<IEsiFragment> _serializer;

        public DistributedEsiFragmentCache(IDistributedCache cache, ISerializer<IEsiFragment> serializer)
        {
            _serializer = serializer;
            _cache = cache;
        }

        public async Task<IEsiFragment> GetOrAddWithHeader(string url,
            Func<Task<(IEsiFragment, CacheControlHeaderValue)>> valueFactory)
        {
            var cachedFragment = await _cache.GetAsync(url);
            if (cachedFragment != null)
            {
                return _serializer.DeserializeBytes(cachedFragment);
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
            var cachedFragment = await _cache.GetAsync(url);
            if (cachedFragment != null)
            {
                return _serializer.DeserializeBytes(cachedFragment);
            }

            var (fragment, cacheExpiration) = await valueFactory();

            await SetCache(url, cacheExpiration, fragment);

            return fragment;
        }

        private async Task SetCache(string url, TimeSpan maxAge, IEsiFragment fragment)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(maxAge)
            };
            var bytes = _serializer.SerializeBytes(fragment);
            await _cache.SetAsync(url, bytes, options);
        }
    }
}