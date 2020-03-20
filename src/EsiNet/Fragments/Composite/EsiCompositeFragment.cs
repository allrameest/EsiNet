using System;
using System.Collections.Generic;

namespace EsiNet.Fragments.Composite
{
    [Serializable]
    public class EsiCompositeFragment : IEsiFragment
    {
        public EsiCompositeFragment(IReadOnlyCollection<IEsiFragment> fragments)
        {
            Fragments = fragments ?? throw new ArgumentNullException(nameof(fragments));
        }

        public IReadOnlyCollection<IEsiFragment> Fragments { get; }
    }
}