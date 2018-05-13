using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public delegate Task<IEnumerable<string>> ExecuteDelegate(IEsiFragment fragment);

    public delegate Task<IEnumerable<string>> ExecuteDelegate<in T>(T fragment);
}