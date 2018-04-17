namespace EsiNet.Caching
{
    public static class Serializer
    {
        public static ISerializer<T> Binary<T>()
        {
            return new BinarySerializer<T>();
        }
    }
}