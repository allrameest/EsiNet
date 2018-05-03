using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace EsiNet.Caching
{
    public class DistributedEsiFragmentCache : IEsiFragmentCache
    {
        private const string Prefix = "Esi_";
        private readonly IDistributedCache _cache;
        private readonly ISerializer _serializer;

        public DistributedEsiFragmentCache(IDistributedCache cache, ISerializer serializer)
        {
            _serializer = serializer;
            _cache = cache;
        }

        public async Task<(bool, T)> TryGet<T>(string key)
        {
            var cachedBytes = await _cache.GetAsync(Prefix + key);
            if (cachedBytes != null)
            {
                return (true, _serializer.DeserializeBytes<T>(cachedBytes));
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
            await _cache.SetAsync(Prefix + key, bytes, options);
        }
    }
}