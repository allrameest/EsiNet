using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet
{
    public class EsiFragmentExecutor
    {
        private readonly IReadOnlyDictionary<Type, Func<IEsiFragment, Task<string>>> _executors;

        public EsiFragmentExecutor(IReadOnlyDictionary<Type, Func<IEsiFragment, Task<string>>> executors)
        {
            _executors = executors;
        }

        public Task<string> Execute(IEsiFragment fragment)
        {
            var type = fragment.GetType();
            if (!_executors.TryGetValue(type, out var executor))
            {
                throw new NotSupportedException($"No executor found for type '{type}'.");
            }

            return executor(fragment);
        }
    }
}