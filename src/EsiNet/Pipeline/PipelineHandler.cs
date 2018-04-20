using System;
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
        public override async Task<string> Handle(
            ServiceFactory serviceFactory,
            IEsiFragment fragment,
            Func<IEsiFragment, Task<string>> executor)
        {
            var typedFragment = (T) fragment;

            Task<string> ExecuteHandler(T f) => executor(f);

            var pipelines = serviceFactory.GetInstances<IFragmentExecutePipeline<T>>();

            return await pipelines
                .Reverse()
                .Aggregate(
                    (ExecuteDelegate<T>) ExecuteHandler,
                    (next, pipeline) => async f => await pipeline.Handle(f, next))(typedFragment);
        }
    }
}