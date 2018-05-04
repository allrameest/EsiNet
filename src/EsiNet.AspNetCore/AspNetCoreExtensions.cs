using System;
using System.Net.Http;
using EsiNet.Caching;
using EsiNet.Fragments;
using EsiNet.Http;
using EsiNet.Logging;
using EsiNet.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EsiNet.AspNetCore
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddEsiNet(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IEsiFragmentCache>(sp =>
                new TwoStageEsiFragmentCache(
                    sp.GetService<IMemoryCache>(),
                    sp.GetService<IDistributedCache>(),
                    Serializer.Wire().GZip()));

            services.AddSingleton(sp => CreateLog(sp.GetService<ILoggerFactory>().CreateLogger("EsiNet")));

            services.AddSingleton(sp => EsiParserFactory.Create(sp.GetService));

            services.AddSingleton(sp =>
            {
                var cache = sp.GetService<IEsiFragmentCache>();
                var parser = sp.GetService<EsiBodyParser>();
                var log = sp.GetService<Log>();
                var httpClient = new HttpClient();
                var httpLoader = new HttpLoader(httpClient, sp.GetServices<IHttpLoaderPipeline>());

                return EsiExecutorFactory.Create(cache, httpLoader, parser, log, sp.GetService);
            });

            return services;
        }

        public static IApplicationBuilder UseEsiNet(this IApplicationBuilder app)
        {
            app.UseMiddleware<EsiMiddleware>();
            return app;
        }

        private static Log CreateLog(ILogger logger)
        {
            return (esiLevel, exception, message) =>
            {
                var msLevel = MapLevel(esiLevel);
                logger.Log(msLevel, 0, (object) null, exception, (o, ex) => message());
            };
        }

        private static LogLevel MapLevel(Logging.LogLevel esiLevel)
        {
            switch (esiLevel)
            {
                case Logging.LogLevel.Debug:
                    return LogLevel.Debug;
                case Logging.LogLevel.Information:
                    return LogLevel.Information;
                case Logging.LogLevel.Warning:
                    return LogLevel.Warning;
                case Logging.LogLevel.Error:
                    return LogLevel.Error;
                default:
                    throw new NotSupportedException($"Unknown level '{esiLevel}'.");
            }
        }
    }
}