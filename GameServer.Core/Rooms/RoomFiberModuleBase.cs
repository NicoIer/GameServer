using System.Diagnostics;
using GameServer.Core.Fibers;
using Network;

namespace GameServer.Core.Rooms;

public abstract class RoomFiberModuleBase : IFiberModule
{
    private readonly ReqRspServerCenter _reqRspCenter = new();
    private readonly int _frameIntervalMs;
    private long _nextFrameTimeMs;
    private long _lastFrameElapsedTicks;
    private long _maxFrameElapsedTicks;
    private int _frame;

    protected RoomFiberModuleBase(string roomId, int roomFrameRate)
    {
        RoomId = roomId;
        _frameIntervalMs = 1000 / roomFrameRate;
    }

    public string RoomId { get; }
    public virtual RoomLifecycleState LifecycleState => RoomLifecycleState.Active;
    public virtual int PlayerCount => 0;
    public TimeSpan LastFrameElapsed => TimeSpan.FromTicks(Interlocked.Read(ref _lastFrameElapsedTicks));
    public TimeSpan MaxFrameElapsed => TimeSpan.FromTicks(Interlocked.Read(ref _maxFrameElapsedTicks));
    protected RoomFrameAwaiter FrameAwaiter { get; } = new();

    public ValueTask OnStartAsync(Fiber fiber, CancellationToken cancellationToken)
    {
        try
        {
            OnRoomCreated();
            RegisterHandlers(_reqRspCenter);
        }
        catch (Exception e)
        {
            global::GameServer.Core.Log.Error("Room", e, $"event=room_module_create_failed roomId={RoomId}");
        }

        fiber.NextWakeTimeMs = Environment.TickCount64 + _frameIntervalMs;
        return ValueTask.CompletedTask;
    }

    public void OnUpdate(FiberUpdateContext context)
    {
        if (_nextFrameTimeMs == 0)
        {
            _nextFrameTimeMs = context.TimeNowMs + _frameIntervalMs;
            context.Fiber.NextWakeTimeMs = _nextFrameTimeMs;
            return;
        }

        while (context.TimeNowMs >= _nextFrameTimeMs)
        {
            _frame++;
            long startTimestamp = Stopwatch.GetTimestamp();
            try
            {
                OnRoomUpdate(_nextFrameTimeMs, _frame);
            }
            catch (Exception e)
            {
                global::GameServer.Core.Log.Error("Room", e, $"event=room_module_update_failed roomId={RoomId} frame={_frame}");
            }

            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            Interlocked.Exchange(ref _lastFrameElapsedTicks, elapsed.Ticks);
            UpdateMaxFrameElapsed(elapsed.Ticks);
            _nextFrameTimeMs += _frameIntervalMs;
        }

        context.Fiber.NextWakeTimeMs = _nextFrameTimeMs;
    }

    public void OnLateUpdate(FiberUpdateContext context)
    {
    }

    public ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        FrameAwaiter.Cancel();
        try
        {
            OnRoomStopped();
        }
        catch (Exception e)
        {
            global::GameServer.Core.Log.Error("Room", e, $"event=room_module_destroy_failed roomId={RoomId}");
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask<RspHead> HandleRequestAsync(int connectionId, ReqHead request, NetworkBuffer responsePayloadWriter)
    {
        try
        {
            return await _reqRspCenter.HandleRequestAsync(connectionId, request, responsePayloadWriter);
        }
        catch
        {
            return new RspHead(request.index, request.reqHash, 0, ErrorCode.InternalError, "room request failed", default);
        }
    }

    public virtual ValueTask HandleConnectionDisconnectedAsync(int connectionId, RoomConnectionContext context)
    {
        return ValueTask.CompletedTask;
    }

    public virtual bool ShouldCloseRoom(long timeNowMs)
    {
        RoomLifecycleState lifecycleState = LifecycleState;
        return lifecycleState == RoomLifecycleState.Empty ||
               lifecycleState == RoomLifecycleState.Closing ||
               lifecycleState == RoomLifecycleState.Closed;
    }

    public virtual void BeginCloseRoom(long timeNowMs)
    {
    }

    protected abstract void RegisterHandlers(ReqRspServerCenter center);
    protected virtual void OnRoomCreated()
    {
    }

    protected abstract void OnRoomUpdate(long timeNowMs, int frame);
    protected virtual void OnRoomStopped()
    {
    }

    private void UpdateMaxFrameElapsed(long elapsedTicks)
    {
        while (true)
        {
            long current = Interlocked.Read(ref _maxFrameElapsedTicks);
            if (elapsedTicks <= current)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _maxFrameElapsedTicks, elapsedTicks, current) == current)
            {
                return;
            }
        }
    }
}
