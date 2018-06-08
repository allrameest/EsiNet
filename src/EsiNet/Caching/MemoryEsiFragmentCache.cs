using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace EsiNet.Caching
{
    public class MemoryEsiFragmentCache : IEsiFragmentCache
    {
        private readonly IMemoryCache _cache;

        public MemoryEsiFragmentCache(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public Task<(bool, T)> TryGet<T>(string key)
        {
            var result = _cache.TryGetValue<T>(CreateFullKey<T>(key), out var cachedValue)
                ? (true, cachedValue)
                : (false, default(T));
            return Task.FromResult(result);
        }

        public Task Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            _cache.Set(CreateFullKey<T>(key), value, absoluteExpirationRelativeToNow);
            return Task.CompletedTask;
        }

        private static string CreateFullKey<T>(string key)
        {
            return $"Esi_{typeof(T).Name}_{key}";
        }
    }
}