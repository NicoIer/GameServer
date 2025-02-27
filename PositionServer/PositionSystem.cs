using GameCore.Position;
using MemoryPack;
using Network.Server;
using Network.Time;
using UnityToolkit;
using UnityToolkit.MathTypes;

namespace PositionServer;

public class PositionSystem : ISystem, IOnInit<NetworkServer>, IOnUpdate
{
    public uint entityCounter;
    private NetworkServer _server;
    public readonly World world;
    public readonly List<Vector3> spawnPoints;
    private readonly NetworkTimeServer _timeServer;
    private CommonCommand _disposeCommand;

    public PositionSystem(NetworkTimeServer timeServer)
    {
        this._timeServer = timeServer;
        _server = null!;
        world = new World();

        // TODO 这里是随机的出生点，需要根据地图大小来设置
        spawnPoints = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(3, 0, 0),
            new Vector3(4, 0, 0),
            new Vector3(5, 0, 0),
        };
    }

    public void OnInit(NetworkServer t)
    {
        _disposeCommand = new CommonCommand();
        _server = t;
        _server.socket.OnConnected += OnConnected;
        _server.socket.OnDisconnected += OnDisconnected;
        // _server.socket.OnDataReceived += OnDataReceived;
        _disposeCommand.Attach(_server.messageHandler.Add<CmdPositionEntity>(OnCmdPositionEntity));
    }

    private void OnCmdPositionEntity(in int connectionId, in CmdPositionEntity message)
    {
    }

    private void OnConnected(int connectId)
    {
        uint entityId = Interlocked.Increment(ref entityCounter);
        Vector3 spawnPoint = spawnPoints[(int)(entityId % spawnPoints.Count)];
        world.AddEntity(in connectId, in entityId, in spawnPoint);
        ToolkitLog.Info($"OnConnected: {connectId} {entityId} {spawnPoint}");
    }

    private void OnDisconnected(int connectId)
    {
        ToolkitLog.Info($"OnDisconnected: {connectId}");
        world.Remove(connectId);
    }

    public void Dispose()
    {
        _server.socket.OnConnected -= OnConnected;
        _server.socket.OnDisconnected -= OnDisconnected;
        // _server.socket.OnDataReceived -= OnDataReceived;
        _disposeCommand.Execute();
    }

    public void OnUpdate(float deltaTime)
    {
        world.timestamp = _timeServer.timestampMs;
        // TODO 同步给客户端 世界信息
        var snapshot = world.GetSnapshot();
        _server.SendToAll(snapshot);
    }
}