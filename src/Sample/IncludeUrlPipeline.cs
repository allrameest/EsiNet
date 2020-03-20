using System;
using System.Collections.Generic;
using EsiNet.Fragments;
using EsiNet.Fragments.Include;
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
                var uriBuilder = new UriBuilder(includeFragment.Uri) {Query = "a=1"};
                fragment = new EsiIncludeFragment(uriBuilder.Uri);
            }

            return fragment;
        }
    }
}