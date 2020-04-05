using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsiNet.Fragments.Text
{
    public class EsiTextFragmentExecutor
    {
        public Task<IEnumerable<string>> Execute(EsiTextFragment fragment, EsiExecutionContext executionContext)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));

            return Task.FromResult<IEnumerable<string>>(new[] {fragment.Body});
        }
    }
}