using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Logging;

namespace EsiNet.Fragments
{
    public class EsiTryFragmentExecutor
    {
        private readonly EsiFragmentExecutor _fragmentExecutor;
        private readonly Log _log;

        public EsiTryFragmentExecutor(
            EsiFragmentExecutor fragmentExecutor,
            Log log)
        {
            _fragmentExecutor = fragmentExecutor ?? throw new ArgumentNullException(nameof(fragmentExecutor));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<IEnumerable<string>> Execute(EsiTryFragment fragment)
        {
            if (fragment == null) throw new ArgumentNullException(nameof(fragment));

            try
            {
                return await _fragmentExecutor.Execute(fragment.AttemptFragment);
            }
            catch (Exception ex)
            {
                _log.Error(() => "Error when executing attempt fragment.", ex);
                return await _fragmentExecutor.Execute(fragment.ExceptFragment);
            }
        }
    }
}