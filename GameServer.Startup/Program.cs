using GameServer.Rpc.Server;
using GameServer.Rpc.Services;

int port = ReadPort(args);

await using var grpcServer = new GrpcServerRuntime(port);
grpcServer.Dispatcher.RegisterHandlersFromAssembly(typeof(GrpcTestPingHandler).Assembly);
grpcServer.MapGrpcService<GrpcTestService>();

await grpcServer.StartAsync();
Console.WriteLine($"GameServer gRPC started on 0.0.0.0:{port}");

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

await grpcServer.StopAsync();

static int ReadPort(string[] args)
{
    const int defaultPort = 50051;

    for (int i = 0; i < args.Length - 1; i++)
    {
        if (args[i] == "--grpc-port")
        {
            return int.Parse(args[i + 1]);
        }
    }

    string? envPort = Environment.GetEnvironmentVariable("GRPC_PORT");
    if (!string.IsNullOrWhiteSpace(envPort))
    {
        return int.Parse(envPort);
    }

    return defaultPort;
}
