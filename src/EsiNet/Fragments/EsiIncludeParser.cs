using System;
using System.Collections.Generic;

namespace EsiNet.Fragments
{
    public class EsiIncludeParser : IEsiFragmentParser
    {
        private readonly IncludeUriParser _uriParser;

        public EsiIncludeParser(IncludeUriParser uriParser)
        {
            _uriParser = uriParser ?? throw new ArgumentNullException(nameof(uriParser));
        }

        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            if (body == null) throw new ArgumentNullException(nameof(body));

            var srcUrl = attributes["src"];

            var srcFragment = new EsiIncludeFragment(_uriParser(srcUrl));

            IEsiFragment includeFragment;
            if (attributes.TryGetValue("alt", out var altUrl))
            {
                var altFragment = new EsiIncludeFragment(_uriParser(altUrl));
                includeFragment = new EsiTryFragment(srcFragment, altFragment);
            }
            else
            {
                includeFragment = srcFragment;
            }

            return ShouldContinueOnError(attributes)
                ? new EsiTryFragment(includeFragment, new EsiIgnoreFragment())
                : includeFragment;
        }

        private static bool ShouldContinueOnError(IReadOnlyDictionary<string, string> attributes)
        {
            return
                attributes.TryGetValue("onerror", out var onerrorValue) &&
                onerrorValue.Equals("continue", StringComparison.OrdinalIgnoreCase);
        }
    }
}