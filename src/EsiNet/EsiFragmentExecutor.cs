using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet
{
    public class EsiFragmentExecutor
    {
        private readonly IReadOnlyDictionary<Type, Func<IEsiFragment, Task<Func<Stream, Task>>>> _executors;

        public EsiFragmentExecutor(
            IReadOnlyDictionary<Type, Func<IEsiFragment, Task<Func<Stream, Task>>>> executors)
        {
            _executors = executors;
        }

        public Task<Func<Stream, Task>> Execute(IEsiFragment fragment)
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