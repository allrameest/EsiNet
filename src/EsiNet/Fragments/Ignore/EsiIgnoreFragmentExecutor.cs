using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsiNet.Fragments.Ignore
{
    public class EsiIgnoreFragmentExecutor
    {
        public Task<IEnumerable<string>> Execute(EsiIgnoreFragment fragment, EsiExecutionContext executionContext)
        {
            return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());
        }
    }
}