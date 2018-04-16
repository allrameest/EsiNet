using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiInclude : IEsiFragment
    {
        private readonly EsiFragmentCache _cache;
        private readonly IHttpLoader _httpLoader;
        private readonly EsiBodyParser _esiBodyParser;
        private readonly string _url;

        public EsiInclude(EsiFragmentCache cache, IHttpLoader httpLoader, EsiBodyParser esiBodyParser, string url)
        {
            _esiBodyParser = esiBodyParser;
            _httpLoader = httpLoader;
            _cache = cache;
            _url = url;
        }

        public async Task<string> Execute()
        {
            var fragment = await _cache.GetOrAddWithHeader(_url, RequestAndParse);
            return await fragment.Execute();
        }

        private async Task<(IEsiFragment, CacheControlHeaderValue)> RequestAndParse()
        {
            var response = await _httpLoader.Get(_url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var fragment = _esiBodyParser.Parse(content);

            return (fragment, response.Headers.CacheControl);
        }
    }
}