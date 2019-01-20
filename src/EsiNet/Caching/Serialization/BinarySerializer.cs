using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EsiNet.Caching.Serialization
{
    public class BinarySerializer : ISerializer
    {
        private readonly BinaryFormatter _serializer = new BinaryFormatter();

        public void Serialize<T>(T value, Stream destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            _serializer.Serialize(destination, value);
        }

        public T Deserialize<T>(Stream source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return (T) _serializer.Deserialize(source);
        }
    }
}