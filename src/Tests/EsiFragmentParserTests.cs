using System;
using System.Collections.Generic;
using EsiNet;
using EsiNet.Fragments;
using EsiNet.Fragments.Text;
using EsiNet.Pipeline;
using FakeItEasy;
using SharpTestsEx;
using Xunit;

namespace Tests
{
    public class EsiFragmentParserTests
    {
        [Fact]
        public void Should_return_outer_body_when_no_matching_tag()
        {
            var fragmentParser = new EsiFragmentParser(
                new Dictionary<string, IEsiFragmentParser>(),
                Array.Empty<IFragmentParsePipeline>());
            const string tag = "esi:unknown";
            const string body = "body";
            const string outerBody = "<esi:unknown>body</unknown>";
            var attributes = new Dictionary<string, string>();

            var fragment = fragmentParser.Parse(tag, attributes, body, outerBody);

            fragment.Should().Be.InstanceOf<EsiTextFragment>();
            ((EsiTextFragment) fragment).Body.Should().Be.EqualTo(outerBody);
        }

        [Fact]
        public void Should_return_fragment_for_matching_tag()
        {
            var attributes = new Dictionary<string, string>();

            var fooParser = A.Fake<IEsiFragmentParser>();
            var fooFragment = A.Fake<IEsiFragment>();
            A.CallTo(() => fooParser.Parse(attributes, "")).Returns(fooFragment);

            var fragmentParser = new EsiFragmentParser(
                new Dictionary<string, IEsiFragmentParser>
                {
                    ["esi:foo"] = fooParser
                },
                Array.Empty<IFragmentParsePipeline>());

            var fragment = fragmentParser.Parse("esi:foo", attributes, "", "");

            fragment.Should().Be.EqualTo(fooFragment);
        }

        [Fact]
        public void Should_execute_pipeline_when_parsing()
        {
            var attributes = new Dictionary<string, string>();

            var fragmentParser = new EsiFragmentParser(
                new Dictionary<string, IEsiFragmentParser>
                {
                    ["esi:text"] = new EsiTextParser()
                },
                new IFragmentParsePipeline[]
                {
                    new RoundPipeline(),
                    new SquarePipeline(),
                    new CurlyPipeline()
                });

            var fragment = fragmentParser.Parse("esi:text", attributes, "body", "outer");

            ((EsiTextFragment) fragment).Body.Should().Be.EqualTo("{[(body)]}");
        }

        private class RoundPipeline : IFragmentParsePipeline
        {
            public IEsiFragment Handle(
                IReadOnlyDictionary<string, string> attributes, string body, ParseDelegate next)
            {
                return next(attributes, $"({body})");
            }
        }

        private class SquarePipeline : IFragmentParsePipeline
        {
            public IEsiFragment Handle(
                IReadOnlyDictionary<string, string> attributes, string body, ParseDelegate next)
            {
                return next(attributes, $"[{body}]");
            }
        }

        private class CurlyPipeline : IFragmentParsePipeline
        {
            public IEsiFragment Handle(
                IReadOnlyDictionary<string, string> attributes, string body, ParseDelegate next)
            {
                return next(attributes, $"{{{body}}}");
            }
        }
    }
}