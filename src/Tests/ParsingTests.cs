using System;
using DeepEqual.Syntax;
using EsiNet.AspNetCore;
using EsiNet.Fragments;
using EsiNet.Pipeline;
using Xunit;

namespace Tests
{
    public class ParsingTests
    {
        [Fact]
        public void Parse_OnlyIncludeTag_SingleIncludeFragmentReturned()
        {
            var fragment = Parse(@"<esi:include src=""http://host/fragment""/>");

            var expected = new EsiIncludeFragment("http://host/fragment");
            fragment.ShouldDeepEqual(expected);
        }

        [Fact]
        public void Parse_OnlyText_SingleTextFragmentReturned()
        {
            var fragment = Parse(@"txt");

            var expected = new EsiTextFragment("txt");
            fragment.ShouldDeepEqual(expected);
        }

        [Fact]
        public void Parse_EmptyBody_IgnoreFragmentReturned()
        {
            var fragment = Parse(@"");

            var expected = new EsiIgnoreFragment();
            fragment.ShouldDeepEqual(expected);
        }

        [Fact]
        public void Parse_IncludeTagWithSurroundingContent_CompositeFragmentReturned()
        {
            var fragment = Parse(@"Pre<esi:include src=""http://host/fragment""/>Post");

            var expected = new EsiCompositeFragment(new IEsiFragment[]
            {
                new EsiTextFragment("Pre"), 
                new EsiIncludeFragment("http://host/fragment"), 
                new EsiTextFragment("Post"), 
            });
            fragment.ShouldDeepEqual(expected);
        }

        [Fact]
        public void Parse_TryTagWithAttemptExcept_TryFragmentReturned()
        {
            var fragment = Parse(
                @"<esi:try><esi:attempt>Attempt</esi:attempt><esi:except>Except</esi:except></esi:try>");

            var expected = new EsiTryFragment(
                new EsiTextFragment("Attempt"),
                new EsiTextFragment("Except"));
            fragment.ShouldDeepEqual(expected);
        }

        [Fact]
        public void Parse_TextTagWithIncludeInside_TextFragmentWithIncludeAsTextReturned()
        {
            var fragment = Parse(@"<esi:text><esi:include src=""http://host/fragment""/></esi:text>");

            var expected = new EsiTextFragment(@"<esi:include src=""http://host/fragment""/>");
            fragment.ShouldDeepEqual(expected);
        }

        [Fact]
        public void Parse_CommentTag_IgnoreFragmentReturned()
        {
            var fragment = Parse(@"<esi:comment text=""Comment""/>");

            var expected = new EsiIgnoreFragment();
            fragment.ShouldDeepEqual(expected);
        }

        [Fact]
        public void Parse_RemoveTag_IgnoreFragmentReturned()
        {
            var fragment = Parse(@"<esi:remove>Remove</esi:remove>");

            var expected = new EsiIgnoreFragment();
            fragment.ShouldDeepEqual(expected);
        }

        private static IEsiFragment Parse(string body)
        {
            var parser = EsiParserFactory.Create(Array.Empty<IFragmentParsePipeline>());
            return parser.Parse(body);
        }
    }
}