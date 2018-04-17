using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EsiNet.Caching
{
    public class BinarySerializer<T> : ISerializer<T>
    {
        private readonly BinaryFormatter _serializer = new BinaryFormatter();

        public void Serialize(T value, Stream destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            _serializer.Serialize(destination, value);
        }

        public T Deserialize(Stream source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return (T) _serializer.Deserialize(source);
        }
    }
}