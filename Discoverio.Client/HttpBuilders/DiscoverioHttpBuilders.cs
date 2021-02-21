using Discoverio.Client.Delegates;
using Discoverio.Client.Middlewares;
using Discoverio.Client.Policies;
using Discoverio.Client.Services.Host;
using Discoverio.Client.Services.Initializer;
using Discoverio.Client.Services.Monitor;
using Discoverio.Client.Services.Registration;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Net.Http;
using static DiscoveryService.Services.ApplicationRegistrationService;
using static DiscoveryService.Services.DiscoveryService;
using static DiscoveryService.Services.MonitorService;

namespace Discoverio.Client.HttpBuilders
{
    public static class DiscoverioHttpBuilders
    {
        public static void AddDiscoverio(this IServiceCollection services)
        {
            var grpcServerDeclaration = new Action<IServiceProvider, GrpcClientFactoryOptions>((s, o) =>
            {
                var serverHost = s.GetService<IConfiguration>().GetValue<string>("Discoverio.Client:ServerHost");
                o.Address = new Uri(serverHost);
            });

            var policySelector = new Func<IServiceProvider, HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>>((sp, httpResponseMessage) =>
            {
                var errorPolicy = sp.GetService<IErrorPolicy>();
                return errorPolicy.RegistrationRetryPolicy(httpResponseMessage);
            });

            services.AddGrpcClient<ApplicationRegistrationServiceClient>(grpcServerDeclaration);
            services.AddGrpcClient<DiscoveryServiceClient>(grpcServerDeclaration).AddPolicyHandler(policySelector).AddInterceptor<LoggerInterceptor>();
            services.AddGrpcClient<MonitorServiceClient>(grpcServerDeclaration).AddInterceptor<LoggerInterceptor>();

            services.AddHttpContextAccessor();
            //services.AddSingleton<IInitializerService, DiscoverioInitializerService>();
            services.AddSingleton<IRegistrationService, RegistrationService>();
            services.AddSingleton<IMonitorService, MonitorService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddScoped<IHostService, HostService>();
            services.AddScoped<IErrorPolicy, ErrorPolicy>();
            services.AddScoped<ServiceDiscoveryHandler>();
            services.AddSingleton<LoggerInterceptor>();
        }

        public static void AddDiscovery(this IHttpClientBuilder builder)
        {
            builder.AddHttpMessageHandler<ServiceDiscoveryHandler>();
        }
    }
}
