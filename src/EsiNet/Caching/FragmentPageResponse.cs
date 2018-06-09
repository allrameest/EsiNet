using System;
using System.Collections.Generic;
using EsiNet.Fragments;

namespace EsiNet.Caching
{
    [Serializable]
    public class FragmentPageResponse
    {
        public FragmentPageResponse(IEsiFragment fragment, IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers)
        {
            Fragment = fragment ?? throw new ArgumentNullException(nameof(fragment));
            Headers = headers;
        }

        public IEsiFragment Fragment { get; }
        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers { get; }
    }
}