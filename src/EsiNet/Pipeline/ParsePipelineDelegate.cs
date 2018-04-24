using System.Collections.Generic;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public delegate IEsiFragment ParsePipelineDelegate(
        IReadOnlyDictionary<string, string> attributes, string body, ParseDelegate next);
}