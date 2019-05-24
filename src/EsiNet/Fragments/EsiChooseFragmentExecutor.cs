using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsiNet.Fragments.Choose;
using EsiNet.Logging;

namespace EsiNet.Fragments
{
    public class EsiChooseFragmentExecutor
    {
        private static readonly Dictionary<string, string> Variables = new Dictionary<string, string>
        {
            ["HTTP_HOST"] = "localhost",
            ["HTTP_REFERER"] = "http://www.foo.com/"
        };

        private readonly EsiFragmentExecutor _fragmentExecutor;
        private readonly Log _log;

        public EsiChooseFragmentExecutor(
            EsiFragmentExecutor fragmentExecutor,
            Log log)
        {
            _fragmentExecutor = fragmentExecutor ?? throw new ArgumentNullException(nameof(fragmentExecutor));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<IEnumerable<string>> Execute(EsiChooseFragment fragment, EsiExecutionContext executionContext)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var fragmentToShow = GetFirstSucceedingWhen(fragment.WhenFragments, executionContext.Variables) ?? fragment.OtherwiseFragment;
            return await _fragmentExecutor.Execute(fragmentToShow, executionContext);
        }

        private static IEsiFragment GetFirstSucceedingWhen(
            IEnumerable<EsiWhenFragment> whenFragments,
            IReadOnlyDictionary<string, IVariableValueResolver> variables)
        {
            return whenFragments
                .Where(f => Evaluate(f.Expression, variables))
                .Select(f => f.InnerFragment)
                .FirstOrDefault();
        }

        private static bool Evaluate(IWhenExpression expression, IReadOnlyDictionary<string, IVariableValueResolver> variables)
        {
            return WhenEvaluator.Evaluate(expression, variables);
        }
    }
}