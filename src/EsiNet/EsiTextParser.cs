using System.Collections.Generic;

namespace EsiNet
{
    public class EsiTextParser : IEsiParser
    {
        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            return new EsiTextFragment(body);
        }
    }
}