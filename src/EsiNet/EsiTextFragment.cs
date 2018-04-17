namespace EsiNet
{
    public class EsiTextFragment : IEsiFragment
    {
        public EsiTextFragment(string body)
        {
            Body = body;
        }

        public string Body { get; }
    }
}