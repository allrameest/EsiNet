using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiIgnoreFragmentExecutor
    {
        public Task<string> Execute(EsiIgnoreFragment fragment)
        {
            return Task.FromResult(string.Empty);
        }
    }
}