using System;
using System.Collections.Generic;
using EsiNet.Fragments;
using EsiNet.Fragments.Choose;
using EsiNet.Fragments.Ignore;
using EsiNet.Fragments.Include;
using EsiNet.Fragments.Text;
using EsiNet.Fragments.Try;
using EsiNet.Fragments.Vars;
using EsiNet.Pipeline;

namespace EsiNet.AspNetCore
{
    public static class EsiParserFactory
    {
        public static EsiBodyParser Create(
            IEnumerable<IFragmentParsePipeline> parsePipelines)
        {
            if (parsePipelines == null) throw new ArgumentNullException(nameof(parsePipelines));

            var parsers = new Dictionary<string, IEsiFragmentParser>();

            var fragmentParser = new EsiFragmentParser(parsers, parsePipelines);
            var bodyParser = new EsiBodyParser(fragmentParser);

            parsers["esi:include"] = new EsiIncludeParser();
            parsers["esi:remove"] = new EsiIgnoreParser();
            parsers["esi:comment"] = new EsiIgnoreParser();
            parsers["esi:text"] = new EsiTextParser();
            parsers["esi:try"] = new EsiTryParser(bodyParser);
            parsers["esi:choose"] = new EsiChooseParser(bodyParser);
            parsers["esi:vars"] = new EsiVarsParser();

            return bodyParser;
        }
    }
}