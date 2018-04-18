using System;
using System.Threading.Tasks;

namespace EsiNet.Fragments
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