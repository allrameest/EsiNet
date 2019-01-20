using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EsiNet.Caching
{
    public class MemoryVaryHeaderStore : IVaryHeaderStore
    {
        private readonly MemoryCache _cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        public bool TryGet(Uri uri, out IReadOnlyCollection<string> headerNames)
        {
            return _cache.TryGetValue(uri, out headerNames);
        }

        public void Set(Uri uri, IReadOnlyCollection<string> headerNames)
        {
            _cache.Set(uri, headerNames);
        }
    }
}