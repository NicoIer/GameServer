using GameServer.Core.Fibers;
using GameServer.Core.Protocol;
using Google.Protobuf;
using MemoryPack;
using Network;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

namespace GameServer.Core.Rooms;

public abstract class RoomWorkerBase<TRoomModule> : IRoomWorker, IDisposable
    where TRoomModule : RoomFiberModuleBase
{
    private readonly FiberManager _fiberManager = new();
    private readonly Dictionary<string, RoomRuntimeHandle> _rooms = new(StringComparer.Ordinal);
    private readonly SemaphoreSlim _roomLock = new(1, 1);
    private bool _stopped;

    protected RoomWorkerBase(RoomConnectionRegistry connections, int roomFrameRate)
    {
        Connections = connections;
        RoomFrameRate = roomFrameRate;
    }

    protected RoomConnectionRegistry Connections { get; }
    protected int RoomFrameRate { get; }

    public Task<int> AddConnectionAsync(long uid, string roomId)
    {
        if (_stopped)
        {
            return Task.FromCanceled<int>(new CancellationToken(true));
        }

        return Task.FromResult(Connections.Add(uid, roomId));
    }

    public Task RemoveConnectionAsync(int connectionId)
    {
        Connections.Remove(connectionId);
        return Task.CompletedTask;
    }

    public async Task<RspHead> HandleRequestAsync(int connectionId, ReqHead request)
    {
        if (_stopped)
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InternalError, "room worker stopped", default);
        }

        if (!Connections.TryGet(connectionId, out RoomConnectionContext context))
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, $"missing room connection context connectionId={connectionId}", default);
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
        _stopped = true;
    }

    public void Dispose()
    {
        Stop();
        _roomLock.Dispose();
        _fiberManager.Dispose();
    }

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

        await _roomLock.WaitAsync();
        try
        {
            if (_rooms.TryGetValue(roomId, out RoomRuntimeHandle handle))
            {
                return handle;
            }
        }
        finally
        {
            _roomLock.Release();
        }

        return null;
    }

    private async Task<RoomRuntimeHandle> GetOrCreateRoomAsync(string roomId)
    {
        await _roomLock.WaitAsync();
        try
        {
            if (_rooms.TryGetValue(roomId, out RoomRuntimeHandle existing))
            {
                return existing;
            }

            TRoomModule module = CreateRoomModule(roomId);
            Fiber fiber = await _fiberManager.CreateAsync(FiberSchedulerType.ThreadPool, CreateRoomFiberName(roomId), module);
            var handle = new RoomRuntimeHandle(fiber, module);
            _rooms.Add(roomId, handle);
            return handle;
        }
        finally
        {
            _roomLock.Release();
        }
    }

    private readonly record struct RoomRuntimeHandle(Fiber Fiber, TRoomModule Module);
}
