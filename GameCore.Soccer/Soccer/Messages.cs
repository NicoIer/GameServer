using System.Numerics;
using MemoryPack;
using Network;

namespace Soccer
{
    
    [MemoryPackable]
    public partial struct ServerInfo
    {
        public string serverName;
        public string serverAddress;
        public ushort port;
        public ushort timeServerPort;
    }
    
    public enum IdentifierEnum
    {
        RedPlayer = 0,
        BluePlayer = 1,
        SoccerBall = 2,
    }

    [MemoryPackable]
    public partial struct WorldData : INetworkMessage
    {
        public int redScore;
        public int blueScore;
        public PhysicsData redPlayer;
        public PhysicsData bluePlayer;
        public PhysicsData soccer;
    }
    
    [MemoryPackable]
    public partial struct PhysicsData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
    }
    
}