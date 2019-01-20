using System;
using System.IO;

namespace EsiNet.Caching.Serialization
{
    public static class SerializerExtensions
    {
        public static byte[] SerializeBytes<T>(this ISerializer serializer, T value)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(value, stream);
                return stream.ToArray();
            }
        }

        public static T DeserializeBytes<T>(this ISerializer serializer, byte[] value)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            using (var stream = new MemoryStream(value))
            {
                return serializer.Deserialize<T>(stream);
            }
        }

        public static ISerializer GZip(this ISerializer innerSerializer)
        {
            if (innerSerializer == null) throw new ArgumentNullException(nameof(innerSerializer));
            return new GZipSerializerDecorator(innerSerializer);
        }
    }
}