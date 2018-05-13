using System;
using System.IO;
using System.Threading.Tasks;

namespace EsiNet.Fragments
{
    public class EsiIgnoreFragmentExecutor
    {
        public Task<Func<Stream, Task>> Execute(EsiIgnoreFragment fragment)
        {
            Func<Stream, Task> nullWriter = stream => Task.CompletedTask;
            return Task.FromResult(nullWriter);
        }
    }
}