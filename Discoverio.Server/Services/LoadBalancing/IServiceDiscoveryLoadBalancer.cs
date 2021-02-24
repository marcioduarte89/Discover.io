using DiscoveryService.Services;

namespace Discoverio.Server.Services.LoadBalancing
{
    public interface IServiceDiscoveryLoadBalancer
    {
        string ResolveHost(UUID uniqueId, string appName);
    }
}
