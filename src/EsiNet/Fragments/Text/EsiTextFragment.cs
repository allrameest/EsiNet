using System;

namespace EsiNet.Fragments.Text
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