using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public delegate Task<string> ExecuteDelegate(IEsiFragment fragment);

    public delegate Task<string> ExecuteDelegate<in T>(T fragment);
}