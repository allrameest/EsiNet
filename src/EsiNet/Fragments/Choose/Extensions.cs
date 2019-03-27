using System;

namespace EsiNet.Fragments.Choose
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
    }
}