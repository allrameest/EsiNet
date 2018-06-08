using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EsiNet.Caching
{
    public static class EsiFragmentCacheExtensions
    {
        public static async Task<T> GetOrAdd<T>(
            this IEsiFragmentCache cache,
            Uri uri,
            Func<Task<(T, CacheControlHeaderValue)>> valueFactory)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            var key = uri.ToString();

            var (found, cached) = await cache.TryGet<T>(key);
            if (found)
            {
                return cached;
            }

            var (fragment, cacheControl) = await valueFactory();

            await cache.Set(key, cacheControl, fragment);

            return fragment;
        }

        public static async Task Set<T>(
            this IEsiFragmentCache cache, string key, CacheControlHeaderValue cacheControl, T value)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));

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