using System;
using System.Net.Http;

namespace EsiNet.Http
{
    public delegate HttpClient HttpClientFactory(Uri uri);
}