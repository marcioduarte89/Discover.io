using Discoverio.Server.Services.RegistrationProviders.InMemoryProvider;
using DiscoveryService.Services;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Discoverio.Server.Tests.RegistrationProviders.InMemoryProvider
{
    public class InMemoryRegistrationProviderTests
    {
        private Mock<ILogger<InMemoryRegistrationProvider>> _inMemoryLoggerMock;
        private IConfiguration _configurationMock;

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
        }

        [Test]
        [TestCase("", "www.someclient.com")]
        [TestCase(" ", "www.someclient.com")]
        [TestCase(null, "www.someclient.com")]
        [TestCase("Client", "")]
        [TestCase("Client", " ")]
        [TestCase("Client", null)]
        public void Register_GivenInvalidInputData_ThrowsArgumentException(string appName, string host)
        {
            var inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, _configurationMock);
            Assert.Throws<RpcException>(() => inMemoryProvider.Register(appName, host));
        }

        [Test]
        [TestCase("Client", "clientApp/users")]
        public void Register_GivenInvalidUri_ThrowsArgumentException(string appName, string host)
        {
            var inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, _configurationMock);
            Assert.Throws<RpcException>(() => inMemoryProvider.Register(appName, host));
        }

        [Test]
        public void Register_AlreadyExistingRegistration_ReturnsAlreadyRegisteredApp()
        {
            var inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, _configurationMock);
            inMemoryProvider.Register("ClientApp", "https://www.someclient.com");

            Assert.IsNotNull(inMemoryProvider.Register("ClientApp", "https://www.someclient.com"));
        }

        [Test]
        public void RegisterHeartBeat_NoApplicationRegistered_ReturnsInvalidStatus()
        {
            var inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, _configurationMock);
            inMemoryProvider.Register("ClientApp", "https://www.someclient.com");

            Assert.IsFalse(inMemoryProvider.RegisterHeartBeat(new UUID()));
        }

        [Test]
        public void RegisterHeartBeat_ApplicationIsRegistered_ReturnsValidStatus()
        {
            var inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, _configurationMock);
            var uniqueId = inMemoryProvider.Register("ClientApp", "https://www.someclient.com");

            Assert.IsTrue(inMemoryProvider.RegisterHeartBeat(uniqueId));
        }

        [Test]
        public void HasRegistration_ApplicationIsRegistered_ReturnsTrue()
        {
            var inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, _configurationMock);
            var uniqueId = inMemoryProvider.Register("ClientApp", "https://www.someclient.com");

            Assert.IsTrue(inMemoryProvider.HasRegistration(uniqueId));
        }

        [Test]
        public void DeRegisterApplication_ApplicationIsRegisteredThenDeRegistered_ReturnsFalse()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Discoverio.Server:DeRegisterCycleFrequency", "1"},
                {"Discoverio.Server:ElapsedTimeToDeRegister", "1"},
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var inMemoryProvider = new InMemoryRegistrationProvider(_inMemoryLoggerMock.Object, configuration);
            var uniqueId = inMemoryProvider.Register("ClientApp", "https://www.someclient.com");
            Assert.IsTrue(inMemoryProvider.HasRegistration(uniqueId));

            Thread.Sleep(TimeSpan.FromSeconds(2));

            Assert.IsFalse(inMemoryProvider.HasRegistration(uniqueId));
        }
    }
}
