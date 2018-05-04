using System;
using System.Collections.Generic;

namespace EsiNet.Fragments
{
    public class EsiIncludeParser : IEsiFragmentParser
    {
        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            var src = attributes["src"];

            return new EsiIncludeFragment(new Uri(src, UriKind.RelativeOrAbsolute));
        }
    }
}