using Game001.Room;
using Game001.Startup.Room;
using GameServer.Core.Startup;

Game001StartupConfig config = ServerStartupConfigLoader.Load<Game001StartupConfig>(args);
Game001RoomStartupConfig roomConfig = config.Game001Room;

await RoomServerStartupRunner.RunUntilShutdownAsync<Game001RoomWorker, Game001RoomServiceImpl>(
    "Game001.Room",
    config.Center.Address,
    roomConfig,
    (connections, pushHub) => new Game001RoomWorker(connections, pushHub, roomConfig.FrameRate));
