using System;

namespace Discoverio.Client.Services.Host
{
    public class HostService : IHostService
    {
        public Uri Build(Uri appHost, string resolvedHost)
        {
            return new Uri($"{resolvedHost}/{appHost.PathAndQuery.Substring(1, appHost.PathAndQuery.Length - 1)}");
        }

        public string ResolveAppName(Uri appHost)
        {
            return appHost.Host;
        }
    }
}
