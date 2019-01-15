using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public delegate Task<IEnumerable<string>> ExecuteDelegate(
        IEsiFragment fragment, EsiExecutionContext executionContext);

    public delegate Task<IEnumerable<string>> ExecuteDelegate<in T>(T fragment, EsiExecutionContext executionContext);
}