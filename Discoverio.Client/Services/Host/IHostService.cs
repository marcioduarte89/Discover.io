using System;

namespace Discoverio.Client.Services.Host
{
    public interface IHostService
    {
        Uri Build(Uri appHost, string resolvedHost);

        string GetAppName(Uri appHost);
    }
}
