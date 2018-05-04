using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EsiNet.Pipeline
{
    public interface IHttpLoaderPipeline
    {
        Task<HttpResponseMessage> Handle(Uri uri, HttpLoadDelegate next);
    }
}