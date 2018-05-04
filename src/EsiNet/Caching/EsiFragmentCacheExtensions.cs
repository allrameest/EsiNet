using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Caching
{
    public static class EsiFragmentCacheExtensions
    {
        private const string FragmentKeyPrefix = "Fragment_";
        private const string PageResponseKeyPrefix = "PageResponse_";

        public static async Task<IEsiFragment> GetOrAddFragment(
            this IEsiFragmentCache cache,
            Uri uri,
            Func<Task<(IEsiFragment, CacheControlHeaderValue)>> valueFactory)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            var key = FragmentKeyPrefix + uri;

            var (found, cachedFragment) = await cache.TryGet<IEsiFragment>(key);
            if (found)
            {
                return cachedFragment;
            }

            var (fragment, cacheControl) = await valueFactory();

            await cache.Set(key, cacheControl, fragment);

            return fragment;
        }

        public static async Task<FragmentPageResponse> GetOrAddPageResponse(
            this IEsiFragmentCache cache,
            Uri uri,
            Func<Task<(FragmentPageResponse, CacheControlHeaderValue)>> valueFactory)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            var key = PageResponseKeyPrefix + uri;

            var (found, cachedResponse) = await cache.TryGet<FragmentPageResponse>(key);
            if (found)
            {
                return cachedResponse;
            }

            var (fragment, cacheControl) = await valueFactory();

            await cache.Set(key, cacheControl, fragment);

            return fragment;
        }

        public static async Task Set<T>(
            this IEsiFragmentCache cache, string key, CacheControlHeaderValue cacheControl, T value)
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

            await cache.Set(key, value, maxAge.Value);
        }
    }
}