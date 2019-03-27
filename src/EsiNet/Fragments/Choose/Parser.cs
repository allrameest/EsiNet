using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace EsiNet.Fragments.Choose
{
    public static class WhenParser
    {
        public static ComparisonExpression Parse(string text)
        {
            using (var reader = new StringReader(text))
            {
                SkipWhitespace(reader);

                var left = ParseValue(reader);

                SkipWhitespace(reader);

                var comparisonOperator = ParseOperator(reader);

                SkipWhitespace(reader);

                var right = ParseValue(reader);

                SkipWhitespace(reader);

                if (reader.Peek() != -1) throw new Exception();

                return new ComparisonExpression(left, right, comparisonOperator);
            }
        }

        private static ComparisonOperator ParseOperator(StringReader reader)
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

            throw new Exception();
        }

        private static ValueExpression ParseValue(StringReader reader)
        {
            var c = reader.PeekChar();
            switch (c)
            {
                case '$':
                    return ParseVariable(reader);
                case '\'':
                    return ParseConstant(reader);
                default:
                    throw new Exception();
            }
        }

        private static VariableExpression ParseVariable(StringReader reader)
        {
            reader.Read(); //Skip $

            if (reader.Read() != '(') throw new Exception();

            SkipWhitespace(reader);

            var name = new StringBuilder();
            while (IsNameChar(reader.PeekChar()))
            {
                name.Append(reader.ReadChar());
            }

            if (name.Length == 0) throw new Exception();

            SkipWhitespace(reader);

            if (reader.Read() != ')') throw new Exception();

            return new VariableExpression(name.ToString());
        }

        private static ConstantExpression ParseConstant(StringReader reader)
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

            throw new Exception();
        }

        private static char ParseEscapedCharacter(StringReader reader)
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
                    throw new Exception();
            }
        }

        private static char ParseUnicodeCharacter(StringReader reader)
        {
            var code = new StringBuilder();
            for (var i = 0; i < 4; i++)
            {
                var c = reader.ReadChar();
                if (!IsHexChar(c)) throw new Exception();
                code.Append(c);
            }

            return (char) int.Parse(code.ToString(), NumberStyles.HexNumber);
        }

        private static bool IsNameChar(char c) => char.IsLetter(c) || c == '_';

        private static bool IsHexChar(char c) =>
            c >= '0' && c <= '9' ||
            c >= 'a' && c <= 'f' ||
            c >= 'A' && c <= 'F';

        private static void SkipWhitespace(StringReader reader)
        {
            while (char.IsWhiteSpace(reader.PeekChar()))
            {
                reader.Read();
            }
        }
    }
}