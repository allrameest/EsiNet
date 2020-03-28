namespace EsiNet.Caching
{
    public static class CacheVersion
    {
        // Used in cache key and bumped when cached object changes
        // This is done to avoid problems with deserialization of old data
        public const string Version = "2";
    }
}