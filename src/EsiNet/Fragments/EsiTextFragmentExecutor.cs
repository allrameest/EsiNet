using System.Threading.Tasks;

namespace EsiNet.Fragments
{
    public class EsiTextFragmentExecutor
    {
        public Task<string> Execute(EsiTextFragment fragment)
        {
            return Task.FromResult(fragment.Body);
        }
    }
}