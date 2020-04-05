using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsiNet.Expressions;

namespace EsiNet.Fragments.Choose
{
    public class EsiChooseFragmentExecutor
    {
        private readonly EsiFragmentExecutor _fragmentExecutor;

        public EsiChooseFragmentExecutor(EsiFragmentExecutor fragmentExecutor)
        {
            _fragmentExecutor = fragmentExecutor ?? throw new ArgumentNullException(nameof(fragmentExecutor));
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

        private static bool Evaluate(IBooleanExpression expression, IReadOnlyDictionary<string, IVariableValueResolver> variables)
        {
            return ExpressionEvaluator.Evaluate(expression, variables);
        }
    }
}