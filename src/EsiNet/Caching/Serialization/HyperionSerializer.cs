using System;
using System.IO;

namespace EsiNet.Caching.Serialization
{
    public class HyperionSerializer : ISerializer
    {
        private readonly Hyperion.Serializer _serializer = new Hyperion.Serializer();

        public void Serialize<T>(T value, Stream destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            _serializer.Serialize(value, destination);
        }

        public T Deserialize<T>(Stream source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return _serializer.Deserialize<T>(source);
        }
    }
}