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
    private readonly ConcurrentDictionary<string, byte> _closingRooms = new(StringComparer.Ordinal);
    private int _stopped;

    protected RoomWorkerBase(RoomConnectionRegistry connections, RoomPushHub pushHub, int roomFrameRate)
    {
        Connections = connections;
        PushHub = pushHub;
        RoomFrameRate = roomFrameRate;
    }

    protected RoomConnectionRegistry Connections { get; }
    public RoomPushHub PushHub { get; }
    public int RoomCount => _rooms.Count;
    public int ClosingRoomCount => _closingRooms.Count;
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

        PushHub.Unregister(connectionId);

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
        TryQueueRoomClose(context.RoomId, handle, Environment.TickCount64);
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

        RoomRequestRoute route;
        try
        {
            if (!RequestRouter.TryResolve(request, context, out route))
            {
                return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.NotSupported, "room request is not registered", default);
            }
        }
        catch
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, "invalid room request payload", default);
        }

        RoomRuntimeHandle? room = await ResolveRoomAsync(route);
        if (room == null)
        {
            return CreateRoomNotFoundResponse(request, route);
        }

        if (room.Value.Module.LifecycleState == RoomLifecycleState.Closing ||
            room.Value.Module.LifecycleState == RoomLifecycleState.Closed)
        {
            return CreateRoomNotFoundResponse(request, route);
        }

        RspHead response = await room.Value.Fiber.CallAsync(() => room.Value.Module.HandleRequestAsync(connectionId, request));
        if (response.error == NetworkErrorCode.Success && route.SuccessConnectionAction == RoomRequestConnectionAction.BindRoom)
        {
            Connections.TrySetRoom(connectionId, route.RoomId);
        }
        else if (response.error == NetworkErrorCode.Success && route.SuccessConnectionAction == RoomRequestConnectionAction.ClearRoom)
        {
            Connections.TrySetRoom(connectionId, string.Empty);
        }

        TryQueueRoomClose(route.RoomId, room.Value, Environment.TickCount64);
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
        QueueClosingRooms(timeNowMs);
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

    protected abstract RoomRequestRouter RequestRouter { get; }
    protected abstract TRoomModule CreateRoomModule(string roomId);

    protected virtual string CreateRoomFiberName(string roomId)
    {
        return $"RoomRoot.{roomId}";
    }

    private async Task<RoomRuntimeHandle?> ResolveRoomAsync(RoomRequestRoute route)
    {
        if (route.CanCreateRoom)
        {
            return await GetOrCreateRoomAsync(route.RoomId);
        }

        return await FindRoomAsync(route.RoomId);
    }

    private static RspHead CreateRoomNotFoundResponse(ReqHead request, RoomRequestRoute route)
    {
        string message = $"room not found room={route.RoomId}";
        return new RspHead(request.index, request.reqHash, 0, route.RoomNotFoundErrorCode, message, default);
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

    private void QueueClosingRooms(long timeNowMs)
    {
        foreach (KeyValuePair<string, Lazy<Task<RoomRuntimeHandle>>> item in _rooms)
        {
            Lazy<Task<RoomRuntimeHandle>> roomTask = item.Value;
            if (!roomTask.IsValueCreated || !roomTask.Value.IsCompletedSuccessfully)
            {
                continue;
            }

            RoomRuntimeHandle handle = roomTask.Value.Result;
            if (handle.Module.ShouldCloseRoom(timeNowMs))
            {
                TryQueueRoomClose(item.Key, roomTask, handle, timeNowMs);
            }
        }
    }

    private void TryQueueRoomClose(string roomId, RoomRuntimeHandle handle, long timeNowMs)
    {
        if (!_rooms.TryGetValue(roomId, out Lazy<Task<RoomRuntimeHandle>>? roomTask))
        {
            return;
        }

        if (!handle.Module.ShouldCloseRoom(timeNowMs))
        {
            return;
        }

        TryQueueRoomClose(roomId, roomTask, handle, timeNowMs);
    }

    private void TryQueueRoomClose(
        string roomId,
        Lazy<Task<RoomRuntimeHandle>> roomTask,
        RoomRuntimeHandle handle,
        long timeNowMs)
    {
        if (!_closingRooms.TryAdd(roomId, 0))
        {
            return;
        }

        _ = CloseRoomAsync(roomId, roomTask, handle, timeNowMs);
    }

    private async Task CloseRoomAsync(
        string roomId,
        Lazy<Task<RoomRuntimeHandle>> roomTask,
        RoomRuntimeHandle handle,
        long timeNowMs)
    {
        try
        {
            Exception? beginCloseException = null;
            bool shouldClose;
            try
            {
                shouldClose = await handle.Fiber.CallAsync(() =>
                {
                    if (!handle.Module.ShouldCloseRoom(timeNowMs))
                    {
                        return false;
                    }

                    handle.Module.BeginCloseRoom(timeNowMs);
                    return true;
                });
            }
            catch (Exception e)
            {
                beginCloseException = e;
                shouldClose = handle.Module.ShouldCloseRoom(timeNowMs);
            }

            if (!shouldClose)
            {
                return;
            }

            var item = new KeyValuePair<string, Lazy<Task<RoomRuntimeHandle>>>(roomId, roomTask);
            if (!((ICollection<KeyValuePair<string, Lazy<Task<RoomRuntimeHandle>>>>)_rooms).Remove(item))
            {
                return;
            }

            await _fiberManager.RemoveAsync(handle.Fiber.FiberId);
            if (beginCloseException != null)
            {
                Console.WriteLine(beginCloseException);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _closingRooms.TryRemove(roomId, out _);
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
