using System.Collections.Generic;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public delegate IEsiFragment ParseDelegate(IReadOnlyDictionary<string, string> attributes, string body);
}