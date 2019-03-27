using System;
using System.Globalization;
using System.Text;

namespace EsiNet.Fragments.Choose
{
    public class WhenParser
    {
        public static ComparisonExpression Parse(string text)
        {
            using (var reader = new ExpressionReader(text))
            {
                SkipWhitespace(reader);

                var left = ParseValue(reader);

                SkipWhitespace(reader);

                var comparisonOperator = ParseOperator(reader);

                SkipWhitespace(reader);

                var right = ParseValue(reader);

                SkipWhitespace(reader);

                if (reader.Peek() != -1) throw UnexpectedCharacterException(reader);

                return new ComparisonExpression(left, right, comparisonOperator);
            }
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