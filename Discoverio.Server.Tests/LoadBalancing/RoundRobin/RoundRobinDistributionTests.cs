using Discoverio.Server.Services.RegistrationProviders;
using Discoverio.Server.Services.RegistrationProviders.InMemoryProvider;
using Discoverio.Server.Services.RoundRobin;
using DiscoveryService.Services;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discoverio.Server.Tests.LoadBalancing.RoundRobin
{
    public class RoundRobinDistributionTests
    {
        private Mock<ILogger<RoundRobinDistribution>> _discoveryLoggerMock;
        private Mock<ILogger<InMemoryRegistrationProvider>> _inMemoryLoggerMock;
        private IConfiguration _configurationMock;
        private InMemoryRegistrationProvider _inMemoryProvider;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Discoverio.Server:DeRegisterCycleFrequency", "5"},
                {"Discoverio.Server:ElapsedTimeToDeRegister", "5"},
            };

            _configurationMock = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _inMemoryLoggerMock = new Mock<ILogger<InMemoryRegistrationProvider>>();
            _discoveryLoggerMock = new Mock<ILogger<RoundRobinDistribution>>();

            _inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, _configurationMock);
        }

        [Test]
        public void ResolveHost_NoRegistration_ThrowsRpcException()
        {
            var discovery = new RoundRobinDistribution(_inMemoryProvider, _discoveryLoggerMock.Object);
            Assert.Throws<RpcException>(() => discovery.ResolveHost(
                new UUID()
                {
                    Value = Guid.NewGuid().ToString()
                },
                "SomeAppName"));
        }

        [Test]
        public void ResolveHost_NotRegisteredInDistributor_ThrowsRpcException()
        {
            var uid = Guid.NewGuid().ToString();
            var inMemoryProvider = new Mock<IRegistrationProvider>();
            inMemoryProvider.Setup(x => x.HasRegistration(It.IsAny<UUID>())).Returns(true);

            var discovery = new RoundRobinDistribution(inMemoryProvider.Object, _discoveryLoggerMock.Object);
            Assert.Throws<RpcException>(() => discovery.ResolveHost(
                new UUID()
                {
                    Value = uid
                },
                "SomeAppName"
                )
            );
        }

        [Test]
        public void ResolveHost_RegisteredInDistributor_ShouldReturnNextHost()
        {
            var inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, _configurationMock);
            var discovery = new RoundRobinDistribution(inMemoryProvider, _discoveryLoggerMock.Object);
            var uid = inMemoryProvider.Register("ClientApp", "https://www.someclient.com");

            var host = discovery.ResolveHost(uid, "ClientApp");
            Assert.AreEqual("https://www.someclient.com", host);
        }

        //[Test]
        //public async Task ResolveHost_RegisterSameAppWithDifferentHostInDistributor_ShouldReturnNextHost()
        //{
        //    var inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, _configurationMock);
        //    var discovery = new RoundRobinDistribution(inMemoryProvider, _discoveryLoggerMock.Object);
        //    var uid = inMemoryProvider.Register("ClientApp", "https://www.someclient.com");
        //    var uid2 = inMemoryProvider.Register("ClientApp", "https://www.someclient2.com");


        //    var task1 = Task.Run(() =>
        //    {
        //        discovery.ResolveHost(uid, "ClientApp");
        //    });

        //    var task2 = Task.Run(() =>
        //    {
        //        discovery.ResolveHost(uid, "ClientApp");
        //    });


        //    Task.WaitAll(task1, task2);

        //    var host = discovery.ResolveHost(uid, "ClientApp");
        //    Assert.AreEqual("https://www.someclient.com", host);
        //}
    }
}
