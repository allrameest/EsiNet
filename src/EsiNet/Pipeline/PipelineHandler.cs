using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public abstract class PipelineHandler
    {
        public abstract Task<string> Handle(
            ServiceFactory serviceFactory,
            IEsiFragment fragment,
            Func<IEsiFragment, Task<string>> executor);
    }

    public class PipelineHandler<T> : PipelineHandler
        where T : IEsiFragment
    {
        private readonly object _syncRoot = new object();
        private IReadOnlyCollection<IFragmentExecutePipeline<T>> _pipelinesCache;

        public override async Task<string> Handle(
            ServiceFactory serviceFactory,
            IEsiFragment fragment,
            Func<IEsiFragment, Task<string>> executor)
        {
            var typedFragment = (T) fragment;

            Task<string> ExecuteHandler(T f) => executor(f);

            var pipelines = GetPipelines(serviceFactory);

            return await pipelines
                .Reverse()
                .Aggregate(
                    (ExecuteDelegate<T>) ExecuteHandler,
                    (next, pipeline) => async f => await pipeline.Handle(f, next))(typedFragment);
        }

        private IEnumerable<IFragmentExecutePipeline<T>> GetPipelines(ServiceFactory serviceFactory)
        {
            var pipelines = _pipelinesCache;
            if (pipelines == null)
            {
                lock (_syncRoot)
                {
                    pipelines = _pipelinesCache;
                    if (pipelines == null)
                    {
                        pipelines = serviceFactory.GetInstances<IFragmentExecutePipeline<T>>().ToArray();
                        _pipelinesCache = pipelines;
                    }
                }
            }

            return pipelines;
        }
    }
}