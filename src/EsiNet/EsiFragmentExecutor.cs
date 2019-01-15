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
        private readonly ConcurrentDictionary<Type, IReadOnlyCollection<ExecutePipelineDelegate>> _pipelineCache =
            new ConcurrentDictionary<Type, IReadOnlyCollection<ExecutePipelineDelegate>>();
        private readonly IReadOnlyDictionary<Type, Func<IEsiFragment, EsiExecutionContext, Task<IEnumerable<string>>>> _executors;
        private readonly ServiceFactory _serviceFactory;

        public EsiFragmentExecutor(
            IReadOnlyDictionary<Type, Func<IEsiFragment, EsiExecutionContext, Task<IEnumerable<string>>>> executors,
            ServiceFactory serviceFactory)
        {
            _executors = executors ?? throw new ArgumentNullException(nameof(executors));
            _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public async Task<IEnumerable<string>> Execute(IEsiFragment fragment, EsiExecutionContext executionContext)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));

            var fragmentType = fragment.GetType();
            if (!_executors.TryGetValue(fragmentType, out var executor))
            {
                throw new NotSupportedException($"No executor found for type '{fragmentType}'.");
            }

            var pipelineDelegates = _pipelineCache.GetOrAdd(fragmentType, GetPipelineDelegates);

            if (!pipelineDelegates.Any())
            {
                return await executor(fragment, executionContext);
            }

            Task<IEnumerable<string>> Exec(IEsiFragment f, EsiExecutionContext ec) => executor(f, ec);

            return await pipelineDelegates
                .Aggregate(
                    (ExecuteDelegate)Exec,
                    (next, pipeline) => async (f, ec) => await pipeline(f, ec, next))(fragment, executionContext);
        }

        private IReadOnlyCollection<ExecutePipelineDelegate> GetPipelineDelegates(Type fragmentType)
        {
            var pipelineResolver = PipelineResolverFactory.Create(fragmentType);
            return pipelineResolver.GetExecutePipelineDelegates(_serviceFactory);
        }
    }
}