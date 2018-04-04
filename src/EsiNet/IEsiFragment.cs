using System.Threading.Tasks;

namespace EsiNet
{
    public interface IEsiFragment
    {
        Task<string> Execute();
    }
}