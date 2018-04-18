using System;
using System.IO;

namespace EsiNet.Caching
{
    public class WireSerializer<T> : ISerializer<T>
    {
        private readonly Wire.Serializer _serializer = new Wire.Serializer();

        public void Serialize(T value, Stream destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            _serializer.Serialize(value, destination);
        }

        public T Deserialize(Stream source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return _serializer.Deserialize<T>(source);
        }
    }
}