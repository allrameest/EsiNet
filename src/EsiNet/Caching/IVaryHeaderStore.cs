using System;
using System.Collections.Generic;

namespace EsiNet.Caching
{
    public interface IVaryHeaderStore
    {
        bool TryGet(Uri uri, out IReadOnlyCollection<string> headerNames);
        void Set(Uri uri, IReadOnlyCollection<string> headerNames);
    }
}