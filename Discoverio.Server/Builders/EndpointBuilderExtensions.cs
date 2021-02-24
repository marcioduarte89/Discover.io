using Discoverio.Server.Services.Discovery;
using Discoverio.Server.Services.Monitoring;
using Discoverio.Server.Services.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Discoverio.Server.Builders
{
    public static class EndpointBuilderExtensions
    {
        public static void AddDiscoveryGrpcServices(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<RegistrationService>();
            endpoints.MapGrpcService<ApplicationMonitorService>();
            endpoints.MapGrpcService<ApplicationDiscoveryService>();
        }
    }
}
