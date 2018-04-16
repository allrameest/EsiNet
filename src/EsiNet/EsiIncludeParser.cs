using System.Collections.Generic;
using System.Net.Http;

namespace EsiNet
{
    public class EsiIncludeParser : IEsiParser
    {
        private readonly EsiFragmentCache _cache;
        private readonly EsiBodyParser _esiBodyParser;
        private readonly HttpClient _httpClient;

        public EsiIncludeParser(EsiFragmentCache cache, EsiBodyParser esiBodyParser, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _cache = cache;
            _esiBodyParser = esiBodyParser;
        }

        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            var src = attributes["src"];

            return new EsiInclude(_cache, _esiBodyParser, _httpClient, src);
        }
    }
}