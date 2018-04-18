using System;

namespace EsiNet.Fragments
{
    [Serializable]
    public class EsiTextFragment : IEsiFragment
    {
        public EsiTextFragment(string body)
        {
            Body = body;
        }

        public string Body { get; }
    }
}