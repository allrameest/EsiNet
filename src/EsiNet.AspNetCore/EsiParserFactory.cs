using System.Collections.Generic;

namespace EsiNet.AspNetCore
{
    public static class EsiParserFactory
    {
        public static EsiBodyParser Create()
        {
            var parsers = new Dictionary<string, IEsiParser>
            {
                ["esi:include"] = new EsiIncludeParser(),
                ["esi:remove"] = new EsiIgnoreParser(),
                ["esi:comment"] = new EsiIgnoreParser(),
                ["esi:text"] = new EsiTextParser()
            };

            return new EsiBodyParser(parsers);
        }
    }
}