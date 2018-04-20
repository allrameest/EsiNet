using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Fragments;
using EsiNet.Pipeline;

namespace EsiNet
{
    public class EsiFragmentExecutor
    {
        private readonly IReadOnlyDictionary<Type, Func<IEsiFragment, Task<string>>> _executors;
        private readonly ServiceFactory _serviceFactory;

        public EsiFragmentExecutor(
            IReadOnlyDictionary<Type, Func<IEsiFragment, Task<string>>> executors,
            ServiceFactory serviceFactory)
        {
            _executors = executors;
            _serviceFactory = serviceFactory;
        }

        public Task<string> Execute(IEsiFragment fragment)
        {
            var fragmentType = fragment.GetType();
            if (!_executors.TryGetValue(fragmentType, out var executor))
            {
                throw new NotSupportedException($"No executor found for type '{fragmentType}'.");
            }

            var pipelineHandler = PipelineHandlerFactory.Create(fragmentType);
            return pipelineHandler.Handle(_serviceFactory, fragment, executor);
        }
    }
}