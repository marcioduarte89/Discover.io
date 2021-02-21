using Discoverio.Client.Common;
using DiscoveryService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using static DiscoveryService.Services.ApplicationRegistrationService;

namespace Discoverio.Client.Services.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ApplicationRegistrationServiceClient _registrationServiceClient;
        private readonly IConfiguration _configuration;

        public RegistrationService(
            IMemoryCache memoryCache,
            ApplicationRegistrationServiceClient registrationServiceClient,
            IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _registrationServiceClient = registrationServiceClient;
            _configuration = configuration;
        }
        public async Task<RegistrationStatus> Register()
        {
            var registrationStatus = await _registrationServiceClient.RegisterAsync(new ApplicationSettings()
            {
                AppName = _configuration.GetValue<string>("Discoverio.Client:AppName"),
                Host = _configuration.GetValue<string>("Discoverio.Client:AppHost")
            });

            _memoryCache.Set(KnownKeys.SERVICE_DISCOVERY_DISCOVERY_KEY, registrationStatus.UniqueIdentifier);

            return registrationStatus;
        }
    }
}
