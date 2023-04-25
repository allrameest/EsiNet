namespace EsiNet.Caching.Serialization
{
    public static class Serializer
    {
        public static ISerializer Hyperion()
        {
            return new HyperionSerializer();
        }
    }
}