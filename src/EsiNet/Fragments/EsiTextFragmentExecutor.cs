using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsiNet.Fragments
{
    public class EsiTextFragmentExecutor
    {
        public Task<IEnumerable<string>> Execute(EsiTextFragment fragment)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));

            return Task.FromResult<IEnumerable<string>>(new[] {fragment.Body});
        }
    }
}