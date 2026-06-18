using GameServer.Core.Network;
using Network;

namespace TestGame.Core;

[NetworkRequest(typeof(CreateRoomRsp))]
public partial struct CreateRoomReq
{
}

public partial struct CreateRoomRsp : INetworkRsp
{
}
