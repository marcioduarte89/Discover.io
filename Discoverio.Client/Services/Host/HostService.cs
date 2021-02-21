﻿using Microsoft.Extensions.Configuration;
using System;

namespace Discoverio.Client.Services.Host
{
    public class HostService : IHostService
    {
        private readonly IConfiguration _configuration;

        public HostService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Uri GetApplicationHost()
        {
            return new Uri(_configuration.GetValue<string>("Discoverio.Client:AppHost"));
        }

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
