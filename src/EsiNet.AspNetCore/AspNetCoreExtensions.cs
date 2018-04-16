using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace EsiNet.AspNetCore
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddEsi(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton(sp => new EsiFragmentCache(sp.GetService<IMemoryCache>()));
            services.AddSingleton(sp =>
            {
                var cache = sp.GetService<EsiFragmentCache>();
                var httpClient = new HttpClient();
                var httpLoader = new HttpLoader(httpClient);
                return EsiParserFactory.Create(cache, httpLoader);
            });

            return services;
        }

        public static IApplicationBuilder UseEsi(this IApplicationBuilder app)
        {
            app.UseMiddleware<EsiMiddleware>();
            return app;
        }
    }
}