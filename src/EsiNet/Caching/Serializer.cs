namespace EsiNet.Caching
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