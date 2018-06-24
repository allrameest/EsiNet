using System;
using DeepEqual.Syntax;
using EsiNet.AspNetCore;
using EsiNet.Fragments;
using EsiNet.Pipeline;
using Xunit;

namespace Tests.Complete
{
    public class ParsingTests
    {
        [Fact]
        public void Parse_OnlyIncludeTag_SingleIncludeFragmentReturned()
        {
            var fragment = Parse(@"<esi:include src=""http://host/fragment""/>");

            var expected = new EsiIncludeFragment(new Uri("http://host/fragment"));
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
                new EsiIncludeFragment(new Uri("http://host/fragment")),
                new EsiTextFragment("Post"),
            });
            fragment.ShouldDeepEqual(expected);
        }

        [Fact]
        public void Parse_IncludeTagWithOnErrorContinue_TryFragmentReturned()
        {
            var fragment = Parse(
                @"<esi:include src=""http://host/fragment"" onerror=""continue""/>");

            var expected = new EsiTryFragment(
                new EsiIncludeFragment(new Uri("http://host/fragment")),
                new EsiIgnoreFragment());
            fragment.ShouldDeepEqual(expected);
        }

        [Fact]
        public void Parse_IncludeTagWithAltUrl_TryFragmentReturned()
        {
            var fragment = Parse(
                @"<esi:include src=""http://host/fragment"" alt=""http://alt/fragment""/>");

            var expected = new EsiTryFragment(
                new EsiIncludeFragment(new Uri("http://host/fragment")),
                new EsiIncludeFragment(new Uri("http://alt/fragment")));
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
            var parser = EsiParserFactory.Create(Array.Empty<IFragmentParsePipeline>(), url => new Uri(url));
            return parser.Parse(body);
        }
    }
}