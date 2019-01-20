using System;
using System.Threading.Tasks;

namespace EsiNet.Caching
{
    public class NullEsiFragmentCache : IEsiFragmentCache
    {
        public Task<(bool, T)> TryGet<T>(CacheKey key)
        {
            return Task.FromResult((false, default(T)));
        }

        public Task Set<T>(CacheKey key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            return Task.CompletedTask;
        }
    }
}