using Discoverio.Server.Services.RegistrationProviders;
using DiscoveryService.Services;
using Grpc.Core;
using NUnit.Framework;
using System;
using System.Threading;

namespace Discoverio.Server.Tests.RegistrationProviders
{
    public class RegistrationTests
    {
        Registration _registration;

        [SetUp]
        public void Setup() {
            _registration = new Registration(new UUID()
            {
                Value = Guid.NewGuid().ToString()
            },
            "AppName",
            "Host");
        }

        [Test]
        public void Registration_GivenNullUId_ThrowsRpcException()
        {
            Assert.Throws<RpcException>(() => new Registration(null, "AppName", "Host"));
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Registration_GivenInvalidAppName_ThrowsRpcException(string appName)
        {
            Assert.Throws<RpcException>(() => new Registration(new UUID()
            {
                Value = Guid.NewGuid().ToString()
            }, 
            appName, 
            "Host"));
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Registration_GivenInvalidHost_ThrowsRpcException(string host)
        {
            Assert.Throws<RpcException>(() => new Registration(new UUID()
            {
                Value = Guid.NewGuid().ToString()
            }, 
            "AppName",
            host));
        }

        [Test]
        public void RegisterHeartBeat_ShouldRegisterHeartBeat()
        {
            var initialTimeUpdated = _registration.LastUpdated;
            _registration.RegisterHeartBeat();

            Assert.AreNotEqual(initialTimeUpdated, _registration.LastUpdated);
        }

        [Test]
        public void HasExpired_WhenHasNotExpired_ShouldReturnFalse()
        {
            Assert.IsFalse(_registration.HasExpired(TimeSpan.FromSeconds(3).Seconds));
        }

        [Test]
        public void HasExpired_WhenHasExpired_ShouldReturnTrue()
        {
            Thread.Sleep(3);

            Assert.IsFalse(_registration.HasExpired(TimeSpan.FromSeconds(1).Seconds));
        }
    }
}
