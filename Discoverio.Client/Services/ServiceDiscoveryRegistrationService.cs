using Discoverio.Client.Services.Registration;
using DiscoveryService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using static DiscoveryService.Services.MonitorService;

namespace Discoverio.Client.Services
{
    public class ServiceDiscoveryRegistrationService : IHostedService
    {
        private readonly MonitorServiceClient _monitorClient;
        private readonly IRegistrationService _registrationService;
        private readonly IConfiguration _configuration;

        public ServiceDiscoveryRegistrationService(
            MonitorServiceClient monitorclient,
            IRegistrationService registrationService,
            IConfiguration configuration)
        {
            _monitorClient = monitorclient;
            _registrationService = registrationService;
            _configuration = configuration;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var registrationStatus = await _registrationService.Register();

            new Timer(
                HearBeat,
                registrationStatus,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(_configuration.GetValue<double>("Discoverio.Client:HeartBeatFrequency") )
            );
        }

        private async void HearBeat(object state)
        {
            if(state is RegistrationStatus registrationStatus)
            {
                await _monitorClient.HeartBeatAsync(registrationStatus.UniqueIdentifier);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
