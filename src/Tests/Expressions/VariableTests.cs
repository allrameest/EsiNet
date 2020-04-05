using System;
using System.Collections.Generic;
using EsiNet;
using EsiNet.Expressions;
using SharpTestsEx;
using Xunit;

namespace Tests.Expressions
{
    public class VariableTests
    {
        [Theory]
        [InlineData(@"Host: $(HTTP_HOST)", "Host: example.com")]
        [InlineData(@"Vat: $(HTTP_COOKIE{showPricesWithVat})", "Vat: true")]
        [InlineData(@"Host: $(HTTP_HOST), Vat: $(HTTP_COOKIE{showPricesWithVat})", "Host: example.com, Vat: true")]
        [InlineData(@"[$(HTTP_COOKIE{unknown})]", "[]")]
        [InlineData(@"[$(WHATEVER)]", "[]")]
        [InlineData(@"$(HTTP_HOST|localhost)", "example.com")]
        [InlineData(@"$(WHATEVER|default)", "default")]
        [InlineData(@"$(WHATEVER|'default')", "default")]
        [InlineData(@"$(WHATEVER|'')", "")]
        [InlineData(@"$(WHATEVER|)", "")]
        [InlineData(@"$(WHATEVER|' \\ \' \r\n ')", " \\ ' \r\n ")]
        [InlineData(@"$(HTTP_COOKIE{showPricesWithVat}|default)", "true")]
        [InlineData(@"$(HTTP_COOKIE{whatever}|default)", "default")]
        [InlineData(@"$(HTTP_COOKIE{whatever}|'default')", "default")]
        public void Parse_and_build(string input, string expected)
        {
            var variables = new Dictionary<string, IVariableValueResolver>
            {
                ["HTTP_HOST"] = new SimpleVariableValueResolver(new Lazy<string>("example.com")),
                ["HTTP_COOKIE"] = new DictionaryVariableValueResolver(new Lazy<IReadOnlyDictionary<string, string>>(
                    new Dictionary<string, string>
                    {
                        ["showPricesWithVat"] = "true"
                    }))
            };
            var executionContext = new EsiExecutionContext(
                new Dictionary<string, IReadOnlyCollection<string>>(), variables);

            var variableString = VariableStringParser.Parse(input);
            var actual = string.Concat(VariableStringResolver.Resolve(executionContext, variableString));

            actual.Should().Be.EqualTo(expected);
        }
    }
}