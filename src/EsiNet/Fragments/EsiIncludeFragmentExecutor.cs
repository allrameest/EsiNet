using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EsiNet.Caching;
using EsiNet.Http;

namespace EsiNet.Fragments
{
    public class EsiIncludeFragmentExecutor
    {
        private readonly EsiFragmentCacheFacade _cache;
        private readonly IHttpLoader _httpLoader;
        private readonly EsiBodyParser _esiBodyParser;
        private readonly EsiFragmentExecutor _fragmentExecutor;

        public EsiIncludeFragmentExecutor(
            EsiFragmentCacheFacade cache,
            IHttpLoader httpLoader,
            EsiBodyParser esiBodyParser,
            EsiFragmentExecutor fragmentExecutor)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpLoader = httpLoader ?? throw new ArgumentNullException(nameof(httpLoader));
            _esiBodyParser = esiBodyParser ?? throw new ArgumentNullException(nameof(esiBodyParser));
            _fragmentExecutor = fragmentExecutor ?? throw new ArgumentNullException(nameof(fragmentExecutor));
        }

        public async Task<IEnumerable<string>> Execute(
            EsiIncludeFragment fragment,
            EsiExecutionContext executionContext)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));

            var remoteFragment = await _cache.GetOrAdd(
                fragment.Uri,
                executionContext,
                () => RequestAndParse(fragment.Uri, executionContext));
            return await _fragmentExecutor.Execute(remoteFragment, executionContext);
        }

        private async Task<CacheResponse<IEsiFragment>> RequestAndParse(
            Uri uri, EsiExecutionContext executionContext)
        {
            var response = await _httpLoader.Get(uri, executionContext);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var fragment = _esiBodyParser.Parse(content);

            return CacheResponse.Create(fragment, response.Headers.CacheControl, response.Headers.Vary.ToList());
        }
    }
}