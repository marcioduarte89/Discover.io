using Discoverio.Client.Services.Host;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Discoverio.Client.Tests.Services.Host
{
    public class HostServiceTests
    {
        private IConfiguration _configuration;
        private HostService _hostService;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Discoverio.Client:AppHost", "https://localhost:44385"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _hostService = new HostService(_configuration);

        }

        [Test]
        public void Build_WhenResolvingAppNameFromHost_ShouldReturnUpdatedHost()
        {
            var updatedHost = _hostService.Build(new Uri("http://ClientApp/api/resource"), "https://localhost:44385");

            Assert.AreEqual("https://localhost:44385/api/resource", updatedHost.AbsoluteUri);
        }

        [Test]
        public void ResolveAppName_WhenResolvingAppNameFromHost_ShouldReturnUpdatedHost()
        {
            var updatedHost = _hostService.Build(new Uri("http://ClientApp/api/resource"), "https://localhost:44385");

            Assert.AreEqual("https://localhost:44385/api/resource", updatedHost.AbsoluteUri);
        }
    }
}
