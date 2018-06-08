using System;

namespace EsiNet.Fragments
{
    [Serializable]
    public class EsiTextFragment : IEsiFragment
    {
        public EsiTextFragment(string body)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public string Body { get; }
    }
}