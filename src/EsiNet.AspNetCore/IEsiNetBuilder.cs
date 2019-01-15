using Microsoft.Extensions.DependencyInjection;

namespace EsiNet.AspNetCore
{
    public interface IEsiNetBuilder
    {
        IServiceCollection Services { get; }
    }
}