using System.IO;

namespace EsiNet.Caching
{
    public interface ISerializer<T>
    {
        void Serialize(T value, Stream destination);
        T Deserialize(Stream source);
    }
}