using Microsoft.Extensions.Caching.Memory;
using System;

namespace Discoverio.Client.Services.Discovery
{
    public class ServiceDiscovery : IServiceDiscovery
    {
        private readonly IMemoryCache _cache;

        public ServiceDiscovery(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string Resolve(string appName)
        {
            throw new NotImplementedException();
        }
    }
}
