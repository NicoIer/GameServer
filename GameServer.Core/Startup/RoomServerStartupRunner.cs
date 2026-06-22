using GameServer.Core.Grpc;
using GameServer.Core.Protocol;
using GameServer.Core.Rooms;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Core.Startup;

public static class RoomServerStartupRunner
{
    public static async Task RunUntilShutdownAsync<TWorker, TGrpcService>(
        string serverName,
        string centerAddress,
        RoomServerStartupConfig config,
        Func<RoomConnectionRegistry, RoomPushHub, TWorker> createWorker)
        where TWorker : class, IRoomWorker, IDisposable
        where TGrpcService : class
    {
        var connections = new RoomConnectionRegistry();
        var pushHub = new RoomPushHub();
        using TWorker worker = createWorker(connections, pushHub);
        await using var updateRunner = new RoomWorkerUpdateRunner(worker, config.NetworkTickMs);

        await using var roomServer = new GrpcServerRuntime(config.GrpcPort, services =>
        {
            services.AddSingleton(connections);
            services.AddSingleton(pushHub);
            services.AddSingleton<IRoomWorker>(worker);
            services.AddSingleton(worker);
            services.AddSingleton<TGrpcService>();
        });
        roomServer.MapGrpcService<TGrpcService>();

        using GrpcChannel centerChannel = GrpcClientFactory.CreateChannel(centerAddress);
        var centerClient = new CenterService.CenterServiceClient(centerChannel);
        await using IGameRoomTransportServer transportServer = CreateTransportServer(config, centerClient, worker);

        await updateRunner.StartAsync();
        await roomServer.StartAsync();
        await transportServer.StartAsync();
        await RegisterRoomServerAsync(serverName, centerClient, config, transportServer);

        global::GameServer.Core.Log.Info($"{serverName} started routeId={config.RouteId} grpc={config.GrpcAddress} direct={transportServer.Address}");
        global::GameServer.Core.Log.Info($"{serverName} center={centerAddress} tickMs={config.NetworkTickMs}");

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

        await transportServer.StopAsync();
        await roomServer.StopAsync();
        await updateRunner.StopAsync();
    }

    private static async Task RegisterRoomServerAsync(
        string serverName,
        CenterService.CenterServiceClient centerClient,
        RoomServerStartupConfig config,
        IGameRoomTransportServer transportServer)
    {
        RegisterServiceReply registerReply = await centerClient.RegisterServiceAsync(new RegisterServiceRequest
        {
            Endpoint = new ServiceEndpoint
            {
                GameId = config.GameId,
                Target = config.Target,
                RouteId = config.RouteId,
                Address = config.GrpcAddress,
                DirectProtocol = transportServer.Protocol,
                DirectAddress = transportServer.Address,
            },
        });
        if (registerReply.Error != ErrorCode.Success)
        {
            throw new InvalidOperationException($"register {serverName} failed error={registerReply.Error}");
        }
    }

    private static IGameRoomTransportServer CreateTransportServer(
        RoomServerStartupConfig config,
        CenterService.CenterServiceClient centerClient,
        IRoomWorker worker)
    {
        DirectTransportProtocol protocol = ParseDirectProtocol(config.DirectProtocol);
        if (protocol == DirectTransportProtocol.Tcp)
        {
            return new UnityRoomTransportServer(
                config.DirectTcpPort,
                centerClient,
                worker,
                config.NetworkTickMs,
                config.DirectAddress);
        }

        throw new NotSupportedException($"unsupported room direct transport protocol={config.DirectProtocol}");
    }

    private static DirectTransportProtocol ParseDirectProtocol(string value)
    {
        if (string.Equals(value, "tcp", StringComparison.OrdinalIgnoreCase))
        {
            return DirectTransportProtocol.Tcp;
        }

        if (string.Equals(value, "kcp", StringComparison.OrdinalIgnoreCase))
        {
            return DirectTransportProtocol.Kcp;
        }

        return DirectTransportProtocol.Tcp;
    }
}
