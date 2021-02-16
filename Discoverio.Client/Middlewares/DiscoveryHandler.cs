using Discoverio.Client.Services.Initializer;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Discoverio.Client.Middlewares
{
    public class DiscoveryHandler
    {
        private readonly RequestDelegate _next;

        public DiscoveryHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IInitializerService initializerService)
        {
            await initializerService.Start();
            await _next(httpContext);
        }
    }
}
