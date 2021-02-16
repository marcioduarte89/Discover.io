using Discoverio.Server.Services.RegistrationProviders.Events;
using DiscoveryService.Services;
using Grpc.Core;
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

        public event EventHandler<RegistrationCreatedEventArgs> RegistrationCreated;
        public event EventHandler<RegistrationExpiredEventArgs> RegistrationExpired;

        public IEnumerable<Registration> Registrations => _registrations.Values;

        public InMemoryRegistrationProvider()
        {
            _registrations = new ConcurrentDictionary<Guid, Registration>();
            new Timer(UnRegister, null, 0, 15); //make this configurable
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

            if (_registrations.Values.Any(x => x.AppName.Equals(appName, StringComparison.OrdinalIgnoreCase) && x.Host.Equals(host, StringComparison.OrdinalIgnoreCase)))
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.AlreadyExists, $"Application name { appName } with host { host } already registered"));
            }

            var uniqueIdGuid = Guid.NewGuid();
            var uniqueId = new UUID()
            {
                Value = uniqueIdGuid.ToString()
            };

            _registrations.AddOrUpdate(uniqueIdGuid, (key) => new Registration(uniqueId, appName, host), (key, value) => new Registration(uniqueId, appName, host));

            var registration = new RegistrationCreatedEventArgs() { Registration = _registrations[uniqueIdGuid] };

            RegistrationCreated?.Invoke(this, registration);

            return uniqueId;
        }

        public void RegisterHeartBeat(UUID uniqueId)
        {
            if (!_registrations.TryGetValue(new Guid(uniqueId.Value), out var registration))
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Application with unique id { uniqueId.Value } does not exist"));
            }

            registration.RegisterHeartBeat();
        }

        private void UnRegister(object state)
        {
            if (!_registrations.Any())
            {
                return;
            }

            foreach (var reg in _registrations)
            {
                if (reg.Value.HasExpired())
                {
                    if(_registrations.TryRemove(reg.Key, out var registration))
                    {
                        RegistrationExpired?.Invoke(this, new RegistrationExpiredEventArgs() { Registration = registration });
                    }
                }
            }
        }

        public bool HasRegistration(UUID uniqueId)
        {
            return _registrations.TryGetValue(new Guid(uniqueId.Value), out var _);
        }
    }
}
