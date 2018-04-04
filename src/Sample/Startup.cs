using EsiNet.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddEsi()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseStaticFiles();
            app.UseEsi();
            app.UseMvc();
        }
    }
}
