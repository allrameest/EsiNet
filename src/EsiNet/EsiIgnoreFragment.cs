using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiIgnoreFragment : IEsiFragment
    {
        public Task<string> Execute()
        {
            return Task.FromResult(string.Empty);
        }
    }
}