using GameServer.Core.Network;
using Network;

namespace TestGame.Core;

[NetworkRequest(typeof(RoomPingRsp))]
public partial struct RoomPingReq : INetworkReq
{
}

public partial struct RoomPingReq
{
}

public partial struct RoomPingRsp : INetworkRsp
{
}
