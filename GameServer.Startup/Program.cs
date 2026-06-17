using Game001.Room;
using GameServer.Center;
using GameServer.Center.Login;
using GameServer.Core.Grpc;
using GameServer.Core.Protocol;
using GameServer.Gate;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

int centerPort = ReadPort(args, "--center-port", "CENTER_PORT", 5001);
int gatePort = ReadPort(args, "--gate-port", "GATE_PORT", 5002);
int game001RoomPort = ReadPort(args, "--game001-room-port", "GAME001_ROOM_PORT", 5101);

string centerAddress = $"http://127.0.0.1:{centerPort}";
string game001RoomAddress = $"http://127.0.0.1:{game001RoomPort}";

var centerRegistry = new CenterRegistry();

await using var centerServer = new GrpcServerRuntime(centerPort, services =>
{
    var loginProviders = new LoginProviderRegistry();
    loginProviders.Register(new GuestLoginProvider());

    services.AddSingleton(centerRegistry);
    services.AddSingleton(loginProviders);
    services.AddSingleton<CenterServiceImpl>();
});
centerServer.MapGrpcService<CenterServiceImpl>();

await using var game001RoomServer = new GrpcServerRuntime(game001RoomPort, services =>
{
    services.AddSingleton<Game001RoomState>();
    services.AddSingleton<Game001RoomServiceImpl>();
});
game001RoomServer.MapGrpcService<Game001RoomServiceImpl>();

GrpcChannel gateCenterChannel = GrpcClientFactory.CreateChannel(centerAddress);
var gateCenterClient = new CenterService.CenterServiceClient(gateCenterChannel);
var gateGameClientCache = new GameIngressClientCache();

await using var gateServer = new GrpcServerRuntime(gatePort, services =>
{
    services.AddSingleton(gateCenterClient);
    services.AddSingleton(gateGameClientCache);
    services.AddSingleton<GateServiceImpl>();
});
gateServer.MapGrpcService<GateServiceImpl>();

await centerServer.StartAsync();
await game001RoomServer.StartAsync();
await gateServer.StartAsync();

GrpcChannel startupCenterChannel = GrpcClientFactory.CreateChannel(centerAddress);
var startupCenterClient = new CenterService.CenterServiceClient(startupCenterChannel);
await startupCenterClient.RegisterServiceAsync(new RegisterServiceRequest
{
    Endpoint = new ServiceEndpoint
    {
        GameId = "Game001",
        Target = "room",
        RouteId = "room-001",
        Address = game001RoomAddress,
    },
});

Console.WriteLine($"Center started on {centerAddress}");
Console.WriteLine($"Gate started on http://127.0.0.1:{gatePort}");
Console.WriteLine($"Game001.Room started on {game001RoomAddress}");
Console.WriteLine("Registered Game001 / room / room-001");

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

await gateServer.StopAsync();
await game001RoomServer.StopAsync();
await centerServer.StopAsync();

startupCenterChannel.Dispose();
gateGameClientCache.Dispose();
gateCenterChannel.Dispose();

static int ReadPort(string[] args, string argName, string envName, int defaultPort)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (args[i] == argName)
        {
            return int.Parse(args[i + 1]);
        }
    }

    string? value = Environment.GetEnvironmentVariable(envName);
    if (!string.IsNullOrWhiteSpace(value))
    {
        return int.Parse(value);
    }

    return defaultPort;
}
