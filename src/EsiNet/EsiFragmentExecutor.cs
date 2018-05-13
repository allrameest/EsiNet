using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet
{
    public class EsiFragmentExecutor
    {
        private readonly IReadOnlyDictionary<Type, Func<IEsiFragment, Task<IEnumerable<string>>>> _executors;

        public EsiFragmentExecutor(
            IReadOnlyDictionary<Type, Func<IEsiFragment, Task<IEnumerable<string>>>> executors)
        {
            _executors = executors;
        }

        public Task<IEnumerable<string>> Execute(IEsiFragment fragment)
        {
            var fragmentType = fragment.GetType();
            if (!_executors.TryGetValue(fragmentType, out var executor))
            {
                throw new NotSupportedException($"No executor found for type '{fragmentType}'.");
            }

            return executor(fragment);
        }
    }
}