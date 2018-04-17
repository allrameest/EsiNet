using System;
using System.IO;
using System.IO.Compression;

namespace EsiNet.Caching
{
    public class GZipSerializerDecorator<T> : ISerializer<T>
    {
        private const int BufferSize = 8192;
        private readonly ISerializer<T> _innerSerializer;

        public GZipSerializerDecorator(ISerializer<T> innerSerializer)
        {
            _innerSerializer = innerSerializer;
        }

        public void Serialize(T value, Stream destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            using (var compress = new GZipStream(destination, CompressionMode.Compress))
            using (var buffer = new BufferedStream(compress, BufferSize))
            {
                _innerSerializer.Serialize(value, buffer);
            }
        }

        public T Deserialize(Stream source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            using (var compress = new GZipStream(source, CompressionMode.Decompress))
            {
                return _innerSerializer.Deserialize(compress);
            }
        }
    }
}