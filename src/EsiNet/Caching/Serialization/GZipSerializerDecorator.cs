using System;
using System.IO;
using System.IO.Compression;

namespace EsiNet.Caching.Serialization
{
    public class GZipSerializerDecorator : ISerializer
    {
        private const int BufferSize = 8192;
        private readonly ISerializer _innerSerializer;

        public GZipSerializerDecorator(ISerializer innerSerializer)
        {
            _innerSerializer = innerSerializer ?? throw new ArgumentNullException(nameof(innerSerializer));
        }

        public void Serialize<T>(T value, Stream destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            using (var compress = new GZipStream(destination, CompressionMode.Compress))
            using (var buffer = new BufferedStream(compress, BufferSize))
            {
                _innerSerializer.Serialize(value, buffer);
            }
        }

        public T Deserialize<T>(Stream source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            using (var compress = new GZipStream(source, CompressionMode.Decompress))
            {
                return _innerSerializer.Deserialize<T>(compress);
            }
        }
    }
}