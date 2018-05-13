using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public interface IFragmentExecutePipeline<T>
        where T : IEsiFragment
    {
        Task<IEnumerable<string>> Handle(T fragment, ExecuteDelegate<T> next);
    }
}