using System.Collections.Generic;
using EsiNet.Fragments;
using EsiNet.Pipeline;

namespace Sample
{
    public class IncludeUrlPipeline : IFragmentParsePipeline
    {
        public IEsiFragment Handle(IReadOnlyDictionary<string, string> attributes, string body, ParseDelegate next)
        {
            var fragment = next(attributes, body);

            if (fragment is EsiIncludeFragment includeFragment)
            {
                fragment = new EsiIncludeFragment(includeFragment.Url + "?a=1");
            }

            return fragment;
        }
    }
}