using GameServer.Core.Protocol;

namespace GameServer.Core.Rooms;

public interface IGameRoomTransportServer : IAsyncDisposable
{
    DirectTransportProtocol Protocol { get; }
    string Address { get; }

    Task StartAsync();
    Task StopAsync();
}
