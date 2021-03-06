﻿using System;
using System.Collections.Generic;
using EsiNet.Expressions;
using SharpTestsEx;
using Xunit;

namespace Tests.Expressions
{
    public class ExpressionEvaluatorTests
    {
        [Theory]
        [InlineData("HTTP_HOST", "example.com", ComparisonOperator.Equal, true)]
        [InlineData("http_host", "example.com", ComparisonOperator.Equal, false)]
        [InlineData("HTTP_HOST", "example.com", ComparisonOperator.NotEqual, false)]
        [InlineData("HTTP_HOST", "foo.com", ComparisonOperator.Equal, false)]
        [InlineData("HTTP_HOST", "foo.com", ComparisonOperator.NotEqual, true)]
        [InlineData("HTTP_REFERER", "example.com", ComparisonOperator.Equal, false)]
        [InlineData("HTTP_REFERER", "", ComparisonOperator.Equal, false)]
        [InlineData("HTTP_REFERER", "", ComparisonOperator.NotEqual, true)]
        public void Evaluate_single_comparison(
            string variableName, string value, ComparisonOperator comparisonOperator, bool expected)
        {
            var variables = new Dictionary<string, IVariableValueResolver>
            {
                ["HTTP_HOST"] = new SimpleVariableValueResolver(new Lazy<string>("example.com"))
            };
            var expression = new ComparisonExpression(
                new SimpleVariableExpression(variableName),
                new ConstantExpression(value),
                comparisonOperator,
                BooleanOperator.And);

            var actual = ExpressionEvaluator.Evaluate(expression, variables);

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
        [InlineData("0", "", ComparisonOperator.GreaterThan, true)]
        [InlineData(" ", "", ComparisonOperator.GreaterThan, true)]
        public void Evaluate_greater_less_than(
            string left, string right, ComparisonOperator comparisonOperator, bool expected)
        {
            var variables = new Dictionary<string, IVariableValueResolver>();
            var expression = new ComparisonExpression(
                new ConstantExpression(left),
                new ConstantExpression(right),
                comparisonOperator,
                BooleanOperator.And);

            var actual = ExpressionEvaluator.Evaluate(expression, variables);

            actual.Should().Be.EqualTo(expected);
        }

        [Theory]
        [InlineData(BooleanOperator.And, false)]
        [InlineData(BooleanOperator.Or, true)]
        public void Evaluate_multiple(BooleanOperator booleanOperator, bool expected)
        {
            var variables = new Dictionary<string, IVariableValueResolver>();
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

            var actual = ExpressionEvaluator.Evaluate(expression, variables);

            actual.Should().Be.EqualTo(expected);
        }

        [Fact]
        public void Evaluate_grouped()
        {
            var variables = new Dictionary<string, IVariableValueResolver>();
            var expression = new GroupExpression(new IBooleanExpression[]
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

            var actual = ExpressionEvaluator.Evaluate(expression, variables);

            actual.Should().Be.False();
        }

        [Theory]
        [InlineData("HTTP_COOKIE", "showPricesWithVat", "true", ComparisonOperator.Equal, true)]
        [InlineData("HTTP_COOKIE", "showPricesWithVat", "false", ComparisonOperator.Equal, false)]
        [InlineData("HTTP_COOKIE", "showPricesWithVat", "TRUE", ComparisonOperator.Equal, false)]
        [InlineData("HTTP_COOKIE", "SHOWPRICESWITHVAT", "true", ComparisonOperator.Equal, false)]
        [InlineData("HTTP_COOKIE", "showPricesWithVat", "true", ComparisonOperator.NotEqual, false)]
        [InlineData("HTTP_COOKIE", "showPricesWithVat", "false", ComparisonOperator.NotEqual, true)]
        public void Evaluate_dictionary_comparison(
            string variableName, string key, string value, ComparisonOperator comparisonOperator, bool expected)
        {
            var variables = new Dictionary<string, IVariableValueResolver>
            {
                ["HTTP_COOKIE"] = new DictionaryVariableValueResolver(new Lazy<IReadOnlyDictionary<string, string>>(
                    new Dictionary<string, string>
                    {
                        ["showPricesWithVat"] = "true"
                    }))
            };
            var expression = new ComparisonExpression(
                new DictionaryVariableExpression(variableName, key),
                new ConstantExpression(value),
                comparisonOperator,
                BooleanOperator.And);

            var actual = ExpressionEvaluator.Evaluate(expression, variables);

            actual.Should().Be.EqualTo(expected);
        }
    }
}