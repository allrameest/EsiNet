using System.Threading.Tasks;
using EsiNet;
using EsiNet.Fragments;
using EsiNet.Pipeline;

namespace Sample
{
    public class BracketPipeline : IFragmentExecutePipeline<EsiIncludeFragment>
    {
        public async Task<string> Handle(EsiIncludeFragment fragment, ExecuteDelegate<EsiIncludeFragment> next)
        {
            var content = await next(fragment);
            return $"[{content}]";
        }
    }
}