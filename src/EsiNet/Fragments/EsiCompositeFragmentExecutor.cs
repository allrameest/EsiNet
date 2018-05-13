using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EsiNet.Fragments
{
    public class EsiCompositeFragmentExecutor
    {
        private readonly EsiFragmentExecutor _fragmentExecutor;

        public EsiCompositeFragmentExecutor(EsiFragmentExecutor fragmentExecutor)
        {
            _fragmentExecutor = fragmentExecutor;
        }

        public async Task<Func<Stream, Task>> Execute(EsiCompositeFragment fragment)
        {
            var tasks = fragment.Fragments
                .Select(_fragmentExecutor.Execute);
            var writers = await Task.WhenAll(tasks);

            Func<Stream, Task> compositeWriter = async stream =>
            {
                foreach (var writer in writers)
                {
                    await writer(stream);
                }
            };

            return compositeWriter;
        }
    }
}