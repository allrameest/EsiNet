using System.Net.Http;
using System.Threading.Tasks;

namespace EsiNet.Pipeline
{
    public delegate Task<HttpResponseMessage> HttpLoadDelegate(string url);
}