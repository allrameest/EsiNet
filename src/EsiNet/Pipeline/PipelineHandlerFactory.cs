using System;
using System.Collections.Concurrent;

namespace EsiNet.Pipeline
{
    public static class PipelineHandlerFactory
    {
        private static readonly ConcurrentDictionary<Type, PipelineHandler> PipelineHandlerCache =
            new ConcurrentDictionary<Type, PipelineHandler>();

        public static PipelineHandler Create(Type fragmentType)
        {
            return PipelineHandlerCache.GetOrAdd(fragmentType, CreatePipelineHandler);
        }

        private static PipelineHandler CreatePipelineHandler(Type fragmentType)
        {
            var pipelineHandlerType = typeof(PipelineHandler<>).MakeGenericType(fragmentType);
            return (PipelineHandler)Activator.CreateInstance(pipelineHandlerType);
        }
    }
}