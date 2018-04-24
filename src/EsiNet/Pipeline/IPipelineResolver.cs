using System.Collections.Generic;

namespace EsiNet.Pipeline
{
    public interface IPipelineResolver
    {
        IReadOnlyCollection<ExecutePipelineDelegate> GetExecutePipelineDelegates(ServiceFactory serviceFactory);
    }
}