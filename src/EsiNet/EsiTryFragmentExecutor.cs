using System.Threading.Tasks;

namespace EsiNet
{
    public class EsiTryFragmentExecutor
    {
        private readonly EsiFragmentExecutor _fragmentExecutor;

        public EsiTryFragmentExecutor(
            EsiFragmentExecutor fragmentExecutor)
        {
            _fragmentExecutor = fragmentExecutor;
        }

        public async Task<string> Execute(EsiTryFragment fragment)
        {
            try
            {
                return await _fragmentExecutor.Execute(fragment.AttemptFragment);
            }
            catch
            {
                return await _fragmentExecutor.Execute(fragment.ExceptFragment);
            }
        }
    }
}