using System;
using System.Collections.Generic;

namespace EsiNet.Caching
{
    public class CacheKey
    {
        private readonly string _toString;

        public CacheKey(Uri uri, IReadOnlyCollection<string> varyHeaderValues)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            VaryHeaderValues = varyHeaderValues ?? throw new ArgumentNullException(nameof(varyHeaderValues));
            _toString = CreateString(uri, varyHeaderValues);
        }

        public Uri Uri { get; }
        public IReadOnlyCollection<string> VaryHeaderValues { get; }

        public override string ToString()
        {
            return _toString;
        }

        private static string CreateString(Uri uri, IEnumerable<string> varyHeaderValues)
        {
            return $"{uri}\t{CacheVersion.Version}\t{string.Join("\t", varyHeaderValues)}";
        }

        private bool Equals(CacheKey other)
        {
            return string.Equals(_toString, other._toString);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CacheKey key && Equals(key);
        }

        public override int GetHashCode()
        {
            return _toString.GetHashCode();
        }
    }
}