using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiInclude : IEsiFragment
    {
        private readonly EsiBodyParser _esiBodyParser;
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly EsiFragmentCache _cache;

        public EsiInclude(EsiFragmentCache cache, EsiBodyParser esiBodyParser, HttpClient httpClient, string url)
        {
            _esiBodyParser = esiBodyParser;
            _httpClient = httpClient;
            _url = url;
            _cache = cache;
        }

        public async Task<string> Execute()
        {
            var fragment = await _cache.GetOrAddWithHeader(_url, RequestAndParse);
            return await fragment.Execute();
        }

        private async Task<(IEsiFragment, CacheControlHeaderValue)> RequestAndParse()
        {
            var response = await MakeRequest();
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var fragment = _esiBodyParser.Parse(content);
            return (fragment, response.Headers.CacheControl);
        }

        private async Task<HttpResponseMessage> MakeRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _url);
            request.Headers.Add("X-Esi", "true");
            return await _httpClient.SendAsync(request);
        }
    }
}