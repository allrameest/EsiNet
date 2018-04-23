using System;

namespace EsiNet.Pipeline
{
    public static class PipelineResolverFactory
    {
        public static IPipelineResolver Create(Type fragmentType)
        {
            var pipelineServiceType = typeof(PipelineResolver<>).MakeGenericType(fragmentType);
            return (IPipelineResolver) Activator.CreateInstance(pipelineServiceType);
        }
    }
}