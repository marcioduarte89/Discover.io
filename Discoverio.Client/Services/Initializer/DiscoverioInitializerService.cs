using Discoverio.Client.Common;
using Discoverio.Client.Services.Registration;
using DiscoveryService.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using static DiscoveryService.Services.MonitorService;

namespace Discoverio.Client.Services.Initializer
{
    public class DiscoverioInitializerService : IInitializerService
    {
        private readonly MonitorServiceClient _monitorClient;
        private readonly IRegistrationService _registrationService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public DiscoverioInitializerService(
            MonitorServiceClient monitorclient,
            IRegistrationService registrationService,
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _monitorClient = monitorclient;
            _registrationService = registrationService;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task Start()
        {
            if(_memoryCache.TryGetValue(KnownKeys.SERVICE_DISCOVERY_DISCOVERY_KEY, out var _))
            {
                return;
            }

            var registrationStatus = await _registrationService.Register();

            new Timer(
                HearBeat,
                registrationStatus,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(_configuration.GetValue<double>("Discoverio.Client:HeartBeatFrequency"))
            );
        }

        private async void HearBeat(object state)
        {
            if (state is RegistrationStatus registrationStatus)
            {
                await _monitorClient.HeartBeatAsync(registrationStatus.UniqueIdentifier);
            }
        }
    }
}
