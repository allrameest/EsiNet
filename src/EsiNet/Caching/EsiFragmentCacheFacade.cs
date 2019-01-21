using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsiNet.Caching
{
    public class EsiFragmentCacheFacade
    {
        private static readonly IReadOnlyCollection<string> DefaultVaryHeaderNames = new[]
        {
            "Host", "X-Forward-For"
        };

        private readonly IEsiFragmentCache _cache;
        private readonly IVaryHeaderStore _varyHeaderStore;

        public EsiFragmentCacheFacade(IEsiFragmentCache cache, IVaryHeaderStore varyHeaderStore)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _varyHeaderStore = varyHeaderStore ?? throw new ArgumentNullException(nameof(varyHeaderStore));
        }

        public async Task<T> GetOrAdd<T>(
            Uri uri,
            EsiExecutionContext executionContext,
            Func<Task<CacheResponse<T>>> valueFactory)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            var (found, cached) = await TryGet<T>(uri, executionContext);
            if (found)
            {
                return cached;
            }

            var response = await valueFactory();

            await Set(uri, executionContext, response);

            return response.Value;
        }

        public Task<(bool, T)> TryGet<T>(Uri uri, EsiExecutionContext executionContext)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            var key = GetVaryStoreCacheKey(uri, executionContext);
            return _cache.TryGet<T>(key);
        }

        public async Task Set<T>(
            Uri uri,
            EsiExecutionContext executionContext,
            CacheResponse<T> response)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));
            if (response == null) throw new ArgumentNullException(nameof(response));

            var cacheControl = response.CacheControl;
            if (cacheControl == null)
            {
                return;
            }

            var maxAge = cacheControl.SharedMaxAge ?? cacheControl.MaxAge;
            if (!cacheControl.Public || cacheControl.NoCache || !maxAge.HasValue)
            {
                return;
            }

            var key = CreateCacheKey(uri, response.Vary, executionContext);
            SaveVary(uri, response.Vary);
            await _cache.Set(key, response.Value, maxAge.Value);
        }

        private CacheKey GetVaryStoreCacheKey(Uri uri, EsiExecutionContext executionContext)
        {
            var varyHeaderNames = _varyHeaderStore.TryGet(uri, out var headerNames)
                ? headerNames
                : Array.Empty<string>();
            return CreateCacheKey(uri, varyHeaderNames, executionContext);
        }

        private static CacheKey CreateCacheKey(
            Uri uri, IEnumerable<string> varyHeaderNames, EsiExecutionContext executionContext)
        {
            var requestHeaders = executionContext.RequestHeaders;
            var varyHeaderValues = varyHeaderNames
                .Union(DefaultVaryHeaderNames, StringComparer.OrdinalIgnoreCase)
                .OrderBy(headerName => headerName)
                .Select(headerName => GetHeaderValueString(requestHeaders, headerName));

            return new CacheKey(uri, varyHeaderValues.ToList());
        }

        private void SaveVary(
            Uri uri, IReadOnlyCollection<string> varyHeaderNames)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (varyHeaderNames == null) throw new ArgumentNullException(nameof(varyHeaderNames));

            _varyHeaderStore.Set(uri, varyHeaderNames);
        }

        private static string GetHeaderValueString(
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers, string headerName) =>
            headers.TryGetValue(headerName, out var values) ? string.Join(",", values) : string.Empty;
    }
}