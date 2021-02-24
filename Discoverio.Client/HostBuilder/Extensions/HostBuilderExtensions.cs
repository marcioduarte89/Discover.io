using Discoverio.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Discoverio.Client.HostBuilder.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddDiscoveryHostedService(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddHostedService<DiscoveryHostedService>();
            });

            return hostBuilder;
        }
    }
}
