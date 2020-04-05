using System;
using System.Globalization;
using System.Text;

namespace EsiNet.Expressions
{
    public static class ConstantParser
    {
        public static string Parse(ExpressionReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            if (reader.ReadChar() != '\'') throw reader.UnexpectedCharacterException();

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
                        return value.ToString();
                    default:
                        value.Append(c);
                        break;
                }
            }

            throw reader.UnexpectedCharacterException();
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
                    throw reader.UnexpectedCharacterException();
            }
        }

        private static char ParseUnicodeCharacter(ExpressionReader reader)
        {
            var code = new StringBuilder();
            for (var i = 0; i < 4; i++)
            {
                var c = reader.ReadChar();
                if (!IsHexChar(c)) throw reader.UnexpectedCharacterException();
                code.Append(c);
            }

            return (char) int.Parse(code.ToString(), NumberStyles.HexNumber);
        }

        private static bool IsHexChar(char c) =>
            c >= '0' && c <= '9' ||
            c >= 'a' && c <= 'f' ||
            c >= 'A' && c <= 'F';
    }
}