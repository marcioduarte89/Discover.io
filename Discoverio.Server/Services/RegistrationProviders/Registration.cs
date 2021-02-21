using DiscoveryService.Services;
using System;

namespace Discoverio.Server.Services.RegistrationProviders
{
    public class Registration
    {
        public UUID UniqueIdentifier { get; private set; }
        public DateTime LastUpdated { get; private set; }
        public string AppName { get; private set; }
        public string Host { get; private set; }

        public Registration(UUID UId, string appName, string host)
        {
            AppName = appName;
            Host = host;
            LastUpdated = DateTime.Now;
            UniqueIdentifier = UId;
        }

        public void RegisterHeartBeat()
        {
            LastUpdated = DateTime.Now;
        }

        public bool HasExpired(double allowedTimeElapsed)
        {
            return (DateTime.Now - LastUpdated).TotalSeconds > allowedTimeElapsed;
        }
    }
}
