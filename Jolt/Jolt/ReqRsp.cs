using System;
using MemoryPack;
using Network;

namespace GameCore.Jolt
{
    [MemoryPackable]
    public partial struct ReqBodyInfo : INetworkReq
    {
        public uint entityId;
    }

    [MemoryPackable]
    public partial struct RspBodyInfo : INetworkRsp
    {
        public readonly uint entityId;
        public readonly BodyData bodyData;

        public RspBodyInfo(in uint entityId, in BodyData bodyData)
        {
            this.entityId = entityId;
            this.bodyData = bodyData;
        }
    }
}