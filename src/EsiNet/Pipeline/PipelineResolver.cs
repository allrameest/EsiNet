using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public class PipelineResolver<T> : IPipelineResolver
        where T : IEsiFragment
    {
        public IReadOnlyCollection<ExecutePipelineDelegate> GetExecutePipelineDelegates(
            ServiceFactory serviceFactory)
        {
            if (serviceFactory == null) throw new ArgumentNullException(nameof(serviceFactory));

            var pipelines = serviceFactory.GetInstances<IFragmentExecutePipeline<T>>();

            return pipelines
                .Reverse()
                .Select(pipeline => new ExecutePipelineDelegate((fragment, executionContext, next) =>
                {
                    return pipeline.Handle((T) fragment, executionContext, TypedNext);

                    Task<IEnumerable<string>> TypedNext(T f, EsiExecutionContext ec) => next(f, ec);
                }))
                .ToArray();
        }
    }
}