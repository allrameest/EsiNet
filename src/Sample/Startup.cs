﻿using System;
using System.Globalization;
using EsiNet.AspNetCore;
using EsiNet.Fragments.Include;
using EsiNet.Logging;
using EsiNet.Polly;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Net.Http.Headers;

namespace Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();

            services.AddEsiNet()
                .AddFragmentExecutePipeline<EsiIncludeFragment, BracketPipeline>()
                .AddFragmentParsePipeline<IncludeUrlPipeline>()
                .AddHttpLoaderPipeline(sp => new CircuitBreakerHttpLoaderPipeline(
                    sp.GetService<Log>(), 3, TimeSpan.FromMinutes(1)))
                .AddHttpLoaderPipeline(sp => new RetryHttpLoaderPipeline(
                    sp.GetService<Log>(), 1));

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc(opt => opt.EnableEndpointRouting = false);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            app.UseStatusCodePagesWithReExecute("/ErrorPage", "?statusCode={0}");
            app.UseExceptionHandler("/ErrorPage");
            //app.UseDeveloperExceptionPage();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (!ctx.File.Name.EndsWith(".html", true, CultureInfo.InvariantCulture)) return;
                    const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        $"public,max-age={durationInSeconds}";
                }
            });

            app.UseEsiNet();
            app.UseResponseCompression();
            app.UseMvc();
        }
    }
}