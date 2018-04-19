using System;
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
            _fragmentExecutor = fragmentExecutor;
            _log = log;
        }

        public async Task<string> Execute(EsiTryFragment fragment)
        {
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