using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EsiNet.Http
{
    public interface IHttpLoader
    {
        Task<HttpResponseMessage> Get(Uri uri, EsiExecutionContext executionContext);
    }
}