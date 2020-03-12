using System;
using System.Collections.Generic;
using EsiNet.Fragments;
using EsiNet.Pipeline;

namespace EsiNet.AspNetCore
{
    public static class EsiParserFactory
    {
        public static EsiBodyParser Create(
            IEnumerable<IFragmentParsePipeline> parsePipelines,
            IncludeUriParser includeUriParser)
        {
            if (parsePipelines == null) throw new ArgumentNullException(nameof(parsePipelines));
            if (includeUriParser == null) throw new ArgumentNullException(nameof(includeUriParser));

            var parsers = new Dictionary<string, IEsiFragmentParser>();

            var fragmentParser = new EsiFragmentParser(parsers, parsePipelines);
            var bodyParser = new EsiBodyParser(fragmentParser);

            parsers["esi:include"] = new EsiIncludeParser(includeUriParser);
            parsers["esi:remove"] = new EsiIgnoreParser();
            parsers["esi:comment"] = new EsiIgnoreParser();
            parsers["esi:text"] = new EsiTextParser();
            parsers["esi:try"] = new EsiTryParser(bodyParser);
            parsers["esi:choose"] = new EsiChooseParser(bodyParser);

            return bodyParser;
        }
    }
}