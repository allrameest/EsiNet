using System;
using System.Collections.Generic;
using EsiNet.Expressions;

namespace EsiNet.Fragments.Choose
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
        public EsiWhenFragment(IBooleanExpression expression, IEsiFragment innerFragment)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            InnerFragment = innerFragment ?? throw new ArgumentNullException(nameof(innerFragment));
        }

        public IBooleanExpression Expression { get; }
        public IEsiFragment InnerFragment { get; }
    }
}