using System.Collections.Concurrent;
using GameServer.Core.Fibers;
using GameServer.Core.Network;
using GameServer.Core.Protocol;
using Google.Protobuf;
using MemoryPack;
using Network;
using UnityToolkit;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

namespace GameServer.Core.Rooms;

public abstract class RoomWorkerBase<TRoomModule> : IRoomWorker, IDisposable
    where TRoomModule : RoomFiberModuleBase
{
    private static readonly ushort RoomConnectReqHash = TypeId<RoomConnectReq>.stableId16;
    private static readonly ushort RoomConnectRspHash = TypeId<RoomConnectRsp>.stableId16;

    private readonly FiberManager _fiberManager = new();
    private readonly ConcurrentDictionary<string, Lazy<Task<RoomRuntimeHandle>>> _rooms = new(StringComparer.Ordinal);
    private int _stopped;

    protected RoomWorkerBase(RoomConnectionRegistry connections, int roomFrameRate)
    {
        Connections = connections;
        RoomFrameRate = roomFrameRate;
    }

    protected RoomConnectionRegistry Connections { get; }
    protected int RoomFrameRate { get; }

    public Task<int> AddConnectionAsync(long uid, string roomId)
    {
        if (IsStopped)
        {
            return Task.FromCanceled<int>(new CancellationToken(true));
        }

        return Task.FromResult(Connections.Add(uid, roomId));
    }

    public async Task RemoveConnectionAsync(int connectionId)
    {
        if (!Connections.TryRemove(connectionId, out RoomConnectionContext context))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(context.RoomId))
        {
            return;
        }

        RoomRuntimeHandle? room = await FindRoomAsync(context.RoomId);
        if (room == null)
        {
            return;
        }

        RoomRuntimeHandle handle = room.Value;
        await handle.Fiber.CallAsync(() => HandleConnectionDisconnectedAsync(handle, connectionId, context));
    }

    public async Task<RspHead> HandleRequestAsync(int connectionId, ReqHead request)
    {
        if (IsStopped)
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InternalError, "room worker stopped", default);
        }

        if (!Connections.TryGet(connectionId, out RoomConnectionContext context))
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, $"missing room connection context connectionId={connectionId}", default);
        }

        if (request.reqHash == RoomConnectReqHash)
        {
            return await HandleRoomConnectAsync(connectionId, request);
        }

        string roomId;
        try
        {
            if (!TryResolveRoomId(request, context, out roomId))
            {
                return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.NotSupported, "room request is not registered", default);
            }
        }
        catch
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, "invalid room request payload", default);
        }

        RoomRuntimeHandle? room = await ResolveRoomAsync(request, roomId);
        if (room == null)
        {
            return CreateRoomNotFoundResponse(request, roomId);
        }

        RspHead response = await room.Value.Fiber.CallAsync(() => room.Value.Module.HandleRequestAsync(connectionId, request));
        if (ShouldBindConnectionRoom(request, response))
        {
            Connections.TrySetRoom(connectionId, roomId);
        }
        else if (ShouldClearConnectionRoom(request, response))
        {
            Connections.TrySetRoom(connectionId, string.Empty);
        }

        return response;
    }

    public async Task<GameResponse> HandleDataAsync(long uid, ByteString data)
    {
        int connectionId = Connections.Add(uid, string.Empty);
        try
        {
            ReqHead request = MemoryPackSerializer.Deserialize<ReqHead>(data.ToByteArray());
            RspHead response = await HandleRequestAsync(connectionId, request);
            return new GameResponse
            {
                Error = ProtocolErrorCode.Success,
                Data = ByteString.CopyFrom(MemoryPackSerializer.Serialize(response)),
            };
        }
        catch
        {
            return new GameResponse { Error = ProtocolErrorCode.InvalidRequest };
        }
        finally
        {
            Connections.Remove(connectionId);
        }
    }

    public void Update(long timeNowMs)
    {
        _fiberManager.UpdateMain(timeNowMs);
    }

    public void Stop()
    {
        Interlocked.Exchange(ref _stopped, 1);
    }

    public void Dispose()
    {
        Stop();
        _fiberManager.Dispose();
    }

    private bool IsStopped => Volatile.Read(ref _stopped) != 0;

    protected abstract bool TryResolveRoomId(ReqHead request, RoomConnectionContext context, out string roomId);
    protected abstract bool IsCreateRoomRequest(ReqHead request);
    protected abstract bool ShouldBindConnectionRoom(ReqHead request, RspHead response);
    protected abstract bool ShouldClearConnectionRoom(ReqHead request, RspHead response);
    protected abstract RspHead CreateRoomNotFoundResponse(ReqHead request, string roomId);
    protected abstract TRoomModule CreateRoomModule(string roomId);

    protected virtual string CreateRoomFiberName(string roomId)
    {
        return $"RoomRoot.{roomId}";
    }

    private async Task<RoomRuntimeHandle?> ResolveRoomAsync(ReqHead request, string roomId)
    {
        if (IsCreateRoomRequest(request))
        {
            return await GetOrCreateRoomAsync(roomId);
        }

        return await FindRoomAsync(roomId);
    }

    private async Task<RoomRuntimeHandle?> FindRoomAsync(string roomId)
    {
        if (!_rooms.TryGetValue(roomId, out Lazy<Task<RoomRuntimeHandle>>? roomTask))
        {
            return null;
        }

        try
        {
            return await roomTask.Value.ConfigureAwait(false);
        }
        catch
        {
            _rooms.TryRemove(roomId, out _);
            return null;
        }
    }

    private static async ValueTask<bool> HandleConnectionDisconnectedAsync(
        RoomRuntimeHandle handle,
        int connectionId,
        RoomConnectionContext context)
    {
        await handle.Module.HandleConnectionDisconnectedAsync(connectionId, context);
        return true;
    }

    private async Task<RoomRuntimeHandle> GetOrCreateRoomAsync(string roomId)
    {
        Lazy<Task<RoomRuntimeHandle>> roomTask = _rooms.GetOrAdd(
            roomId,
            key => new Lazy<Task<RoomRuntimeHandle>>(
                () => CreateRoomAsync(key),
                LazyThreadSafetyMode.ExecutionAndPublication));

        try
        {
            return await roomTask.Value.ConfigureAwait(false);
        }
        catch
        {
            _rooms.TryRemove(roomId, out _);
            throw;
        }
    }

    private async Task<RoomRuntimeHandle> CreateRoomAsync(string roomId)
    {
        TRoomModule module = CreateRoomModule(roomId);
        Fiber fiber = await _fiberManager.CreateAsync(FiberSchedulerType.ThreadPool, CreateRoomFiberName(roomId), module);
        return new RoomRuntimeHandle(fiber, module);
    }

    private async Task<RspHead> HandleRoomConnectAsync(int connectionId, ReqHead request)
    {
        RoomConnectReq req;
        try
        {
            req = MemoryPackSerializer.Deserialize<RoomConnectReq>(request.payload);
        }
        catch
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, "invalid room connect payload", default);
        }

        string roomId = req.RoomId;
        if (string.IsNullOrWhiteSpace(roomId))
        {
            return CreateRoomConnectErrorResponse(request, "room id is empty");
        }

        RoomRuntimeHandle? room = await FindRoomAsync(roomId);
        if (room == null)
        {
            return CreateRoomConnectErrorResponse(request, $"room not found room={roomId}");
        }

        Connections.TrySetRoom(connectionId, roomId);
        return CreateRoomConnectResponse(request, $"connected room={roomId}", roomId);
    }

    private static RspHead CreateRoomConnectResponse(ReqHead request, string message, string roomId)
    {
        var rsp = new RoomConnectRsp
        {
            RoomId = roomId,
        };
        return new RspHead(request.index, request.reqHash, RoomConnectRspHash, NetworkErrorCode.Success, message, MemoryPackSerializer.Serialize(rsp));
    }

    private static RspHead CreateRoomConnectErrorResponse(ReqHead request, string message)
    {
        return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, message, default);
    }

    private readonly record struct RoomRuntimeHandle(Fiber Fiber, TRoomModule Module);
}
