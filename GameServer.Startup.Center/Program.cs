using GameServer.Center;
using GameServer.Center.Login;
using GameServer.Core;
using GameServer.Core.Grpc;
using GameServer.Core.Startup;
using Microsoft.Extensions.DependencyInjection;

ServerStartupConfig config = ServerStartupConfigLoader.Load(args);

var registry = new CenterRegistry();
await using var server = new GrpcServerRuntime(config.Center.Port, services =>
{
    var loginProviders = new LoginProviderRegistry();
    loginProviders.Register(new GuestLoginProvider());

    services.AddSingleton(registry);
    services.AddSingleton(loginProviders);
    services.AddSingleton<CenterServiceImpl>();
});
server.MapGrpcService<CenterServiceImpl>();

await server.StartAsync();
Log.Info($"Center started on {config.Center.Address} port={config.Center.Port}");

using var shutdownCts = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    shutdownCts.Cancel();
};

try
{
    await Task.Delay(Timeout.InfiniteTimeSpan, shutdownCts.Token);
}
catch (OperationCanceledException)
{
}

await server.StopAsync();
