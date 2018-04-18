using System.Collections.Generic;

namespace EsiNet.AspNetCore
{
    public static class EsiParserFactory
    {
        public static EsiBodyParser Create()
        {
            var parsers = new Dictionary<string, IEsiParser>();

            var bodyParser = new EsiBodyParser(parsers);

            parsers["esi:include"] = new EsiIncludeParser();
            parsers["esi:remove"] = new EsiIgnoreParser();
            parsers["esi:comment"] = new EsiIgnoreParser();
            parsers["esi:text"] = new EsiTextParser();
            parsers["esi:try"] = new EsiTryParser(bodyParser);

            return bodyParser;
        }
    }
}