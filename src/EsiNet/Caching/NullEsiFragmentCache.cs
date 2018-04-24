using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Caching
{
    public class NullEsiFragmentCache : IEsiFragmentCache
    {
        public async Task<IEsiFragment> GetOrAdd(string url, Func<Task<(IEsiFragment, CacheControlHeaderValue)>> valueFactory)
        {
            var (fragment, _) = await valueFactory();
            return fragment;
        }
    }
}