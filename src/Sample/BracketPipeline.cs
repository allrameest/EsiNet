using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Fragments;
using EsiNet.Pipeline;

namespace Sample
{
    public class BracketPipeline : IFragmentExecutePipeline<EsiIncludeFragment>
    {
        public async Task<IEnumerable<string>> Handle(
            EsiIncludeFragment fragment,
            ExecuteDelegate<EsiIncludeFragment> next)
        {
            var content = await next(fragment);

            return Wrap(content);
        }

        private static IEnumerable<string> Wrap(IEnumerable<string> content)
        {
            yield return "[";
            foreach (var part in content)
            {
                yield return part;
            }
            yield return "]";
        }
    }
}