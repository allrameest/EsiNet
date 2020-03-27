using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsiNet.Expressions;

namespace EsiNet.Fragments.Vars
{
    public class EsiVarsFragmentExecutor
    {
        public Task<IEnumerable<string>> Execute(EsiVarsFragment fragment, EsiExecutionContext executionContext)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var result = VariableStringResolver.Resolve(executionContext, fragment.Body);
            return Task.FromResult(result);
        }
    }
}