# Esi.NET

A asp.net core middleware for [ESI](http://www.w3.org/TR/esi-lang).

```
<esi:include src="http://localhost:57780/cart/buy.html" />
```
# Install


### Services

AddEsiNet adds all service for EsiNet.

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddEsiNet()
                .AddMvc();
        }

### Usage

UseEsiNet add the middleware that parses esi tags.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app
                .UseEsiNet()
                .UseMvc();
        }

# ESI Support

## Tags

* Include
* Try
* Comment
* Remove

