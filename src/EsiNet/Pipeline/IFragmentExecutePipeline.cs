using System.Threading.Tasks;
using EsiNet.Fragments;

namespace EsiNet.Pipeline
{
    public interface IFragmentExecutePipeline<T>
        where T : IEsiFragment
    {
        Task<string> Handle(T fragment, ExecuteDelegate<T> next);
    }
}