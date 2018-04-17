using System;
using System.IO;

namespace EsiNet.Caching
{
    public static class SerializerExtensions
    {
        public static byte[] SerializeBytes<T>(this ISerializer<T> serializer, T value)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(value, stream);
                return stream.ToArray();
            }
        }

        public static T DeserializeBytes<T>(this ISerializer<T> serializer, byte[] value)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            using (var stream = new MemoryStream(value))
            {
                return serializer.Deserialize(stream);
            }
        }

        public static ISerializer<T> GZip<T>(this ISerializer<T> innerSerializer)
        {
            return new GZipSerializerDecorator<T>(innerSerializer);
        }
    }
}