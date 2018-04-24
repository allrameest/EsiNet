using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EsiNet.Fragments;
using EsiNet.Pipeline;

namespace EsiNet
{
    public class EsiFragmentParser
    {
        private readonly ConcurrentDictionary<Type, IReadOnlyCollection<ParsePipelineDelegate>> _pipelineCache =
            new ConcurrentDictionary<Type, IReadOnlyCollection<ParsePipelineDelegate>>();
        private readonly IReadOnlyDictionary<string, IEsiFragmentParser> _parsers;
        private readonly ServiceFactory _serviceFactory;

        public EsiFragmentParser(
            IReadOnlyDictionary<string, IEsiFragmentParser> parsers,
            ServiceFactory serviceFactory)
        {
            _parsers = parsers;
            _serviceFactory = serviceFactory;
        }

        public IEsiFragment Parse(
            string tag, IReadOnlyDictionary<string, string> attributes, string tagBody, string outerBody)
        {
            if (!_parsers.TryGetValue(tag, out var parser))
            {
                return new EsiTextFragment(outerBody);
            }

            var pipelineDelegates = _serviceFactory.GetInstances<IFragmentParsePipeline>();

            IEsiFragment Parse(IReadOnlyDictionary<string, string> a, string b) => parser.Parse(a, b);

            return pipelineDelegates
                .Aggregate(
                    (ParseDelegate) Parse,
                    (next, pipeline) => (a, b) => pipeline.Handle(a, b, next))(attributes, tagBody);
        }
    }
}