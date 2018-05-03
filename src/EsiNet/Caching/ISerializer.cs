using System.IO;

namespace EsiNet.Caching
{
    public interface ISerializer
    {
        void Serialize<T>(T value, Stream destination);
        T Deserialize<T>(Stream source);
    }
}