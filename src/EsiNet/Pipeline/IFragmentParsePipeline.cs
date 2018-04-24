using System.Collections.Generic;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public interface IFragmentParsePipeline
    {
        IEsiFragment Handle(IReadOnlyDictionary<string, string> attributes, string body, ParseDelegate next);
    }
}