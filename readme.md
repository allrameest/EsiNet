<p align="center">
<img src="https://raw.githubusercontent.com/allrameest/EsiNet/master/logo.png" alt="metro logo" />
</p>

[![NuGet Version](http://img.shields.io/nuget/v/EsiNet.svg?style=flat)](https://www.nuget.org/packages/EsiNet/)
[![NuGet Version](http://img.shields.io/nuget/v/EsiNet.AspNetCore.svg?style=flat)](https://www.nuget.org/packages/EsiNet.AspNetCore/)
[![NuGet Version](http://img.shields.io/nuget/v/EsiNet.Polly.svg?style=flat)](https://www.nuget.org/packages/EsiNet.Polly/)

A asp.net core middleware for [ESI](http://www.w3.org/TR/esi-lang).

```
<esi:include src="http://localhost:57780/cart/buy.html" />
```
# Install

### NuGet

```
Install-Package EsiNet.AspNetCore
```

### Services

AddEsiNet adds all service for EsiNet.

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services
		.AddEsiNet()
		.AddMvc();
}
```

### Usage

UseEsiNet add the middleware that parses esi tags.

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

