using System;
using System.Net.Http;
using EsiNet.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace EsiNet.AspNetCore
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddEsi(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IEsiFragmentCache>(sp =>
                new DistributedEsiFragmentCache(sp.GetService<IDistributedCache>()));
            services.AddSingleton(sp => EsiParserFactory.Create());
            services.AddSingleton(sp =>
            {
                var cache = sp.GetService<IEsiFragmentCache>();
                var parser = sp.GetService<EsiBodyParser>();
                var httpClient = new HttpClient();
                var httpLoader = new HttpLoader(httpClient);

                return EsiExecutorFactory.Create(cache, httpLoader, parser);
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