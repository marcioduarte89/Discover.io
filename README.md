Discover.io
=======

## Overview
Simple and lightweight Service Discovery using .NET for .NET.

Discoverio uses GRPC for discovery.

### Installing Discover.io in client Applications

You should install [Discoverio.Client with NuGet](https://www.nuget.org/packages/Discoverio.Client):

    Install-Package Discoverio.Client

#### Client configuration:

In `Program` class, add:

```c#
AddDiscoveryHostedService()
```

After `ConfigureWebHostDefaults` configuration, like:

```c#
public static IHostBuilder CreateHostBuilder(string[] args) =>
   Host.CreateDefaultBuilder(args)
	.ConfigureWebHostDefaults(webBuilder =>
	{
		webBuilder.UseStartup<Startup>();
	})
	.AddDiscoveryHostedService();
```

In `Startup` class, in `ConfigureServices` method, add:

`services.AddDiscoverio();`

Example of a service registration to use Discoverio:

```c#
services.AddHttpClient<ImServiceInterface, ServiceImplementation>(client => {
	client.BaseAddress = new Uri(Configuration.GetValue<string>("ClientAppUri"));
})
.AddDiscovery();
```

`ClientAppUri` can be called anything as long as its uri has the `AppName` for the application we want to communicate to, ex:

``` javascript
"ClientAppTwoUri": "http://ClientAppTwo",
"Discoverio.Client": {
  "AppHost": "http://localhost:57286",
  "AppName": "ClientAppOne",
  "ServerHost": "https://localhost:5001",
  "HeartBeatFrequency": 15
  }
```

`AppHost` - Host being used by the client application.

`AppName` - Identifier of the application. How the Application will be known in the Discoverio server. Can scale client instances, just need to use the same AppName.

`ServerHost` - Address where the Discoverio Server is running.

`HeartBeatFrequency` - Frequency (in seconds) where the client will comunicate with the discovery server.

#### Client configuration:

When installing Discoverio, it will register the `Registration`, `Monitor` and `Discovery` services.

**Description of `Registration` and `Monitor` Services**:

The `Registration` and `Monitor` services will run on a background service, the `Registration` service will try to register the application, indefinitely, until it succeeds.

Once the application is registered it will initiate the HeartBeat via Monitoring Service. If for some reason the app registration gets recycled from the server, the Monitoring service will get notice and the registration process will restart.



#### Installing Discover.io in Server Application

You should install [Discoverio.Server with NuGet](https://www.nuget.org/packages/Discoverio.Server):

    Install-Package Discoverio.Server

#### Server configuration:

In `Startup` class:

In `ConfigureServices` method, add:

```c#
services.AddDiscoverio();
```

In `Configure` method, add: 

```c#
app.UseEndpoints(endpoints => {
	(...)
	endpoints.AddDiscoveryGrpcServices();
	(...)
}
```

In `appsettings.json` add:

``` javascript
  "Discoverio.Server": {
    "DeRegisterCycleFrequency": 30,
    "ElapsedTimeToDeRegister": 30
  }
```

`DeRegisterCycleFrequency` - Frequency used by the server to lookup client registrations

`ElapsedTimeToDeRegister` - Time (in seconds) the server will use as reference to de-register a client if a heartbeat hasn't been received.

#### Other functionality:

Discoverio Server performs load balancing out of the box.

It uses a simple Round Robin load balancing distribution.

#### Notes:

Although extandable to be, currently Discoverio.Server isn't scalable, registrations are kept in memory. But one can implement new versions of `IServiceDiscoveryLoadBalancer` and `IRegistrationProvider` and register them through `AddDiscoverio(this IServiceCollection serviceCollection, Func<IServiceCollection, IServiceCollection> extendedServices)`
