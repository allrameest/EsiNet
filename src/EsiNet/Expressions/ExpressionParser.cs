using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EsiNet.Expressions
{
    public static class ExpressionParser
    {
        public static IBooleanExpression Parse(string text)
        {
            using (var reader = new ExpressionReader(text))
            {
                return ParseExpression(reader, BooleanOperator.And);
            }
        }

        private static IBooleanExpression ParseExpression(ExpressionReader reader, BooleanOperator booleanOperator)
        {
            var expressions = ParseBooleanExpressions(reader, booleanOperator).ToArray();

            if (reader.Peek() != -1) throw reader.UnexpectedCharacterException();

            return expressions.Length == 1
                ? expressions.Single()
                : new GroupExpression(expressions, booleanOperator);
        }

        private static IEnumerable<IBooleanExpression> ParseBooleanExpressions(
            ExpressionReader reader, BooleanOperator booleanOperator)
        {
            reader.SkipWhitespace();

            yield return ParseExpressionPart(reader, booleanOperator);

            reader.SkipWhitespace();

            if (reader.Peek() == -1) yield break;

            while (reader.Peek() != -1)
            {
                switch (reader.PeekChar())
                {
                    case '|':
                    case '&':
                        var @operator = ParseBooleanOperator(reader);
                        reader.SkipWhitespace();
                        yield return ParseExpressionPart(reader, @operator);
                        reader.SkipWhitespace();
                        break;
                    case ')':
                        yield break;
                    default:
                        throw reader.UnexpectedCharacterException();
                }
            }

            if (reader.Peek() != -1) throw reader.UnexpectedCharacterException();
        }

        private static IBooleanExpression ParseExpressionPart(ExpressionReader reader, BooleanOperator booleanOperator)
        {
            reader.SkipWhitespace();

            if (reader.PeekChar() != '(')
            {
                return ParseComparison(reader, booleanOperator);
            }

            var expressions = ParseGroup(reader);
            if (expressions.Count == 1)
            {
                return EnsureOperator(expressions.Single(), booleanOperator);
            }

            return new GroupExpression(expressions, booleanOperator);

        }

        private static IBooleanExpression EnsureOperator(IBooleanExpression expression, BooleanOperator booleanOperator)
        {
            if (expression.BooleanOperator == booleanOperator)
            {
                return expression;
            }

            if (expression is GroupExpression group)
            {
                return new GroupExpression(group.BooleanExpressions, booleanOperator);
            }

            if (expression is ComparisonExpression comparison)
            {
                return new ComparisonExpression(
                    comparison.Left, comparison.Right, comparison.ComparisonOperator, booleanOperator);
            }

            throw new Exception($"Expression type {expression.GetType().Name} not supported");
        }

        private static IReadOnlyCollection<IBooleanExpression> ParseGroup(ExpressionReader reader)
        {
            reader.Read(); //Skip (
            reader.SkipWhitespace();

            var result = ParseBooleanExpressions(reader, BooleanOperator.And).ToArray();

            reader.SkipWhitespace();

            var c = reader.ReadChar();
            if (c != ')')
            {
                throw reader.UnexpectedCharacterException();
            }

            return result;
        }

        private static BooleanOperator ParseBooleanOperator(ExpressionReader reader)
        {
            var c0 = reader.ReadChar();
            var c1 = reader.PeekChar();

            if (c0 == '&')
            {
                if (c1 == '&') reader.ReadChar();
                return BooleanOperator.And;
            }

            if (c0 == '|')
            {
                if (c1 == '|') reader.ReadChar();
                return BooleanOperator.Or;
            }

            throw reader.UnexpectedCharacterException();
        }

        private static ComparisonExpression ParseComparison(ExpressionReader reader, BooleanOperator booleanOperator)
        {
            var left = ParseValue(reader);

            reader.SkipWhitespace();

            var comparisonOperator = ParseOperator(reader);

            reader.SkipWhitespace();

            var right = ParseValue(reader);

            return new ComparisonExpression(left, right, comparisonOperator, booleanOperator);
        }

        private static ComparisonOperator ParseOperator(ExpressionReader reader)
        {
            var c = reader.ReadChar();

            if (c == '=' && reader.PeekChar() == '=')
            {
                reader.Read();
                return ComparisonOperator.Equal;
            }

            if (c == '!' && reader.PeekChar() == '=')
            {
                reader.Read();
                return ComparisonOperator.NotEqual;
            }

            if (c == '>' && reader.PeekChar() == '=')
            {
                reader.Read();
                return ComparisonOperator.GreaterThanOrEqual;
            }

            if (c == '<' && reader.PeekChar() == '=')
            {
                reader.Read();
                return ComparisonOperator.LessThanOrEqual;
            }

            if (c == '>')
                return ComparisonOperator.GreaterThan;

            if (c == '<')
                return ComparisonOperator.LessThan;

            throw reader.UnexpectedCharacterException();
        }

        private static ValueExpression ParseValue(ExpressionReader reader)
        {
            var c = reader.PeekChar();
            switch (c)
            {
                case '$':
                    return VariableParser.ParseVariable(reader);
                case '\'':
                    return ParseConstant(reader);
                default:
                    throw reader.UnexpectedCharacterException();
            }
        }

        private static ConstantExpression ParseConstant(ExpressionReader reader)
        {
            var constantValue = ConstantParser.Parse(reader);
            return new ConstantExpression(constantValue);
        }
    }
}