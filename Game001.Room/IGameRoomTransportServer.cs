using GameServer.Core.Protocol;

namespace Game001.Room;

public interface IGameRoomTransportServer : IAsyncDisposable
{
    DirectTransportProtocol Protocol { get; }
    string Address { get; }

    Task StartAsync();
    Task StopAsync();
}
