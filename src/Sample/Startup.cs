using EsiNet;
using EsiNet.AspNetCore;
using EsiNet.Fragments;
using EsiNet.Pipeline;
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
                .AddSingleton<IFragmentExecutePipeline<EsiIncludeFragment>, BracketPipeline>()
                .AddEsiNet()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (!ctx.File.Name.EndsWith(".html")) return;
                    const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        $"public,max-age={durationInSeconds}";
                }
            });

            app
                .UseEsiNet()
                .UseMvc();
        }
    }
}