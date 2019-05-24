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
        [InlineData("'a'=='b' && ('1'=='2' || 'c'=='c')", false)]
        [InlineData("('a'=='b')", false)]
        [InlineData("('a'=='a')", true)]
        [InlineData("(('a'=='a'))", true)]
        [InlineData("('a'=='b' || 'a'=='a') && ('b'=='b' || 'b'=='a')", true)]
        [InlineData("('a'=='b' || 'a'=='a') && ('b'=='c' || 'b'=='a')", false)]
        [InlineData("$(HTTP_COOKIE{showPricesWithVat})=='true'", true)]
        [InlineData("$(HTTP_COOKIE{showPricesWithVat})=='false'", false)]
        [InlineData("$(WHATEVER{whatever})==''", false)]
        public void Parse_and_evaluate(string input, bool expected)
        {
            var variables = new Dictionary<string, IVariableValueResolver>
            {
                ["HTTP_HOST"] = new SimpleVariableValueResolver("example.com"),
                ["HTTP_COOKIE"] = new DictionaryVariableValueResolver(new Dictionary<string, string>
                {
                    ["showPricesWithVat"] = "true"
                })
            };

            var expression = WhenParser.Parse(input);
            var evaluated = WhenEvaluator.Evaluate(expression, variables);

            evaluated.Should().Be.EqualTo(expected);
        }
    }
}