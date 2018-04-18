﻿using System;

namespace EsiNet
{
    [Serializable]
    public class EsiTryFragment : IEsiFragment
    {
        public EsiTryFragment(IEsiFragment attemptFragment, IEsiFragment exceptFragment)
        {
            AttemptFragment = attemptFragment;
            ExceptFragment = exceptFragment;
        }

        public IEsiFragment AttemptFragment { get; }
        public IEsiFragment ExceptFragment { get; }
    }
}