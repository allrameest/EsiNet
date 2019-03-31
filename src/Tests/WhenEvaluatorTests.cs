using System.Collections.Generic;
using EsiNet.Fragments.Choose;
using SharpTestsEx;
using Xunit;

namespace Tests
{
    public class WhenEvaluatorTests
    {
        [Theory]
        [InlineData("HTTP_HOST", "example.com", ComparisonOperator.Equal, true)]
        [InlineData("HTTP_HOST", "example.com", ComparisonOperator.NotEqual, false)]
        [InlineData("HTTP_HOST", "foo.com", ComparisonOperator.Equal, false)]
        [InlineData("HTTP_HOST", "foo.com", ComparisonOperator.NotEqual, true)]
        [InlineData("HTTP_REFERER", "example.com", ComparisonOperator.Equal, false)]
        public void Evaluate_single_comparison(
            string variableName, string value, ComparisonOperator comparisonOperator, bool expected)
        {
            var variables = new Dictionary<string, string>
            {
                ["HTTP_HOST"] = "example.com"
            };
            var expression = new ComparisonExpression(
                new VariableExpression(variableName),
                new ConstantExpression(value),
                comparisonOperator,
                BooleanOperator.And);

            var actual = WhenEvaluator.Evaluate(expression, variables);

            actual.Should().Be.EqualTo(expected);
        }

        [Theory]
        [InlineData("1", "2", ComparisonOperator.GreaterThan, false)]
        [InlineData("1", "1", ComparisonOperator.GreaterThan, false)]
        [InlineData("2", "1", ComparisonOperator.GreaterThan, true)]
        [InlineData("1", "2", ComparisonOperator.GreaterThanOrEqual, false)]
        [InlineData("1", "1", ComparisonOperator.GreaterThanOrEqual, true)]
        [InlineData("2", "1", ComparisonOperator.GreaterThanOrEqual, true)]
        [InlineData("1", "2", ComparisonOperator.LessThan, true)]
        [InlineData("1", "1", ComparisonOperator.LessThan, false)]
        [InlineData("2", "1", ComparisonOperator.LessThan, false)]
        [InlineData("1", "2", ComparisonOperator.LessThanOrEqual, true)]
        [InlineData("1", "1", ComparisonOperator.LessThanOrEqual, true)]
        [InlineData("2", "1", ComparisonOperator.LessThanOrEqual, false)]
        [InlineData("2", "1000", ComparisonOperator.GreaterThan, true)]
        public void Evaluate_greater_less_than(
            string left, string right, ComparisonOperator comparisonOperator, bool expected)
        {
            var variables = new Dictionary<string, string>();
            var expression = new ComparisonExpression(
                new ConstantExpression(left),
                new ConstantExpression(right),
                comparisonOperator,
                BooleanOperator.And);

            var actual = WhenEvaluator.Evaluate(expression, variables);

            actual.Should().Be.EqualTo(expected);
        }

        [Theory]
        [InlineData(BooleanOperator.And, false)]
        [InlineData(BooleanOperator.Or, true)]
        public void Evaluate_multiple(BooleanOperator booleanOperator, bool expected)
        {
            var variables = new Dictionary<string, string>();
            var expression = new GroupExpression(new[]
            {
                new ComparisonExpression(
                    new ConstantExpression("a"),
                    new ConstantExpression("b"),
                    ComparisonOperator.Equal,
                    BooleanOperator.And),
                new ComparisonExpression(
                    new ConstantExpression("c"),
                    new ConstantExpression("c"),
                    ComparisonOperator.Equal,
                    booleanOperator)
            }, BooleanOperator.And);

            var actual = WhenEvaluator.Evaluate(expression, variables);

            actual.Should().Be.EqualTo(expected);
        }

        [Fact]
        public void Evaluate_grouped()
        {
            var variables = new Dictionary<string, string>();
            var expression = new GroupExpression(new IWhenExpression[]
            {
                new ComparisonExpression(
                    new ConstantExpression("a"),
                    new ConstantExpression("b"),
                    ComparisonOperator.Equal,
                    BooleanOperator.And),
                new GroupExpression(new[]
                    {
                        new ComparisonExpression(
                            new ConstantExpression("1"),
                            new ConstantExpression("2"),
                            ComparisonOperator.Equal,
                            BooleanOperator.And),
                        new ComparisonExpression(
                            new ConstantExpression("c"),
                            new ConstantExpression("c"),
                            ComparisonOperator.Equal,
                            BooleanOperator.Or),
                    },
                    BooleanOperator.And)
            }, BooleanOperator.And);

            var actual = WhenEvaluator.Evaluate(expression, variables);

            actual.Should().Be.False();
        }
    }
}