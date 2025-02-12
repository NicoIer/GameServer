using GameCore.Position;
using MemoryPack;
using Network.Server;
using Network.Time;
using UnityToolkit;
using UnityToolkit.MathTypes;

namespace ServerPosition;

public class PositionSystem : ISystem, IOnInit<NetworkServer>, IOnUpdate
{
    public uint entityCounter;
    private NetworkServer _server;
    public World world;
    public List<Vector3> spawnPoints;
    private NetworkTimeServer timeServer;

    public PositionSystem(NetworkTimeServer timeServer)
    {
        this.timeServer = timeServer;
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
        _server = t;
        t.socket.OnConnected += OnConnected;
        t.socket.OnDisconnected += OnDisconnected;
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
    }

    public void OnUpdate(float deltaTime)
    {
        world.timestamp = timeServer.timestampMs;
        // TODO 同步给客户端 世界信息
        using (var snapshot = world.GetSnapshot())
        {
            _server.SendToAll(snapshot.message);
        }
    }
}