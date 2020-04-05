using System;
using System.Collections.Generic;
using System.Text;
using EsiNet.Expressions;

namespace EsiNet.Fragments.Vars
{
    public class EsiVarsParser : IEsiFragmentParser
    {
        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            if (body == null) throw new ArgumentNullException(nameof(body));

            var variableString = VariableStringParser.Parse(body);

            return new EsiVarsFragment(variableString);
        }
    }
}