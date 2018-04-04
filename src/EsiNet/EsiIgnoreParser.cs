using System.Collections.Generic;

namespace EsiNet
{
    public class EsiIgnoreParser : IEsiParser
    {
        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            return new EsiIgnoreFragment();
        }
    }
}