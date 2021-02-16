using Discoverio.Server.Services.LoadBalancing;
using DiscoveryService.Services;
using Grpc.Core;
using System.Threading.Tasks;
using static DiscoveryService.Services.DiscoveryService;

namespace Discoverio.Server.Services.Discovery
{
    public class ApplicationDiscoveryService : DiscoveryServiceBase
    {
        private readonly IServiceDiscoveryLoadBalancer _serviceDiscoveryLoadBalancer;

        public ApplicationDiscoveryService(IServiceDiscoveryLoadBalancer serviceDiscoveryLoadBalancer)
        {
            _serviceDiscoveryLoadBalancer = serviceDiscoveryLoadBalancer;
        }
        public override Task<DiscoveryResponse> Resolve(DiscoverySettings request, ServerCallContext context)
        {
            var host = _serviceDiscoveryLoadBalancer.ResolveHost(request.UId, request.AppName);

            return Task.FromResult(new DiscoveryResponse()
            {
                Host = host
            });
        }
    }
}
