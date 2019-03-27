using System;
using System.IO;

namespace EsiNet.Fragments.Choose
{
    public static class StringReaderExtensions
    {
        public static char ReadChar(this StringReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return (char)reader.Read();
        }

        public static char PeekChar(this StringReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return (char)reader.Peek();
        }
    }
}