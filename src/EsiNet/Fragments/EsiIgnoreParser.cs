using System.Collections.Generic;

namespace EsiNet.Fragments
{
    public class EsiIgnoreParser : IEsiFragmentParser
    {
        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            return new EsiIgnoreFragment();
        }
    }
}