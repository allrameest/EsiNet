using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiTextFragmentExecutor
    {
        public Task<string> Execute(EsiTextFragment fragment)
        {
            return Task.FromResult(fragment.Body);
        }
    }
}