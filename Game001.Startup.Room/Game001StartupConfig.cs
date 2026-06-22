using GameServer.Core.Startup;

namespace Game001.Startup.Room;

public sealed class Game001StartupConfig
{
    public CenterStartupConfig Center { get; set; } = new();
    public Game001RoomStartupConfig Game001Room { get; set; } = new();
}

public sealed class Game001RoomStartupConfig : RoomServerStartupConfig
{
    public Game001RoomStartupConfig()
    {
        GameId = "Game001";
        Target = "room-worker";
        RouteId = "worker-001";
        GrpcPort = 5101;
        GrpcAddress = "http://127.0.0.1:5101";
        DirectProtocol = "Tcp";
        DirectTcpPort = 6101;
        DirectAddress = "127.0.0.1:6101";
        NetworkTickMs = 1;
    }

    public int FrameRate { get; set; } = 50;
}
