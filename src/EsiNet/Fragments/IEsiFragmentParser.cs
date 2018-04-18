using System.Collections.Generic;

namespace EsiNet.Fragments
{
    public interface IEsiFragmentParser
    {
        IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body);
    }
}