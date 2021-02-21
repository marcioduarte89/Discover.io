using Discoverio.Client.Services.Monitor;
using Discoverio.Client.Services.Registration;
using DiscoveryService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discoverio.Client.Services
{
    public class DiscoveryHostedService : BackgroundService
    {
        private readonly IRegistrationService _registrationService;
        private readonly IMonitorService _monitorService;
        private readonly ILogger<DiscoveryHostedService> _logger;
        private readonly IConfiguration _configuration;
        private bool _hasSuccessfulRegistration;
        private bool _hasSuccessfulHeartBeat;

        public DiscoveryHostedService(
            IRegistrationService registrationService,
            IMonitorService monitorService,
            ILogger<DiscoveryHostedService> logger,
            IConfiguration configuration)
        {
            _registrationService = registrationService;
            _monitorService = monitorService;
            _logger = logger;
            _configuration = configuration;
            _hasSuccessfulRegistration = false;
            _hasSuccessfulHeartBeat = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var registrationStatus = default(RegistrationStatus);

                while (!_hasSuccessfulRegistration)
                {
                    try
                    {
                        registrationStatus = await _registrationService.Register();
                        _hasSuccessfulRegistration = registrationStatus.Success;

                        if (!_hasSuccessfulRegistration)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            continue;
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError("Error while trying to register service", ex);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        continue;
                    }
                }

                do
                {
                    _hasSuccessfulHeartBeat = await _monitorService.SendHeartBeat(registrationStatus.UniqueIdentifier);

                    // If the hearbeat failed its because the server recycled the registration pool and the app was not found, so try and register again
                    if (!_hasSuccessfulHeartBeat)
                    {
                        _hasSuccessfulRegistration = false;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(_configuration.GetValue<int>("Discoverio.Client:HeartBeatFrequency")));

                } while (_hasSuccessfulHeartBeat);
            }
        }
    }
}
