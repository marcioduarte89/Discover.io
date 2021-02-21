using Discoverio.Client.Common;
using Discoverio.Client.Services.Registration;
using DiscoveryService.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DiscoverioInitializerService> _logger;
        private Timer _timer;

        public DiscoverioInitializerService(
            MonitorServiceClient monitorclient,
            IRegistrationService registrationService,
            IConfiguration configuration,
            IMemoryCache memoryCache,
            ILogger<DiscoverioInitializerService> logger)
        {
            _monitorClient = monitorclient;
            _registrationService = registrationService;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task Start()
        {
            // Might already have some registrations or a previous heartbeat is still going
            if (_memoryCache.TryGetValue(KnownKeys.SERVICE_DISCOVERY_DISCOVERY_KEY, out var _))
            {
                _memoryCache.Remove(KnownKeys.SERVICE_DISCOVERY_DISCOVERY_KEY);
            }

            //try
            //{
                var registrationStatus = await _registrationService.Register();

                _timer = new Timer(
                    HearBeat,
                    registrationStatus,
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(_configuration.GetValue<double>("Discoverio.Client:HeartBeatFrequency"))
                );

            //}catch(Exception ex)
            //{
            //    _logger.LogError("Could not register application", ex);
            //}
        }

        private async void HearBeat(object state)
        {
            if (state is RegistrationStatus registrationStatus)
            {
                var result = await _monitorClient.HeartBeatAsync(registrationStatus.UniqueIdentifier);

                if (!result.Success)
                {
                    _timer.Dispose();
                    await Start();
                }

                //int i = 1;

                //if(i == 1)
                //{
                //    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                //}

            }
        }
    }
}
