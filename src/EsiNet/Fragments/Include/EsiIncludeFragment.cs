using System;
using EsiNet.Expressions;

namespace EsiNet.Fragments.Include
{
    [Serializable]
    public class EsiIncludeFragment : IEsiFragment
    {
        public EsiIncludeFragment(VariableString url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public VariableString Url { get; }
    }
}