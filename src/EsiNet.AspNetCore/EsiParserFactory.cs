using System.Collections.Generic;

namespace EsiNet.AspNetCore
{
    public static class EsiParserFactory
    {
        public static EsiBodyParser Create(EsiFragmentCache cache, IHttpLoader httpLoader)
        {
            var parsers = new Dictionary<string, IEsiParser>();

            var bodyParser = new EsiBodyParser(parsers);
            var includeParser = new EsiIncludeParser(cache, bodyParser, httpLoader);
            var ignoreParser = new EsiIgnoreParser();
            var textParser = new EsiTextParser();

            parsers.Add("esi:include", includeParser);
            parsers.Add("esi:remove", ignoreParser);
            parsers.Add("esi:comment", ignoreParser);
            parsers.Add("esi:text", textParser);

            return bodyParser;
        }
    }
}