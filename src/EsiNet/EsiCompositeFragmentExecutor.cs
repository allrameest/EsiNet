using System.Linq;
using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiCompositeFragmentExecutor
    {
        private readonly EsiFragmentExecutor _fragmentExecutor;

        public EsiCompositeFragmentExecutor(EsiFragmentExecutor fragmentExecutor)
        {
            _fragmentExecutor = fragmentExecutor;
        }

        public async Task<string> Execute(EsiCompositeFragment fragment)
        {
            var tasks = fragment.Fragments
                .Select(_fragmentExecutor.Execute);
            var results = await Task.WhenAll(tasks);
            return string.Concat(results);
        }
    }
}