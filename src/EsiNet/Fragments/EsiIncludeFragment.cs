using System;

namespace EsiNet.Fragments
{
    [Serializable]
    public class EsiIncludeFragment : IEsiFragment
    {
        public EsiIncludeFragment(string url)
        {
            Url = url;
        }

        public string Url { get; }
    }
}