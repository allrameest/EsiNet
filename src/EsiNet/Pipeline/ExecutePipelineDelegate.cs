using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public delegate Task<string> ExecutePipelineDelegate(IEsiFragment fragment, ExecuteDelegate next);
}