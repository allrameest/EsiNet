using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiTextFragment : IEsiFragment
    {
        private readonly string _body;

        public EsiTextFragment(string body)
        {
            _body = body;
        }

        public Task<string> Execute()
        {
            return Task.FromResult(_body);
        }
    }
}