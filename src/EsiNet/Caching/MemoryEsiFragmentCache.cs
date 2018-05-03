using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace EsiNet.Caching
{
    public class MemoryEsiFragmentCache : IEsiFragmentCache
    {
        private const string Prefix = "Esi_";
        private readonly IMemoryCache _cache;

        public MemoryEsiFragmentCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<(bool, T)> TryGet<T>(string key)
        {
            var result = _cache.TryGetValue<T>(Prefix + key, out var cachedValue)
                ? (true, cachedValue)
                : (false, default(T));
            return Task.FromResult(result);
        }

        public Task Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            _cache.Set(Prefix + key, value, absoluteExpirationRelativeToNow);
            return Task.CompletedTask;
        }
    }
}