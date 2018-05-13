using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public delegate Task<IEnumerable<string>> ExecutePipelineDelegate(IEsiFragment fragment, ExecuteDelegate next);
}