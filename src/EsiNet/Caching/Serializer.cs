namespace EsiNet.Caching
{
    public static class Serializer
    {
        public static ISerializer<T> Binary<T>()
        {
            return new BinarySerializer<T>();
        }

        public static ISerializer<T> Wire<T>()
        {
            return new WireSerializer<T>();
        }
    }
}