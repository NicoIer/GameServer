using Network.Server;
using UnityToolkit.MathTypes;

namespace Network.Position
{
    public class NetworkPositionServer
    {
        public List<ServerPositionComponent> positionComponents;
        
        public NetworkPositionServer(IServerSocket socket)
        {
            positionComponents = new List<ServerPositionComponent>();
        }
    }

    public partial class ServerPositionComponent
    {
        public uint entityId;
        public Vector3 position;
    }
}