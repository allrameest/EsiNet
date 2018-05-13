using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsiNet.Fragments
{
    public class EsiTextFragmentExecutor
    {
        public Task<IEnumerable<string>> Execute(EsiTextFragment fragment)
        {
            return Task.FromResult<IEnumerable<string>>(new[] {fragment.Body});
        }
    }
}