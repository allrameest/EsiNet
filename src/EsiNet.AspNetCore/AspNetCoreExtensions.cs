using System;
using System.Net.Http;
using EsiNet.Caching;
using EsiNet.Fragments;
using EsiNet.Http;
using EsiNet.Logging;
using EsiNet.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EsiNet.AspNetCore
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddEsiNet(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IEsiFragmentCache>(sp =>
                new TwoStageEsiFragmentCache(
                    sp.GetRequiredService<IMemoryCache>(),
                    sp.GetRequiredService<IDistributedCache>(),
                    Serializer.Wire().GZip()));

            services.TryAddSingleton(sp => CreateLog(sp.GetRequiredService<ILoggerFactory>().CreateLogger("EsiNet")));

            services.TryAddSingleton<IncludeUriParser>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var uriParser = new UriParser(httpContextAccessor);
                return uriParser.Parse;
            });

            services.TryAddSingleton(sp => EsiParserFactory.Create(
                sp.GetServices<IFragmentParsePipeline>(), sp.GetRequiredService<IncludeUriParser>()));

            services.TryAddSingleton<HttpClientFactory>(sp =>
            {
                var httpClient = new HttpClient();
                return uri => httpClient;
            });

            services.TryAddSingleton<IHttpLoader, HttpLoader>();

            services.TryAddSingleton(sp =>
            {
                var cache = sp.GetRequiredService<IEsiFragmentCache>();
                var httpLoader = sp.GetRequiredService<IHttpLoader>();
                var parser = sp.GetRequiredService<EsiBodyParser>();
                var log = sp.GetRequiredService<Log>();

                return EsiExecutorFactory.Create(cache, httpLoader, parser, log, sp.GetService);
            });

            return services;
        }

        public static IApplicationBuilder UseEsiNet(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

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