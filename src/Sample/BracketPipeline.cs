using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet;
using EsiNet.Fragments;
using EsiNet.Fragments.Include;
using EsiNet.Pipeline;

namespace Sample
{
    public class BracketPipeline : IFragmentExecutePipeline<EsiIncludeFragment>
    {
        public async Task<IEnumerable<string>> Handle(
            EsiIncludeFragment fragment,
            EsiExecutionContext executionContext,
            ExecuteDelegate<EsiIncludeFragment> next)
        {
            var content = await next(fragment, executionContext);

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