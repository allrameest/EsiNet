using System.Collections.Generic;
using System.Net.Http;

namespace EsiNet
{
    public class EsiIncludeParser : IEsiParser
    {
        private readonly EsiFragmentCache _cache;
        private readonly EsiBodyParser _esiBodyParser;
        private readonly IHttpLoader _httpLoader;

        public EsiIncludeParser(EsiFragmentCache cache, EsiBodyParser esiBodyParser, IHttpLoader httpLoader)
        {
            _httpLoader = httpLoader;
            _cache = cache;
            _esiBodyParser = esiBodyParser;
        }

        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            var src = attributes["src"];

            return new EsiInclude(_cache, _httpLoader, _esiBodyParser, src);
        }
    }
}