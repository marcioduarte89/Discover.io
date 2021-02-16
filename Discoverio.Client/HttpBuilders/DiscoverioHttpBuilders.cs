using Discoverio.Client.Delegates;
using Discoverio.Client.Exceptions;
using Discoverio.Client.Services.Host;
using Discoverio.Client.Services.Initializer;
using Discoverio.Client.Services.Registration;
using DiscoveryService.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Net;
using System.Net.Http;
using static DiscoveryService.Services.ApplicationRegistrationService;
using static DiscoveryService.Services.DiscoveryService;
using static DiscoveryService.Services.MonitorService;

namespace Discoverio.Client.HttpBuilders
{
    public static class DiscoverioHttpBuilders
    {
        private static IRegistrationService _registrationService;

        private static Func<HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> retryFunc = (request) =>
        {
            return Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.OK && StatusManager.GetStatusCode(r) == StatusCode.NotFound)
            .RetryAsync(1, async (result, retryCount, context) =>
            {
                await _registrationService.Register();
            });
        };

        public static void AddDiscoverio(this IServiceCollection services)
        {
            var serverDeclaration = new Action<IServiceProvider, GrpcClientFactoryOptions>((s, o) =>
            {
                var serverHost = s.GetService<IConfiguration>().GetValue<string>("Discoverio.Client:ServerHost");
                o.Address = new Uri(serverHost);
            });

            services.AddGrpcClient<ApplicationRegistrationServiceClient>(serverDeclaration).AddPolicyHandler(retryFunc);

            services.AddGrpcClient<DiscoveryServiceClient>(serverDeclaration).AddPolicyHandler(retryFunc);

            services.AddGrpcClient<MonitorServiceClient>(serverDeclaration);

            services.AddHttpContextAccessor();
            services.AddScoped<IInitializerService, DiscoverioInitializerService>();
            services.AddSingleton<IRegistrationService, RegistrationService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddScoped<IHostService, HostService>();
            services.AddScoped<ServiceDiscoveryHandler>();

            var serviceProvider = services.BuildServiceProvider();
            _registrationService = serviceProvider.GetService<IRegistrationService>();
        }

        public static void AddDiscovery(this IHttpClientBuilder builder)
        {
            builder.AddHttpMessageHandler<ServiceDiscoveryHandler>();
        }
    }
}
