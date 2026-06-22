using GameServer.Core;
using GameServer.Core.Grpc;
using GameServer.Core.Protocol;
using GameServer.Core.Startup;
using GameServer.Gate;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

ServerStartupConfig config = ServerStartupConfigLoader.Load(args);

using GrpcChannel centerChannel = GrpcClientFactory.CreateChannel(config.Center.Address);
var centerClient = new CenterService.CenterServiceClient(centerChannel);
using var gameClientCache = new GameIngressClientCache();

await using var server = new GrpcServerRuntime(config.Gate.Port, services =>
{
    services.AddSingleton(centerClient);
    services.AddSingleton(gameClientCache);
    services.AddSingleton<GateServiceImpl>();
});
server.MapGrpcService<GateServiceImpl>();

await server.StartAsync();
Log.Info($"Gate started on port={config.Gate.Port} center={config.Center.Address}");

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
