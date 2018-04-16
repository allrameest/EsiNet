using System.Collections.Generic;
using System.Net.Http;

namespace EsiNet
{
    public class EsiIncludeParser : IEsiParser
    {
        private readonly EsiBodyParser _esiBodyParser;
        private readonly HttpClient _httpClient;

        public EsiIncludeParser(EsiBodyParser esiBodyParser, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _esiBodyParser = esiBodyParser;
        }

        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            var src = attributes["src"];

            return new EsiInclude(_esiBodyParser, _httpClient, src);
        }
    }
}