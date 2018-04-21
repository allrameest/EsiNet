using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public class PipelineResolver<T> : IPipelineResolver
        where T : IEsiFragment
    {
        public IReadOnlyCollection<PipelineDelegate> GetPipelineDelegates(
            ServiceFactory serviceFactory)
        {
            var pipelines = serviceFactory.GetInstances<IFragmentExecutePipeline<T>>();

            return pipelines
                .Reverse()
                .Select(pipeline => new PipelineDelegate((fragment, next) =>
                {
                    return pipeline.Handle((T) fragment, TypedNext);

                    Task<string> TypedNext(T f) => next(f);
                }))
                .ToArray();
        }
    }
}