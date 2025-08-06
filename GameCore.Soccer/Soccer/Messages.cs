using System.Numerics;
using MemoryPack;
using Network;

namespace Game001
{
    [MemoryPackable]
    public partial struct WorldData : INetworkMessage
    {
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