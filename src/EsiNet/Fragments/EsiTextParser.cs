using System.Collections.Generic;

namespace EsiNet.Fragments
{
    public class EsiTextParser : IEsiFragmentParser
    {
        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            return new EsiTextFragment(body);
        }
    }
}