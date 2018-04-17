using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EsiNet
{
    public interface IEsiFragmentCache
    {
        Task<IEsiFragment> GetOrAddWithHeader(string url,
            Func<Task<(IEsiFragment, CacheControlHeaderValue)>> valueFactory);

        Task<IEsiFragment> GetOrAdd(string url,
            Func<Task<(IEsiFragment, TimeSpan)>> valueFactory);
    }
}