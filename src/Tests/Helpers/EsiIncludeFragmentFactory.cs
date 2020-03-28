using EsiNet.Expressions;
using EsiNet.Fragments.Include;

namespace Tests.Helpers
{
    public static class EsiIncludeFragmentFactory
    {
        public static EsiIncludeFragment Create(string url) =>
            new EsiIncludeFragment(new VariableString(new[] {url}));
    }
}