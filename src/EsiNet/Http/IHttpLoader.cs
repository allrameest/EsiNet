using System.Net.Http;
using System.Threading.Tasks;

namespace EsiNet.Http
{
    public interface IHttpLoader
    {
        Task<HttpResponseMessage> Get(string url);
    }
}