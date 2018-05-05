using System;
using System.Collections.Generic;

namespace EsiNet.Fragments
{
    public class EsiIncludeParser : IEsiFragmentParser
    {
        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            var srcUrl = attributes["src"];

            var srcFragment = new EsiIncludeFragment(new Uri(srcUrl, UriKind.RelativeOrAbsolute));

            IEsiFragment includeFragment;
            if (attributes.TryGetValue("alt", out var altUrl))
            {
                var altFragment = new EsiIncludeFragment(new Uri(altUrl, UriKind.RelativeOrAbsolute));
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