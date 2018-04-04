using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiCompositeFragment : IEsiFragment
    {
        private readonly IReadOnlyCollection<IEsiFragment> _fragments;

        public EsiCompositeFragment(IReadOnlyCollection<IEsiFragment> fragments)
        {
            _fragments = fragments;
        }

        public async Task<string> Execute()
        {
            var tasks = _fragments
                .Select(f => f.Execute());
            var results = await Task.WhenAll(tasks);
            return string.Concat(results);
        }
    }
}