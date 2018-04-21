using System;

namespace EsiNet.Pipeline
{
    public static class PipelineServiceFactory
    {
        public static IPipelineResolver Create(Type fragmentType)
        {
            var pipelineServiceType = typeof(PipelineResolver<>).MakeGenericType(fragmentType);
            return (IPipelineResolver) Activator.CreateInstance(pipelineServiceType);
        }
    }
}