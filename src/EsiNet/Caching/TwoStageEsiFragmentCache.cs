using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace EsiNet.Caching
{
    public class TwoStageEsiFragmentCache : IEsiFragmentCache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ISerializer _serializer;
        private readonly int _maxMemoryCacheTimeInMinutes;

        public TwoStageEsiFragmentCache(
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ISerializer serializer,
            int maxMemoryCacheTimeInMinutes = 5)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _maxMemoryCacheTimeInMinutes = maxMemoryCacheTimeInMinutes;
        }

        public async Task<(bool, T)> TryGet<T>(string key)
        {
            var fullKey = CreateFullKey<T>(key);
            if (_memoryCache.TryGetValue<T>(fullKey, out var value))
            {
                return (true, value);
            }

            var bytes = await _distributedCache.GetAsync(fullKey);
            if (bytes != null)
            {
                value = _serializer.DeserializeBytes<T>(bytes);
                _memoryCache.Set(fullKey, value);
                return (true, value);
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
            await _distributedCache.SetAsync(CreateFullKey<T>(key), bytes, options);

            var memoryMaxAge = GetMemoryCacheMaxAge(absoluteExpirationRelativeToNow);
            if (memoryMaxAge.HasValue)
            {
                _memoryCache.Set(CreateFullKey<T>(key), value, memoryMaxAge.Value);
            }
        }

        private TimeSpan? GetMemoryCacheMaxAge(TimeSpan maxAge)
        {
            var minutes = (int) maxAge.TotalMinutes;

            if (minutes < 1)
            {
                return null;
            }

            return TimeSpan.FromMinutes(Math.Min(minutes, _maxMemoryCacheTimeInMinutes));
        }

        private static string CreateFullKey<T>(string key)
        {
            return $"Esi_{typeof(T).Name}_{key}";
        }
    }
}