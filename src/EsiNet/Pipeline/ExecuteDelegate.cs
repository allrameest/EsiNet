using System.Threading.Tasks;

namespace EsiNet.Pipeline
{
    public delegate Task<string> ExecuteDelegate<in T>(T fragment);
}