using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace EsiNet.AspNetCore.Internal
{
    public static class HeaderDictionaryExtensions
    {
        public static IReadOnlyDictionary<string, IReadOnlyCollection<string>> ToDictionary(
            this IHeaderDictionary headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            return headers
                .ToDictionary(
                    h => h.Key,
                    h => (IReadOnlyCollection<string>) h.Value.ToArray(),
                    StringComparer.OrdinalIgnoreCase);
        }
    }
}