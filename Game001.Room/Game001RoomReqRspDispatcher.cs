using Game001.Core.Generated;
using GameServer.Core.Protocol;
using Google.Protobuf;
using MemoryPack;
using Network;
using ErrorCode = GameServer.Core.Protocol.ErrorCode;

namespace Game001.Room;

public sealed class Game001RoomReqRspDispatcher
{
    private readonly Game001RoomConnectionRegistry _connections;
    private readonly ReqRspServerCenter _reqRspCenter = new();

    public Game001RoomReqRspDispatcher(Game001RoomConnectionRegistry connections, Game001RoomReqRspHandlers handlers)
    {
        _connections = connections;
        NetworkReqRspInitializer.RegisterAll(_reqRspCenter, handlers);
    }

    public int AddConnection(long uid, string roomId)
    {
        return _connections.Add(uid, roomId);
    }

    public void RemoveConnection(int connectionId)
    {
        _connections.Remove(connectionId);
    }

    public RspHead HandleRequest(int connectionId, ReqHead request)
    {
        try
        {
            return _reqRspCenter.HandleRequest(connectionId, request);
        }
        catch
        {
            return new RspHead(request.index, request.reqHash, 0, Network.ErrorCode.InternalError, "room request failed", default);
        }
    }

    public GameResponse HandleData(long uid, ByteString data)
    {
        int connectionId = AddConnection(uid, string.Empty);
        try
        {
            ReqHead request = MemoryPackSerializer.Deserialize<ReqHead>(data.ToByteArray());
            RspHead response = HandleRequest(connectionId, request);
            return new GameResponse
            {
                Error = ErrorCode.Success,
                Data = ByteString.CopyFrom(MemoryPackSerializer.Serialize(response)),
            };
        }
        catch
        {
            return new GameResponse { Error = ErrorCode.InvalidRequest };
        }
        finally
        {
            RemoveConnection(connectionId);
        }
    }
}
