using System;
using EsiNet.Fragments;

namespace EsiNet.Caching
{
    [Serializable]
    public class FragmentPageResponse
    {
        public FragmentPageResponse(IEsiFragment fragment, string contentType)
        {
            Fragment = fragment ?? throw new ArgumentNullException(nameof(fragment));
            ContentType = contentType;
        }

        public IEsiFragment Fragment { get; }
        public string ContentType { get; }
    }
}