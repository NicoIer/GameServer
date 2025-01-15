using Network.Client;
using UnityToolkit;
using UnityToolkit.MathTypes;

namespace Network.Position
{
    public partial struct PositionMessage : INetworkMessage
    {
        public uint entityId;
        public Vector3 position;
    }

    public class NetworkPositionClient
    {
        private readonly Dictionary<uint, ClientPositionComponent> _positions;
        public IReadOnlyDictionary<uint, ClientPositionComponent> positions => _positions;
        private readonly NetworkClientMessageHandler _messageHandler;
        private IClientSocket _socket;
        private ICommand removeMessageHandler;

        public NetworkPositionClient(IClientSocket socket, NetworkClientMessageHandler messageHandler)
        {
            _positions = new Dictionary<uint, ClientPositionComponent>();
            removeMessageHandler = messageHandler.Add<PositionMessage>(OnPositionMessage);
            _socket = socket;
        }

        ~NetworkPositionClient()
        {
            removeMessageHandler.Execute();
        }

        private void OnPositionMessage(PositionMessage obj)
        {
            _positions[obj.entityId].remotePosition = obj.position;
        }

        public void CmdMove(int idx, Vector3 position)
        {
        }
    }

    public sealed partial class ClientPositionComponent
    {
        public Vector3 remotePosition;
        public Vector3 position;
    }
}