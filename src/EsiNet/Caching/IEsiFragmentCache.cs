using System;
using System.Threading.Tasks;

namespace EsiNet.Caching
{
    public interface IEsiFragmentCache
    {
        Task<(bool, T)> TryGet<T>(CacheKey key);
        Task Set<T>(CacheKey key, T value, TimeSpan absoluteExpirationRelativeToNow);
    }
}