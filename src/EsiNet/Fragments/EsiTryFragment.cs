using System;

namespace EsiNet.Fragments
{
    [Serializable]
    public class EsiTryFragment : IEsiFragment
    {
        public EsiTryFragment(IEsiFragment attemptFragment, IEsiFragment exceptFragment)
        {
            AttemptFragment = attemptFragment ?? throw new ArgumentNullException(nameof(attemptFragment));
            ExceptFragment = exceptFragment ?? throw new ArgumentNullException(nameof(exceptFragment));
        }

        public IEsiFragment AttemptFragment { get; }
        public IEsiFragment ExceptFragment { get; }
    }
}