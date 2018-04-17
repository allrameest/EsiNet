namespace EsiNet
{
    public class EsiIncludeFragment : IEsiFragment
    {
        public EsiIncludeFragment(string url)
        {
            Url = url;
        }

        public string Url { get; }
    }
}