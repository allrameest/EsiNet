using System.Collections.Generic;
using EsiNet.Fragments.Choose;
using SharpTestsEx;
using Xunit;

namespace Tests
{
    public class WhenParserEvaluatorTests
    {
        [Theory]
        [InlineData("$(HTTP_HOST)=='example.com'", true)]
        [InlineData("$(HTTP_HOST)=='foo.com'", false)]
        [InlineData("$(HTTP_HOST)!='example.com'", false)]
        [InlineData("$(HTTP_HOST)!='foo.com'", true)]
        [InlineData("'a'=='b' && 'c'=='c'", false)]
        [InlineData("'a'=='b' || 'c'=='c'", true)]
        [InlineData("'a'=='a' && '1'=='2' && 'c'=='c'", false)]
        [InlineData("'a'=='a' || '1'=='2' && 'c'=='c'", true)]
        [InlineData("'a'=='a' && '1'=='2' || 'c'=='c'", true)]
        [InlineData("'a'=='a' && '1'=='1' && 'b'=='c'", false)]
        [InlineData("'a'=='a' || '1'=='1' && 'b'=='c'", true)]
        [InlineData("'a'=='a' && '1'=='1' || 'b'=='c'", true)]
        [InlineData("'a'=='b' && '1'=='2' || 'c'=='c'", true)]
        public void Parse_and_evaluate(string input, bool expected)
        {
            var variables = new Dictionary<string, string>
            {
                ["HTTP_HOST"] = "example.com"
            };

            var expression = WhenParser.Parse(input);
            var evaluated = WhenEvaluator.Evaluate(expression, variables);

            evaluated.Should().Be.EqualTo(expected);
        }
    }
}