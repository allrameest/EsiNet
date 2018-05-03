using System;
using System.Threading.Tasks;

namespace EsiNet.Caching
{
    public interface IEsiFragmentCache
    {
        Task<(bool, T)> TryGet<T>(string key);
        Task Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);
    }
}