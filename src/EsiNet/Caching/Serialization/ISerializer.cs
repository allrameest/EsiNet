using System.IO;

namespace EsiNet.Caching.Serialization
{
    public interface ISerializer
    {
        void Serialize<T>(T value, Stream destination);
        T Deserialize<T>(Stream source);
    }
}