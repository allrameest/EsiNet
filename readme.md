<p align="center">
<img src="https://raw.githubusercontent.com/allrameest/EsiNet/master/logo.png" alt="EsiNet" />
</p>

[![NuGet EsiNet](http://img.shields.io/nuget/v/EsiNet.svg?style=flat-square&logo=nuget&label=NuGet+EsiNet)](https://www.nuget.org/packages/EsiNet/)

[![NuGet EsiNet.AspNetCore](http://img.shields.io/nuget/v/EsiNet.AspNetCore.svg?style=flat-square&logo=nuget&label=NuGet+EsiNet.AspNetCore)](https://www.nuget.org/packages/EsiNet.AspNetCore/)

[![NuGet EsiNet.Polly](http://img.shields.io/nuget/v/EsiNet.Polly.svg?style=flat-square&logo=nuget&label=NuGet+EsiNet.Polly)](https://www.nuget.org/packages/EsiNet.Polly/)

An ASP.NET Core middleware for [ESI](http://www.w3.org/TR/esi-lang).

```
<esi:include src="http://localhost:57780/cart/buy.html" />
```
# Install

### NuGet

```
Install-Package EsiNet.AspNetCore
```

### Services

AddEsiNet adds all services for EsiNet.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services
		.AddEsiNet()
		.AddMvc();
}
```

### Usage

UseEsiNet adds the middleware that parses esi tags.

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	app
		.UseEsiNet()
		.UseMvc();
}
```

# ESI Support

## Tags

* Include
* Try
* Comment
* Remove
