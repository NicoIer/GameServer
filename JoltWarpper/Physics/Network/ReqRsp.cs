using System;
using MemoryPack;
using Network;

namespace GameCore.Physics
{
    [MemoryPackable]
    public partial struct ReqBodyInfo : INetworkReq
    {
        public uint bodyId;

        public ReqBodyInfo(in uint bodyId)
        {
            this.bodyId = bodyId;
        }
    }

    [MemoryPackable]
    public partial struct RspBodyInfo : INetworkRsp
    {
        public readonly uint bodyId;
        public readonly BodyData bodyData;

        public RspBodyInfo(in uint bodyId, in BodyData bodyData)
        {
            this.bodyId = bodyId;
            this.bodyData = bodyData;
        }
    }
}