using System;
using MemoryPack;
using Network;

namespace GameCore.Jolt
{
    [MemoryPackable]
    public partial struct ReqBodyInfo : INetworkReq
    {
        public byte worldId;
        public RspBodyInfo bodyInfo;
    }

    [MemoryPackable]
    public partial struct RspBodyInfo : INetworkRsp
    {
        public uint entityId;
        public BodyData bodyData;
    }
}