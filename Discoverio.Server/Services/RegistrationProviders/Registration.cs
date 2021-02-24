using Grpc.Core;
using DiscoveryService.Services;
using System;
using Status = Grpc.Core.Status;

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
            Validate(UId, appName, host);
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

        private void Validate(UUID UId, string appName, string host)
        {
            if(UId == null || string.IsNullOrWhiteSpace(UId.Value))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Host cannot be null or empty"));
            }

            if (string.IsNullOrWhiteSpace(appName))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppName cannot be null or empty"));
            }

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Host cannot be null or empty"));
            }
        }
    }
}
