using Grpc.Core;
using NUnit.Framework;
using Discoverio.Server.Services.LoadBalancing.RoundRobin;

namespace Discoverio.Server.Tests.LoadBalancing.RoundRobin
{
    public class HostDistributorTests
    {
        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void AddHost_GivenInvalidHostName_ThrowsRpcException(string host)
        {
            Assert.Throws<RpcException>(() => new HostDistributor().AddHost(host));
        }

        [Test]
        public void AddHost_GivenExistingHostName_ThrowsRpcException()
        {
            var hostDistributor = new HostDistributor();
            hostDistributor.AddHost("https://www.somehost.com");

            Assert.Throws<RpcException>(() => hostDistributor.AddHost("https://www.somehost.com"));
        }

        [Test]
        public void HasHostName_GivenHasHostName_ReturnsTrue()
        {
            var hostDistributor = new HostDistributor();
            hostDistributor.AddHost("https://www.somehost.com");

            Assert.IsTrue(hostDistributor.HasHost("https://www.somehost.com"));
        }

        [Test]
        public void HasHostName_GivenDoesNotHaveHostName_ReturnsFalse()
        {
            var hostDistributor = new HostDistributor();
            hostDistributor.AddHost("https://www.somehost.com");

            Assert.IsFalse(hostDistributor.HasHost("https://www.somehost2.com"));
        }


        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void RemoveHost_InvalidHostName_ThrowsRpcException(string host)
        {
            var hostDistributor = new HostDistributor();
            Assert.Throws<RpcException>(() => hostDistributor.RemoveHost(host));
        }

        [Test]
        public void RemoveHost_GivenDoesNotHaveHostName_ThrowsRpcException()
        {
            var hostDistributor = new HostDistributor();
            hostDistributor.AddHost("https://www.somehost.com");

            Assert.Throws<RpcException>(() => hostDistributor.RemoveHost("https://www.OtherHost.com"));
        }

        [Test]
        public void RemoveHost_GivenHasHostName_RemovesHostName()
        {
            var hostDistributor = new HostDistributor();
            hostDistributor.AddHost("https://www.somehost.com");

            hostDistributor.RemoveHost("https://www.somehost.com");
            Assert.IsFalse(hostDistributor.HasHost("https://www.somehost.com"));
        }

        [Test]
        public void HasHosts_GivenHasHosts_ReturnsTrue()
        {
            var hostDistributor = new HostDistributor();
            hostDistributor.AddHost("https://www.somehost.com");
            Assert.IsTrue(hostDistributor.HasHosts());
        }

        [Test]
        public void HasHosts_GivenDoesNotHasHosts_ReturnsFalse()
        {
            var hostDistributor = new HostDistributor();
            Assert.IsFalse(hostDistributor.HasHosts());
        }

        [Test]
        public void NextHost_OnlyOneHostQueriedMultipleTimes_ReturnsSameHostName()
        {
            var hostDistributor = new HostDistributor();
            hostDistributor.AddHost("https://www.somehost.com");
            Assert.AreEqual("https://www.somehost.com", hostDistributor.NextHost());
            Assert.AreEqual("https://www.somehost.com", hostDistributor.NextHost());
        }

        [Test]
        public void NextHost_MultipleHostsEQueriedMultipleTimes_ReturnsSameHostName()
        {
            var hostDistributor = new HostDistributor();
            hostDistributor.AddHost("https://www.somehost.com");
            hostDistributor.AddHost("https://www.somehost2.com");
            hostDistributor.AddHost("https://www.somehost3.com");
            Assert.AreEqual("https://www.somehost.com", hostDistributor.NextHost());
            Assert.AreEqual("https://www.somehost2.com", hostDistributor.NextHost());
            Assert.AreEqual("https://www.somehost3.com", hostDistributor.NextHost());
            Assert.AreEqual("https://www.somehost.com", hostDistributor.NextHost());
            Assert.AreEqual("https://www.somehost2.com", hostDistributor.NextHost());
            Assert.AreEqual("https://www.somehost3.com", hostDistributor.NextHost());
        }
    }
}
