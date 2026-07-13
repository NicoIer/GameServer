using System.Collections.Concurrent;
using System.Diagnostics;
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
    private long _requestCount;
    private long _requestErrorCount;
    private long _lastRequestElapsedTicks;
    private long _maxRequestElapsedTicks;
    private long _roomCreatedCount;
    private long _roomClosedCount;
    private long _disconnectionCount;
    private long _roomConnectCount;
    private int _stopped;

    protected RoomWorkerBase(RoomConnectionRegistry connections, RoomPushHub pushHub, int roomFrameRate, string workerId)
    {
        Connections = connections;
        PushHub = pushHub;
        RoomFrameRate = roomFrameRate;
        WorkerId = workerId;
    }

    public string WorkerId { get; }
    protected RoomConnectionRegistry Connections { get; }
    public RoomPushHub PushHub { get; }
    public int RoomCount => _rooms.Count;
    public int ClosingRoomCount => _closingRooms.Count;
    public int OnlineConnectionCount => Connections.ConnectionCount;
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

        Interlocked.Increment(ref _disconnectionCount);
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

    public async Task<RspHead> HandleRequestAsync(int connectionId, ReqHead request, NetworkBuffer responsePayloadWriter)
    {
        long startTimestamp = Stopwatch.GetTimestamp();
        bool requestError = true;
        try
        {
            RspHead response = await HandleRequestCoreAsync(connectionId, request, responsePayloadWriter);
            requestError = response.error != NetworkErrorCode.Success;
            return response;
        }
        finally
        {
            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            Interlocked.Increment(ref _requestCount);
            Interlocked.Exchange(ref _lastRequestElapsedTicks, elapsed.Ticks);
            UpdateMaxRequestElapsed(elapsed.Ticks);
            if (requestError)
            {
                Interlocked.Increment(ref _requestErrorCount);
            }
        }
    }

    public void HandleCommand(int connectionId, RoomCommandHead command)
    {
        if (IsStopped)
        {
            global::GameServer.Core.Log.Warning(
                "Room",
                $"event=room_command_dropped reason=worker_stopped connectionId={connectionId} commandHash={command.CommandHash}");
            return;
        }

        if (!Connections.TryGet(connectionId, out RoomConnectionContext context))
        {
            global::GameServer.Core.Log.Warning(
                "Room",
                $"event=room_command_dropped reason=missing_connection connectionId={connectionId} commandHash={command.CommandHash}");
            return;
        }

        if (string.IsNullOrWhiteSpace(context.RoomId))
        {
            global::GameServer.Core.Log.Warning(
                "Room",
                $"event=room_command_dropped reason=room_not_bound connectionId={connectionId} commandHash={command.CommandHash}");
            return;
        }

        if (!_rooms.TryGetValue(context.RoomId, out Lazy<Task<RoomRuntimeHandle>>? roomTask) ||
            !roomTask.IsValueCreated ||
            !roomTask.Value.IsCompletedSuccessfully)
        {
            global::GameServer.Core.Log.Warning(
                "Room",
                $"event=room_command_dropped reason=room_not_found connectionId={connectionId} roomId={context.RoomId} commandHash={command.CommandHash}");
            return;
        }

        RoomRuntimeHandle room = roomTask.Value.Result;
        if (room.Module.LifecycleState == RoomLifecycleState.Closing ||
            room.Module.LifecycleState == RoomLifecycleState.Closed)
        {
            global::GameServer.Core.Log.Warning(
                "Room",
                $"event=room_command_dropped reason=room_closing connectionId={connectionId} roomId={context.RoomId} commandHash={command.CommandHash}");
            return;
        }

        string roomId = context.RoomId;
        room.Fiber.Post(() =>
        {
            if (!Connections.TryGet(connectionId, out RoomConnectionContext currentContext) ||
                !string.Equals(currentContext.RoomId, roomId, StringComparison.Ordinal))
            {
                global::GameServer.Core.Log.Warning(
                    "Room",
                    $"event=room_command_dropped reason=room_binding_changed connectionId={connectionId} roomId={roomId} commandHash={command.CommandHash}");
                return;
            }

            room.Module.HandleCommand(connectionId, command);
        });
    }

    private async Task<RspHead> HandleRequestCoreAsync(int connectionId, ReqHead request, NetworkBuffer responsePayloadWriter)
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
            return await HandleRoomConnectAsync(connectionId, request, responsePayloadWriter);
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

        if (route.Kind == RoomRequestRouteKind.Worker)
        {
            return await route.WorkerHandler!(connectionId, request, context, responsePayloadWriter);
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

        RspHead response = await room.Value.Fiber.CallAsync(() => room.Value.Module.HandleRequestAsync(connectionId, request, responsePayloadWriter));
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
        NetworkBuffer responsePayloadWriter = NetworkBufferPool.Shared.Get();
        NetworkBuffer responseWriter = NetworkBufferPool.Shared.Get();
        GameResponse result;
        try
        {
            ReqHead request = MemoryPackSerializer.Deserialize<ReqHead>(data.ToByteArray());
            RspHead response = await HandleRequestAsync(connectionId, request, responsePayloadWriter);
            responseWriter.Reset();
            MemoryPackSerializer.Serialize(responseWriter, response);
            ArraySegment<byte> responseSegment = responseWriter.ToArraySegment();
            result = new GameResponse
            {
                Error = ProtocolErrorCode.Success,
                Data = ByteString.CopyFrom(responseSegment.Array!, responseSegment.Offset, responseSegment.Count),
            };
        }
        catch
        {
            result = new GameResponse { Error = ProtocolErrorCode.InvalidRequest };
        }

        Connections.Remove(connectionId);
        NetworkBufferPool.Shared.Return(responsePayloadWriter);
        NetworkBufferPool.Shared.Return(responseWriter);
        return result;
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

    public RoomWorkerMetrics GetMetrics()
    {
        return new RoomWorkerMetrics(
            RoomCount,
            ClosingRoomCount,
            OnlineConnectionCount,
            Interlocked.Read(ref _requestCount),
            Interlocked.Read(ref _requestErrorCount),
            TimeSpan.FromTicks(Interlocked.Read(ref _lastRequestElapsedTicks)),
            TimeSpan.FromTicks(Interlocked.Read(ref _maxRequestElapsedTicks)),
            Interlocked.Read(ref _roomCreatedCount),
            Interlocked.Read(ref _roomClosedCount),
            Interlocked.Read(ref _disconnectionCount),
            Interlocked.Read(ref _roomConnectCount),
            PushHub.SentCount,
            PushHub.DroppedCount);
    }

    public List<RoomMetrics> GetRoomMetrics()
    {
        var result = new List<RoomMetrics>();
        foreach (KeyValuePair<string, Lazy<Task<RoomRuntimeHandle>>> item in _rooms)
        {
            Lazy<Task<RoomRuntimeHandle>> roomTask = item.Value;
            if (!roomTask.IsValueCreated || !roomTask.Value.IsCompletedSuccessfully)
            {
                continue;
            }

            RoomRuntimeHandle handle = roomTask.Value.Result;
            result.Add(new RoomMetrics(
                item.Key,
                handle.Module.LifecycleState,
                handle.Module.PlayerCount,
                Connections.GetRoomConnectionCount(item.Key),
                handle.Module.LastFrameElapsed,
                handle.Module.MaxFrameElapsed));
        }

        return result;
    }

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
            Interlocked.Increment(ref _roomClosedCount);
            global::GameServer.Core.Log.Info("Room", $"event=room_closed workerId={WorkerId} roomId={roomId} roomCount={RoomCount} closingRoomCount={ClosingRoomCount}");
            if (beginCloseException != null)
            {
                global::GameServer.Core.Log.Error("Room", beginCloseException, $"event=room_begin_close_failed workerId={WorkerId} roomId={roomId}");
            }
        }
        catch (Exception e)
        {
            global::GameServer.Core.Log.Error("Room", e, $"event=room_close_failed workerId={WorkerId} roomId={roomId}");
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
        Interlocked.Increment(ref _roomCreatedCount);
        global::GameServer.Core.Log.Info("Room", $"event=room_created workerId={WorkerId} roomId={roomId} roomCount={RoomCount}");
        return new RoomRuntimeHandle(fiber, module);
    }

    private void UpdateMaxRequestElapsed(long elapsedTicks)
    {
        while (true)
        {
            long current = Interlocked.Read(ref _maxRequestElapsedTicks);
            if (elapsedTicks <= current)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _maxRequestElapsedTicks, elapsedTicks, current) == current)
            {
                return;
            }
        }
    }

    private async Task<RspHead> HandleRoomConnectAsync(int connectionId, ReqHead request, NetworkBuffer responsePayloadWriter)
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
        Interlocked.Increment(ref _roomConnectCount);
        return CreateRoomConnectResponse(request, $"connected room={roomId}", roomId, responsePayloadWriter);
    }

    private static RspHead CreateRoomConnectResponse(ReqHead request, string message, string roomId, NetworkBuffer responsePayloadWriter)
    {
        var rsp = new RoomConnectRsp
        {
            RoomId = roomId,
        };
        responsePayloadWriter.Reset();
        MemoryPackSerializer.Serialize(responsePayloadWriter, rsp);
        return new RspHead(request.index, request.reqHash, RoomConnectRspHash, NetworkErrorCode.Success, message, responsePayloadWriter.ToArraySegment());
    }

    private static RspHead CreateRoomConnectErrorResponse(ReqHead request, string message)
    {
        return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, message, default);
    }

    private readonly record struct RoomRuntimeHandle(Fiber Fiber, TRoomModule Module);
}
