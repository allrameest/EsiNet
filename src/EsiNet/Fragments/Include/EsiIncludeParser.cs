using System;
using System.Collections.Generic;
using EsiNet.Expressions;
using EsiNet.Fragments.Ignore;
using EsiNet.Fragments.Try;

namespace EsiNet.Fragments.Include
{
    public class EsiIncludeParser : IEsiFragmentParser
    {
        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            if (body == null) throw new ArgumentNullException(nameof(body));

            var srcUrl = VariableStringParser.Parse(attributes["src"]);

            var srcFragment = new EsiIncludeFragment(srcUrl);

            IEsiFragment includeFragment;
            if (attributes.TryGetValue("alt", out var altUrl))
            {
                var altFragment = new EsiIncludeFragment(VariableStringParser.Parse(altUrl));
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