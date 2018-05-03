using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace EsiNet.Caching
{
    public class TwoStageEsiFragmentCache : IEsiFragmentCache
    {
        private const string Prefix = "Esi_";
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ISerializer _serializer;

        public TwoStageEsiFragmentCache(
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ISerializer serializer)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _serializer = serializer;
        }

        public async Task<(bool, T)> TryGet<T>(string key)
        {
            if (_memoryCache.TryGetValue<T>(Prefix + key, out var value))
            {
                return (true, value);
            }

            var bytes = await _distributedCache.GetAsync(Prefix + key);
            if (bytes != null)
            {
                return (true, _serializer.DeserializeBytes<T>(bytes));
            }

            return (false, default(T));
        }

        public async Task Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            };
            var bytes = _serializer.SerializeBytes(value);
            await _distributedCache.SetAsync(Prefix + key, bytes, options);

            var memoryMaxAge = GetMemoryCacheMaxAge(absoluteExpirationRelativeToNow);
            if (memoryMaxAge.HasValue)
            {
                _memoryCache.Set(Prefix + key, value, memoryMaxAge.Value);
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