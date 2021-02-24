using Discoverio.Server.Services.LoadBalancing;
using Discoverio.Server.Services.RegistrationProviders;
using Discoverio.Server.Services.RegistrationProviders.InMemoryProvider;
using Discoverio.Server.Services.RoundRobin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Discoverio.Server.Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDiscoverio(this IServiceCollection serviceCollection)
        {
            var provider = serviceCollection.BuildServiceProvider();

            var registrationProviderLogger = provider.GetService<ILogger<InMemoryRegistrationProvider>>();
            var distributionLogger = provider.GetService<ILogger<RoundRobinDistribution>>();
            var configuration = provider.GetService<IConfiguration>();

            var inMemoryRegistrationProvider = new InMemoryRegistrationProvider(registrationProviderLogger, configuration);
            var roundRobinDistribution = new RoundRobinDistribution(inMemoryRegistrationProvider, distributionLogger);

            serviceCollection.AddSingleton<IRegistrationProvider, InMemoryRegistrationProvider>(x => inMemoryRegistrationProvider);
            serviceCollection.AddSingleton<IServiceDiscoveryLoadBalancer, RoundRobinDistribution>(x => roundRobinDistribution);
        }

        // Use this extension method if one wants to override Registration providers or Distribution
        public static void AddDiscoverio(this IServiceCollection serviceCollection, Func<IServiceCollection, IServiceCollection> extendedServices)
        {
            extendedServices(serviceCollection);
        }
    }
}
