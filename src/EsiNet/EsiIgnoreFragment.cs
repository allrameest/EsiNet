using System;
using System.Threading.Tasks;

namespace EsiNet
{
    [Serializable]
    public class EsiIgnoreFragment : IEsiFragment
    {
        public Task<string> Execute()
        {
            return Task.FromResult(string.Empty);
        }
    }
}