using Discoverio.Server.Services.RegistrationProviders.Events;
using DiscoveryService.Services;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Discoverio.Server.Services.RegistrationProviders.InMemoryProvider
{
    public class InMemoryRegistrationProvider : IRegistrationProvider
    {
        private ConcurrentDictionary<Guid, Registration> _registrations;
        private readonly ILogger<InMemoryRegistrationProvider> _logger;
        private readonly IConfiguration _configuration;
        private Timer _timer;

        public event EventHandler<RegistrationCreatedEventArgs> RegistrationCreated;
        public event EventHandler<RegistrationExpiredEventArgs> RegistrationExpired;

        public IEnumerable<Registration> Registrations => _registrations.Values;

        public InMemoryRegistrationProvider(ILogger<InMemoryRegistrationProvider> logger, IConfiguration configuration)
        {
            _registrations = new ConcurrentDictionary<Guid, Registration>();
            _timer = new Timer(UnRegister, null, TimeSpan.Zero, TimeSpan.FromSeconds(configuration.GetValue<double>("Discoverio.Server:DeRegisterCycleFrequency")));
            _logger = logger;
            _configuration = configuration;
        }

        public UUID Register(string appName, string host)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.InvalidArgument, $"Application name { appName } cannot be null or empty"));
            }

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.InvalidArgument, $"Host { host } cannot be null or empty"));
            }

            if (!Uri.TryCreate(host, UriKind.Absolute, out var _))
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.InvalidArgument, $"Host { host } is not a valid host"));
            }

            var existingRegistration = _registrations.Values.FirstOrDefault(x => x.AppName.Equals(appName, StringComparison.OrdinalIgnoreCase) && x.Host.Equals(host, StringComparison.OrdinalIgnoreCase));

            if (existingRegistration != null)
            {
                return existingRegistration.UniqueIdentifier;
                //throw new RpcException(new Grpc.Core.Status(StatusCode.AlreadyExists, $"Application name { appName } with host { host } already registered"));
            }

            var uniqueIdGuid = Guid.NewGuid();
            var uniqueId = new UUID()
            {
                Value = uniqueIdGuid.ToString()
            };

            _registrations.AddOrUpdate(uniqueIdGuid, (key) => new Registration(uniqueId, appName, host), (key, value) => new Registration(uniqueId, appName, host));

            var registration = new RegistrationCreatedEventArgs() { Registration = _registrations[uniqueIdGuid] };

            RegistrationCreated?.Invoke(this, registration);

            _logger.LogInformation($"Registered application with appName {appName} and host {host}");

            return uniqueId;
        }

        public bool RegisterHeartBeat(UUID uniqueId)
        {
            if (!_registrations.TryGetValue(new Guid(uniqueId.Value), out var registration))
            {
                return false;
                //throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Application with unique id { uniqueId.Value } does not exist"));
            }

            registration.RegisterHeartBeat();

            _logger.LogInformation($"Registering heartbeat for application with appName { registration.AppName } and host { registration.Host }");
            return true;
        }

        private void UnRegister(object state)
        {
            _logger.LogInformation($"Starting UnRegistering check cycle");

            if (!_registrations.Any())
            {
                return;
            }

            foreach (var reg in _registrations)
            {
                if (reg.Value.HasExpired(_configuration.GetValue<double>("Discoverio.Server:ElapsedTimeToDeRegister")))
                {
                    if(_registrations.TryRemove(reg.Key, out var registration))
                    {
                        _logger.LogInformation($"UnRegistering application with appName { registration.AppName } and host { registration.Host }");
                        RegistrationExpired?.Invoke(this, new RegistrationExpiredEventArgs() { Registration = registration });
                    }
                }
            }
        }

        public bool HasRegistration(UUID uniqueId)
        {
            if(uniqueId == null)
            {
                return false;
            }

            return _registrations.TryGetValue(new Guid(uniqueId.Value), out var _);
        }
    }
}
