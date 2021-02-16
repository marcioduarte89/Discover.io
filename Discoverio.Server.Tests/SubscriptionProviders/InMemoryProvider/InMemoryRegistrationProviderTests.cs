using Discoverio.Server.Services.RegistrationProviders.InMemoryProvider;
using NUnit.Framework;
using System;

namespace Discoverio.Server.Tests.RegistrationProviders.InMemoryProvider
{
    public class InMemoryRegistrationProviderTests
    {
        [Test]
        [TestCase("","www.someclient.com")]
        [TestCase(" ", "www.someclient.com")]
        [TestCase(null, "www.someclient.com")]
        [TestCase("Client", "")]
        [TestCase("Client", " ")]
        [TestCase("Client", null)]
        public void Register_GivenInvalidInputData_ThrowsArgumentException(string appName, string host)
        {
            var inMemoryProvider = new InMemoryRegistrationProvider();
            Assert.Throws<ArgumentNullException>(() => inMemoryProvider.Register(appName, host));
        }

        [Test]
        [TestCase("Client", "clientApp/users")]
        public void Register_GivenInvalidUri_ThrowsArgumentException(string appName, string host)
        {
            var inMemoryProvider = new InMemoryRegistrationProvider();
            Assert.Throws<ArgumentException>(() => inMemoryProvider.Register(appName, host));
        }

        [Test]
        public void Register_AlreadyExistingRegistration_ThrowsArgumentException()
        {
            var inMemoryProvider = new InMemoryRegistrationProvider();
            inMemoryProvider.Register("ClientApp", "https://www.someclient.com");

            Assert.Throws<ArgumentException>(() => inMemoryProvider.Register("ClientApp", "https://www.someclient.com"));
        }


        //[Test]
        //[TestCase("", "www.someclient.com")]
        //public void Register_GivenInvalidInputData_ThrowsArgumentException(string appName, string host)
        //{
        //    var inMemoryProvider = new InMemoryRegistrationProvider();
        //    Assert.Throws<ArgumentNullException>(() => inMemoryProvider.Register(appName, host));
        //}
    }
}
