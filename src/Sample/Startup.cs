﻿using System;
using System.Globalization;
using EsiNet.AspNetCore;
using EsiNet.Fragments;
using EsiNet.Logging;
using EsiNet.Pipeline;
using EsiNet.Polly;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddResponseCompression()
                .AddSingleton<IFragmentExecutePipeline<EsiIncludeFragment>, BracketPipeline>()
                .AddSingleton<IFragmentParsePipeline, IncludeUrlPipeline>()
                .AddSingleton<IHttpLoaderPipeline>(sp => new CircuitBreakerHttpLoaderPipeline(
                    sp.GetService<Log>(), 3, TimeSpan.FromMinutes(1)))
                .AddSingleton<IHttpLoaderPipeline>(sp => new RetryHttpLoaderPipeline(
                    sp.GetService<Log>(), 1))
                .AddEsiNet()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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