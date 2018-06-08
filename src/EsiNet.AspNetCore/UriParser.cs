using System;
using Microsoft.AspNetCore.Http;

namespace EsiNet.AspNetCore
{
    public class UriParser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UriParser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Uri Parse(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
            {
                return uri;
            }

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUri = new Uri(request.Scheme + "://" + request.Host.Value);

            return new Uri(baseUri, uri);
        }
    }
}