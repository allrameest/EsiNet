using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EsiNet.Fragments
{
    public class EsiTextFragmentExecutor
    {
        public Task<Func<Stream, Task>> Execute(EsiTextFragment fragment)
        {
            Func<Stream, Task> writer = async stream =>
            {
                var bytes = Encoding.UTF8.GetBytes(fragment.Body);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            };
            return Task.FromResult(writer);
        }
    }
}