using System.Net.Http.Headers;
using System.Threading.Tasks;
using EsiNet.Caching;
using EsiNet.Http;

namespace EsiNet.Fragments
{
    public class EsiIncludeFragmentExecutor
    {
        private readonly IEsiFragmentCache _cache;
        private readonly IHttpLoader _httpLoader;
        private readonly EsiBodyParser _esiBodyParser;
        private readonly EsiFragmentExecutor _fragmentExecutor;

        public EsiIncludeFragmentExecutor(
            IEsiFragmentCache cache,
            IHttpLoader httpLoader,
            EsiBodyParser esiBodyParser,
            EsiFragmentExecutor fragmentExecutor)
        {
            _cache = cache;
            _httpLoader = httpLoader;
            _esiBodyParser = esiBodyParser;
            _fragmentExecutor = fragmentExecutor;
        }

        public async Task<string> Execute(EsiIncludeFragment fragment)
        {
            var remoteFragment = await _cache.GetOrAddFragment(fragment.Url, () => RequestAndParse(fragment.Url));
            return await _fragmentExecutor.Execute(remoteFragment);
        }

        private async Task<(IEsiFragment, CacheControlHeaderValue)> RequestAndParse(string url)
        {
            var response = await _httpLoader.Get(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var fragment = _esiBodyParser.Parse(content);

            return (fragment, response.Headers.CacheControl);
        }
    }
}