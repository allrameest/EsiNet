using System;
using System.Collections.Generic;
using EsiNet.Fragments.Choose;

namespace EsiNet.Fragments
{
    [Serializable]
    public class EsiChooseFragment : IEsiFragment
    {
        public EsiChooseFragment(IReadOnlyCollection<EsiWhenFragment> whenFragments, IEsiFragment otherwiseFragment)
        {
            WhenFragments = whenFragments;
            OtherwiseFragment = otherwiseFragment;
        }

        public IReadOnlyCollection<EsiWhenFragment> WhenFragments { get; }
        public IEsiFragment OtherwiseFragment { get; }
    }

    [Serializable]
    public class EsiWhenFragment : IEsiFragment
    {
        public EsiWhenFragment(ComparisonExpression expression, IEsiFragment innerFragment)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            InnerFragment = innerFragment ?? throw new ArgumentNullException(nameof(innerFragment));
        }

        public ComparisonExpression Expression { get; }
        public IEsiFragment InnerFragment { get; }
    }
}