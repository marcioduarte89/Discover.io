using Discoverio.Client.Common;
using Discoverio.Client.Services.Host;
using DiscoveryService.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static DiscoveryService.Services.DiscoveryService;

namespace Discoverio.Client.Delegates
{
    public class ServiceDiscoveryHandler : DelegatingHandler
    {
        private readonly DiscoveryServiceClient _discoveryServiceClient;
        private readonly IMemoryCache _cache;
        private readonly IHostService _hostService;

        public ServiceDiscoveryHandler(
            DiscoveryServiceClient discoveryServiceClient,
            IMemoryCache cache,
            IHostService hostService)
        {
            _discoveryServiceClient = discoveryServiceClient;
            _cache = cache;
            _hostService = hostService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var host = _discoveryServiceClient.Resolve(new DiscoverySettings()
            {
                AppName = _hostService.GetAppName(request.RequestUri),
                UId = _cache.Get<UUID>(KnownKeys.SERVICE_DISCOVERY_DISCOVERY_KEY)
            });

            request.RequestUri = _hostService.Build(request.RequestUri, host.Host);
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
