using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EsiNet.Fragments.Choose
{
    public class WhenParser
    {
        public static IWhenExpression Parse(string text)
        {
            using (var reader = new ExpressionReader(text))
            {
                return ParseExpression(reader, BooleanOperator.And);
            }
        }

        private static IWhenExpression ParseExpression(ExpressionReader reader, BooleanOperator booleanOperator)
        {
            var expressions = ParseGroup(reader, booleanOperator).ToList();
            return expressions.Count == 1
                ? expressions.Single()
                : new GroupExpression(expressions, booleanOperator);
        }

        private static IEnumerable<IWhenExpression> ParseGroup(ExpressionReader reader, BooleanOperator booleanOperator)
        {
            SkipWhitespace(reader);

            yield return ParseComparison(reader, booleanOperator);

            SkipWhitespace(reader);

            if (reader.Peek() == -1) yield break;

            while (reader.Peek() != -1)
            {
                switch (reader.PeekChar())
                {
                    case '|':
                    case '&':
                        yield return ParseBooleanExpression(reader);
                        SkipWhitespace(reader);
                        break;
                    default:
                        throw UnexpectedCharacterException(reader);
                }
            }

            if (reader.Peek() != -1) throw UnexpectedCharacterException(reader);
        }

        private static IWhenExpression ParseBooleanExpression(ExpressionReader reader)
        {
            var booleanOperator = ParseBooleanOperator(reader);
            SkipWhitespace(reader);
            return ParseComparison(reader, booleanOperator);
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

            throw UnexpectedCharacterException(reader);
        }

        private static ComparisonExpression ParseComparison(ExpressionReader reader, BooleanOperator booleanOperator)
        {
            var left = ParseValue(reader);

            SkipWhitespace(reader);

            var comparisonOperator = ParseOperator(reader);

            SkipWhitespace(reader);

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

            throw UnexpectedCharacterException(reader);
        }

        private static ValueExpression ParseValue(ExpressionReader reader)
        {
            var c = reader.PeekChar();
            switch (c)
            {
                case '$':
                    return ParseVariable(reader);
                case '\'':
                    return ParseConstant(reader);
                default:
                    throw UnexpectedCharacterException(reader);
            }
        }

        private static VariableExpression ParseVariable(ExpressionReader reader)
        {
            reader.Read(); //Skip $

            if (reader.Read() != '(') throw UnexpectedCharacterException(reader);

            SkipWhitespace(reader);

            var name = new StringBuilder();
            while (IsNameChar(reader.PeekChar()))
            {
                name.Append(reader.ReadChar());
            }

            if (name.Length == 0) throw UnexpectedCharacterException(reader);

            SkipWhitespace(reader);

            if (reader.Read() != ')') throw UnexpectedCharacterException(reader);

            return new VariableExpression(name.ToString());
        }

        private static ConstantExpression ParseConstant(ExpressionReader reader)
        {
            reader.Read(); //Skip '

            var value = new StringBuilder();
            while (reader.Peek() != -1)
            {
                var c = reader.ReadChar();
                switch (c)
                {
                    case '\\':
                        value.Append(ParseEscapedCharacter(reader));
                        break;
                    case '\'':
                        return new ConstantExpression(value.ToString());
                    default:
                        value.Append(c);
                        break;
                }
            }

            throw UnexpectedCharacterException(reader);
        }

        private static char ParseEscapedCharacter(ExpressionReader reader)
        {
            var c = reader.ReadChar();

            switch (c)
            {
                case '\'':
                case '\\':
                    return c;
                case 'b':
                    return '\b';
                case 'f':
                    return '\f';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                case 'u':
                    return ParseUnicodeCharacter(reader);
                default:
                    throw UnexpectedCharacterException(reader);
            }
        }

        private static char ParseUnicodeCharacter(ExpressionReader reader)
        {
            var code = new StringBuilder();
            for (var i = 0; i < 4; i++)
            {
                var c = reader.ReadChar();
                if (!IsHexChar(c)) throw UnexpectedCharacterException(reader);
                code.Append(c);
            }

            return (char) int.Parse(code.ToString(), NumberStyles.HexNumber);
        }

        private static bool IsNameChar(char c) => char.IsLetter(c) || c == '_';

        private static bool IsHexChar(char c) =>
            c >= '0' && c <= '9' ||
            c >= 'a' && c <= 'f' ||
            c >= 'A' && c <= 'F';

        private static void SkipWhitespace(ExpressionReader reader)
        {
            while (char.IsWhiteSpace(reader.PeekChar()))
            {
                reader.Read();
            }
        }

        private static Exception UnexpectedCharacterException(ExpressionReader reader)
        {
            var position = reader.LastAccessedPosition;
            return new InvalidWhenExpressionException(
                $"Unexpected character at position {position}" + Environment.NewLine +
                reader.OriginalText + Environment.NewLine +
                new string(' ', position) + '\u21D1');
        }
    }
}