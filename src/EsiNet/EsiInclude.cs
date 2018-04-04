using System.Net.Http;
using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiInclude : IEsiFragment
    {
        private readonly EsiBodyParser _esiBodyParser;
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public EsiInclude(EsiBodyParser esiBodyParser, HttpClient httpClient, string url)
        {
            _esiBodyParser = esiBodyParser;
            _httpClient = httpClient;
            _url = url;
        }

        public async Task<string> Execute()
        {
            var content = await MakeRequest();
            return await _esiBodyParser.Parse(content).Execute();
        }

        private async Task<string> MakeRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _url);
            request.Headers.Add("X-Esi", "true");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}