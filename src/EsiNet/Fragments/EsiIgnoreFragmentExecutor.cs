using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsiNet.Fragments
{
    public class EsiIgnoreFragmentExecutor
    {
        public Task<IEnumerable<string>> Execute(EsiIgnoreFragment fragment)
        {
            return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());
        }
    }
}