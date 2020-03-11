using System;
using System.Collections.Generic;
using System.Linq;
using EsiNet.Fragments.Choose;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace EsiNet.AspNetCore.Internal
{
    public static class HttpRequestExtensions
    {
        public static Uri GetPageUri(this HttpRequest request)
        {
            var host = request.Host.Value ?? "unknown-host";
            return new Uri(
                request.Scheme + "://" + host + request.PathBase.Value + request.Path.Value + request.QueryString.Value);
        }

        public static IReadOnlyDictionary<string, IVariableValueResolver> GetVariablesFromContext(
            this HttpRequest request)
        {
            return new Dictionary<string, IVariableValueResolver>
            {
                ["HTTP_HOST"] = new SimpleVariableValueResolver(
                    new Lazy<string>(
                        () => request.Host.Host)),
                ["HTTP_REFERER"] = new SimpleVariableValueResolver(
                    new Lazy<string>(
                        request.GetReferer)),
                ["QUERY_STRING"] = new DictionaryVariableValueResolver(
                    new Lazy<IReadOnlyDictionary<string, string>>(
                        () => request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()))),
                ["HTTP_COOKIE"] = new DictionaryVariableValueResolver(
                    new Lazy<IReadOnlyDictionary<string, string>>(
                        () => request.Cookies.ToDictionary(x => x.Key, x => x.Value)))
            };
        }

        private static string GetReferer(this HttpRequest request) =>
            request.Headers.TryGetValue(HeaderNames.Referer, out var refererValues)
                ? refererValues.ToString()
                : null;
    }
}