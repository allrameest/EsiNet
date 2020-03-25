using System;

namespace EsiNet.Expressions
{
    public static class ExpressionReaderExtensions
    {
        public static char ReadChar(this ExpressionReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return (char) reader.Read();
        }

        public static char PeekChar(this ExpressionReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return (char) reader.Peek();
        }
        
        public static void SkipWhitespace(this ExpressionReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            
            while (char.IsWhiteSpace(reader.PeekChar()))
            {
                reader.Read();
            }
        }

        public static Exception UnexpectedCharacterException(this ExpressionReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            
            var position = reader.LastAccessedPosition;
            return new InvalidExpressionException(
                $"Unexpected character at position {position}" + Environment.NewLine +
                reader.OriginalText + Environment.NewLine +
                new string(' ', position) + '\u21D1');
        }
    }
}