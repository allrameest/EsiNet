using System;
using System.Collections.Generic;

namespace EsiNet
{
    [Serializable]
    public class EsiCompositeFragment : IEsiFragment
    {
        public EsiCompositeFragment(IReadOnlyCollection<IEsiFragment> fragments)
        {
            Fragments = fragments;
        }

        public IReadOnlyCollection<IEsiFragment> Fragments { get; }
    }
}