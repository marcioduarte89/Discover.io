using Discoverio.Server.Services.RegistrationProviders.Events;
using DiscoveryService.Services;
using System;
using System.Collections.Generic;

namespace Discoverio.Server.Services.RegistrationProviders
{
    public interface IRegistrationProvider
    {
        IEnumerable<Registration> Registrations { get; }

        UUID Register(string appName, string host);

        bool HasRegistration(UUID uniqueId);

        void RegisterHeartBeat(UUID uniqueId);

        event EventHandler<RegistrationCreatedEventArgs> RegistrationCreated;

        event EventHandler<RegistrationExpiredEventArgs> RegistrationExpired;
    }
}
