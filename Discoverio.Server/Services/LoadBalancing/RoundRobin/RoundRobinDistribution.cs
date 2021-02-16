using Discoverio.Server.Services.LoadBalancing;
using Discoverio.Server.Services.LoadBalancing.RoundRobin;
using Discoverio.Server.Services.RegistrationProviders;
using Discoverio.Server.Services.RegistrationProviders.Events;
using DiscoveryService.Services;
using Grpc.Core;
using System;
using System.Collections.Concurrent;

namespace Discoverio.Server.Services.RoundRobin
{
    public class RoundRobinDistribution : IServiceDiscoveryLoadBalancer
    {
        private readonly IRegistrationProvider _registrationProvider;
        private ConcurrentDictionary<string, HostDistributor> _hostDistributor;

        public RoundRobinDistribution(IRegistrationProvider registrationProvider)
        {
            _hostDistributor = new ConcurrentDictionary<string, HostDistributor>(StringComparer.OrdinalIgnoreCase);
            _registrationProvider = registrationProvider;
            _registrationProvider.RegistrationCreated += RegistrationCreated;
            _registrationProvider.RegistrationExpired += RegistrationExpired;
        }

        public string ResolveHost(UUID uniqueId, string appName)
        {
            if (!_registrationProvider.HasRegistration(uniqueId))
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Registration {uniqueId.Value} does not exists"));
            }

            if (!_hostDistributor.TryGetValue(appName, out var distributor))
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, $"Application name {appName} does not exists"));
            }

            return distributor.NextHost();
        }

        private void RegistrationCreated(object sender, RegistrationCreatedEventArgs e)
        {
            if (_hostDistributor.TryGetValue(e.Registration.AppName, out var hostDistributor) && hostDistributor.HasHost(e.Registration.Host))
            {
                throw new RpcException(new Grpc.Core.Status(StatusCode.AlreadyExists, $"Application name {e.Registration.AppName} with host {e.Registration.Host} already exists"));
            }

            hostDistributor = new HostDistributor();
            hostDistributor.AddHost(e.Registration.Host);

            _hostDistributor.TryAdd(e.Registration.AppName, hostDistributor);
        }

        private void RegistrationExpired(object sender, RegistrationExpiredEventArgs e)
        {
            if (_hostDistributor.TryGetValue(e.Registration.AppName, out var hostDistributor))
            {
                hostDistributor.RemoveHost(e.Registration.Host);
            }
        }
    }
}
