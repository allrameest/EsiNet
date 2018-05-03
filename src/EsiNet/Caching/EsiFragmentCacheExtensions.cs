using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Caching
{
    public static partial class EsiFragmentCacheExtensions
    {
        public static async Task<IEsiFragment> GetOrAddFragment(
            this IEsiFragmentCache cache,
            string url,
            Func<Task<(IEsiFragment, CacheControlHeaderValue)>> valueFactory)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            var (found, cachedFragment) = await cache.TryGet<IEsiFragment>(url);
            if (found)
            {
                return cachedFragment;
            }

            var (fragment, cacheControl) = await valueFactory();

            await cache.Set(url, cacheControl, fragment);

            return fragment;
        }

        public static async Task<FragmentPageResponse> GetOrAddPageResponse(
            this IEsiFragmentCache cache,
            string url,
            Func<Task<(FragmentPageResponse, CacheControlHeaderValue)>> valueFactory)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            var (found, cachedResponse) = await cache.TryGet<FragmentPageResponse>(url);
            if (found)
            {
                return cachedResponse;
            }

            var (fragment, cacheControl) = await valueFactory();

            await cache.Set(url, cacheControl, fragment);

            return fragment;
        }

        public static async Task Set<T>(
            this IEsiFragmentCache cache, string url, CacheControlHeaderValue cacheControl, T value)
        {
            if (cacheControl == null)
            {
                return;
            }

            var maxAge = cacheControl.SharedMaxAge ?? cacheControl.MaxAge;
            if (!cacheControl.Public || cacheControl.NoCache || !maxAge.HasValue)
            {
                return;
            }

            await cache.Set(url, value, maxAge.Value);
        }
    }
}