using Discoverio.Server.Services.RegistrationProviders.InMemoryProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;

namespace Discoverio.Server.Tests.RegistrationProviders.InMemoryProvider
{
    public class InMemoryRegistrationProviderTests
    {
        Mock<ILogger<InMemoryRegistrationProvider>> _loggerMock;
        Mock<IConfiguration> _configurationMock;

        [SetUp]
        public void Setup()
        {
            //_loggerMock = new Mock<ILogger<InMemoryRegistrationProvider>>();
            //_configurationMock = new Mock<IConfiguration>();

            //_configurationMock.Setup(x => x.GetValue("Discoverio.Server:DeRegisterCycleFrequency")).Returns(1);
            //_configurationMock.Setup(x => x.GetValue<double>("Discoverio.Server:ElapsedTimeToDeRegister")).Returns(1);
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
            var mock = new Mock<ILogger<InMemoryRegistrationProvider>>();

            var inMemoryProvider = new InMemoryRegistrationProvider(_loggerMock.Object, _configurationMock.Object);
            Assert.Throws<ArgumentNullException>(() => inMemoryProvider.Register(appName, host));
        }

        [Test]
        [TestCase("Client", "clientApp/users")]
        public void Register_GivenInvalidUri_ThrowsArgumentException(string appName, string host)
        {
            var inMemoryProvider = new InMemoryRegistrationProvider(_loggerMock.Object, _configurationMock.Object);
            Assert.Throws<ArgumentException>(() => inMemoryProvider.Register(appName, host));
        }

        [Test]
        public void Register_AlreadyExistingRegistration_ThrowsArgumentException()
        {
            var inMemoryProvider = new InMemoryRegistrationProvider(_loggerMock.Object, _configurationMock.Object);
            inMemoryProvider.Register("ClientApp", "https://www.someclient.com");

            Assert.Throws<ArgumentException>(() => inMemoryProvider.Register("ClientApp", "https://www.someclient.com"));
        }
    }
}
