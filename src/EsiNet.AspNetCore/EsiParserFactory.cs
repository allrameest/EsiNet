using System.Collections.Generic;
using EsiNet.Fragments;

namespace EsiNet.AspNetCore
{
    public static class EsiParserFactory
    {
        public static EsiBodyParser Create(ServiceFactory serviceFactory)
        {
            var parsers = new Dictionary<string, IEsiFragmentParser>();

            var fragmentParser = new EsiFragmentParser(parsers, serviceFactory);
            var bodyParser = new EsiBodyParser(fragmentParser);

            parsers["esi:include"] = new EsiIncludeParser();
            parsers["esi:remove"] = new EsiIgnoreParser();
            parsers["esi:comment"] = new EsiIgnoreParser();
            parsers["esi:text"] = new EsiTextParser();
            parsers["esi:try"] = new EsiTryParser(bodyParser);

            return bodyParser;
        }
    }
}