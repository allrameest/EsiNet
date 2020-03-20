using System;
using System.Collections.Generic;

namespace EsiNet.Fragments.Ignore
{
    public class EsiIgnoreParser : IEsiFragmentParser
    {
        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            if (body == null) throw new ArgumentNullException(nameof(body));

            return new EsiIgnoreFragment();
        }
    }
}