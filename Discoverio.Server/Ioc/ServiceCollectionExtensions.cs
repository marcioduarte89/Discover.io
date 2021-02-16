using Discoverio.Server.Services.LoadBalancing;
using Discoverio.Server.Services.RegistrationProviders;
using Discoverio.Server.Services.RegistrationProviders.InMemoryProvider;
using Discoverio.Server.Services.RoundRobin;
using Microsoft.Extensions.DependencyInjection;

namespace Discoverio.Server.Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDiscoverio(this IServiceCollection serviceCollection)
        {
            var inMemoryRegistrationProvider = new InMemoryRegistrationProvider();
            var roundRobinDistribution = new RoundRobinDistribution(inMemoryRegistrationProvider);

            serviceCollection.AddSingleton<IRegistrationProvider, InMemoryRegistrationProvider>(x => inMemoryRegistrationProvider);
            serviceCollection.AddSingleton<IServiceDiscoveryLoadBalancer, RoundRobinDistribution>(x => roundRobinDistribution);
        }
    }
}
