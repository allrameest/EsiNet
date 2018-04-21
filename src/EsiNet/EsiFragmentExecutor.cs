using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsiNet.Fragments;
using EsiNet.Pipeline;

namespace EsiNet
{
    public class EsiFragmentExecutor
    {
        private readonly ConcurrentDictionary<Type, IReadOnlyCollection<PipelineDelegate>> _pipelineCache =
            new ConcurrentDictionary<Type, IReadOnlyCollection<PipelineDelegate>>();
        private readonly IReadOnlyDictionary<Type, Func<IEsiFragment, Task<string>>> _executors;
        private readonly ServiceFactory _serviceFactory;

        public EsiFragmentExecutor(
            IReadOnlyDictionary<Type, Func<IEsiFragment, Task<string>>> executors,
            ServiceFactory serviceFactory)
        {
            _executors = executors;
            _serviceFactory = serviceFactory;
        }

        public async Task<string> Execute(IEsiFragment fragment)
        {
            var fragmentType = fragment.GetType();
            if (!_executors.TryGetValue(fragmentType, out var executor))
            {
                throw new NotSupportedException($"No executor found for type '{fragmentType}'.");
            }

            var pipelineDelegates = _pipelineCache.GetOrAdd(fragmentType, GetPipelineDelegates);

            Task<string> Exec(IEsiFragment f) => executor(f);

            return await pipelineDelegates
                .Aggregate(
                    (ExecuteDelegate) Exec,
                    (next, pipeline) => async f => await pipeline(f, next))(fragment);
        }

        private IReadOnlyCollection<PipelineDelegate> GetPipelineDelegates(Type fragmentType)
        {
            var pipelineService = PipelineServiceFactory.Create(fragmentType);
            return pipelineService.GetPipelineDelegates(_serviceFactory);
        }
    }
}