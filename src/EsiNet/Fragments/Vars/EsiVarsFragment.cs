using System;
using EsiNet.Expressions;

namespace EsiNet.Fragments.Vars
{
    [Serializable]
    public class EsiVarsFragment : IEsiFragment
    {
        public EsiVarsFragment(VariableString body)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public VariableString Body { get; }
    }
}