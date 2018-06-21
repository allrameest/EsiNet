using System;

namespace EsiNet.Caching
{
    public class CacheEnvelope<T>
    {
        public CacheEnvelope(T body, TimeSpan expirationTime)
        {
            Body = body;
            ExpirationTime = expirationTime;
        }

        public T Body { get; }
        public TimeSpan ExpirationTime { get; }
    }
}