using System.Collections.Generic;

namespace EsiNet
{
    public interface IEsiParser
    {
        IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body);
    }
}