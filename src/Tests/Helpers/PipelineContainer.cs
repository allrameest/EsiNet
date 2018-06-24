using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EsiNet.Fragments;
using EsiNet.Pipeline;

namespace Tests.Helpers
{
    public class PipelineContainer
    {
        private readonly Dictionary<Type, object> _pipelines = new Dictionary<Type, object>();

        public void Add<T>(IFragmentExecutePipeline<T> pipeline)
            where T : IEsiFragment
        {
            List<IFragmentExecutePipeline<T>> list;
            var key = typeof(IEnumerable<IFragmentExecutePipeline<T>>);
            if (_pipelines.TryGetValue(key, out var impl))
            {
                list = (List<IFragmentExecutePipeline<T>>) impl;
            }
            else
            {
                list = new List<IFragmentExecutePipeline<T>>();
                _pipelines.Add(key, list);
            }

            list.Add(pipeline);
        }

        public object GetInstance(Type type)
        {
            if (_pipelines.TryGetValue(type, out var impl))
            {
                return impl;
            }

            return typeof(PipelineContainer)
                .GetMethod(nameof(CreateEmpty), BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(type.GenericTypeArguments.Single())
                .Invoke(null, null);
        }

        private static object CreateEmpty<T>()
        {
            return Array.Empty<T>();
        }
    }
}