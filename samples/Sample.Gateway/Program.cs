using EsiNet.AspNetCore;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddEsiNet();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddReverseProxy()
    .LoadFromMemory(
    new List<RouteConfig>() { GetRouteConfig() },
    new List<ClusterConfig>() { GetClusterConfig() });

var app = builder.Build();

app.UseEsiNet();
app.MapReverseProxy();
app.MapGet("/", () => "Hello from Gateway!");

app.Run();

static RouteConfig GetRouteConfig() => new()
{
    RouteId = "route1",
    ClusterId = "cluster1",
    Match = new() { Path = "test/{**catchall}" },
    Transforms = new List<IReadOnlyDictionary<string, string>>()
    { new Dictionary<string, string>() {
            { "PathPattern", "{**catchall}" }
        }
    }
};

static ClusterConfig GetClusterConfig() => new()
{
    ClusterId = "cluster1",
    Destinations = new Dictionary<string, DestinationConfig> {
            { "destination1", new DestinationConfig() { Address = "http://localhost:50932" }
            }
        }
};
