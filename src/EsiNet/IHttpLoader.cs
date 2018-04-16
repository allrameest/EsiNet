using System.Net.Http;
using System.Threading.Tasks;

namespace EsiNet
{
    public interface IHttpLoader
    {
        Task<HttpResponseMessage> Get(string url);
    }
}