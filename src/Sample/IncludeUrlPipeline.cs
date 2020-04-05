using System;
using System.Collections.Generic;
using System.Linq;
using EsiNet.Expressions;
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
                fragment = new EsiIncludeFragment(
                    new VariableString(
                        includeFragment.Url.Parts.Append("?a=1").ToList()));
            }

            return fragment;
        }
    }
}