using System.Collections.Generic;

namespace EsiNet.Pipeline
{
    public interface IPipelineResolver
    {
        IReadOnlyCollection<PipelineDelegate> GetPipelineDelegates(ServiceFactory serviceFactory);
    }
}