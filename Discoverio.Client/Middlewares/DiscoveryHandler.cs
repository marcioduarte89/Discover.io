namespace Discoverio.Client.Middlewares
{
    //public class DiscoveryHandler
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly IMemoryCache _memoryCache;

    //    public DiscoveryHandler(RequestDelegate next, IMemoryCache memoryCache)
    //    {
    //        _next = next;
    //        _memoryCache = memoryCache;
    //    }

    //    public async Task InvokeAsync(HttpContext httpContext, IInitializerService initializerService)
    //    {
    //        if(!_memoryCache.TryGetValue(KnownKeys.SERVICE_DISCOVERY_DISCOVERY_KEY, out var _)){
    //            await initializerService.Start();
    //        }

    //        await _next(httpContext);
    //    }
    //}
}
