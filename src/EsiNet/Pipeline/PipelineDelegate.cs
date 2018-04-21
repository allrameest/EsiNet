using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public delegate Task<string> PipelineDelegate(IEsiFragment fragment, ExecuteDelegate next);
}