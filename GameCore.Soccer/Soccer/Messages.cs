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
        public int port;
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
        public PlayerData redPlayer;
        public PlayerData bluePlayer;
        public SoccerData soccer;
    }

    [MemoryPackable]
    public partial struct PlayerData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
    }

    [MemoryPackable]
    public partial struct SoccerData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
    }
}