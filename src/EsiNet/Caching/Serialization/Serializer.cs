namespace EsiNet.Caching.Serialization
{
    public static class Serializer
    {
        public static ISerializer Binary()
        {
            return new BinarySerializer();
        }

        public static ISerializer Wire()
        {
            return new WireSerializer();
        }
    }
}