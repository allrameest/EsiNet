using Microsoft.Extensions.DependencyInjection;

namespace EsiNet.AspNetCore
{
    public class EsiNetBuilder : IEsiNetBuilder
    {
        public EsiNetBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}