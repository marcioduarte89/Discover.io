using Discoverio.Server.Services.RegistrationProviders;
using DiscoveryService.Services;
using Grpc.Core;
using System.Threading.Tasks;
using static DiscoveryService.Services.ApplicationRegistrationService;

namespace Discoverio.Server.Services.Registration
{
    public class RegistrationService : ApplicationRegistrationServiceBase
    {
        private readonly IRegistrationProvider _registrationProvider;

        public RegistrationService(IRegistrationProvider registrationProvider)
        {
            _registrationProvider = registrationProvider;
        }

        public override Task<RegistrationStatus> Register(ApplicationSettings request, ServerCallContext context)
        {
            var uniqueId = _registrationProvider.Register(request.AppName, request.Host);

            return Task.FromResult(new RegistrationStatus()
            {
                UniqueIdentifier = uniqueId,
                Success = true
            });
        }
    }
}
