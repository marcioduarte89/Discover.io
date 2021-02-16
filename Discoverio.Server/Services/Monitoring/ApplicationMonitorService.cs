using Discoverio.Server.Services.RegistrationProviders;
using DiscoveryService.Services;
using Grpc.Core;
using System.Threading.Tasks;
using static DiscoveryService.Services.MonitorService;

namespace Discoverio.Server.Services.Monitoring
{
    public class ApplicationMonitorService : MonitorServiceBase
    {
        private readonly IRegistrationProvider _registrationProvider;

        public ApplicationMonitorService(IRegistrationProvider registrationProvider)
        {
            _registrationProvider = registrationProvider;
        }

        public override Task<DiscoveryService.Services.Status> HeartBeat(UUID request, ServerCallContext context)
        {
            _registrationProvider.RegisterHeartBeat(request);
            return Task.FromResult(new DiscoveryService.Services.Status()
            {
                Success = true
            });
        }
    }
}
