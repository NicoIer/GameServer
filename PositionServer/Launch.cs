using Network.Server;
using Network.Time;
using UnityToolkit;

namespace PositionServer;

public class Launch
{
    private readonly NetworkTimeServer _timeServer;
    private readonly NetworkServer _gameServer;
    private readonly ushort _timeServerPort;

    public Launch(ushort timeServerPort, ushort gameServerPort)
    {
        _timeServerPort = timeServerPort;
        _timeServer = new NetworkTimeServer();
        TelepathyServerSocket socket = new TelepathyServerSocket(gameServerPort);
        _gameServer = new NetworkServer(socket);
        PositionSystem positionSystem = new PositionSystem(_timeServer);
        _gameServer.AddSystem<PositionSystem>(positionSystem);
    }

    public async Task Run()
    {
        var timeServerTask = _timeServer.Start(_timeServerPort);
        var gameServer = _gameServer.Run(true);
        await Task.WhenAny(timeServerTask, gameServer);
        // 谁G了
        if (timeServerTask.IsFaulted)
        {
            throw timeServerTask.Exception!;
        }
        if(gameServer.IsFaulted)
        {
            throw gameServer.Exception!;
        }
    }
}

