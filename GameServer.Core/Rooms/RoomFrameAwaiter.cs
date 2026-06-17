using GameServer.Core.Fibers;

namespace GameServer.Core.Rooms;

public sealed class RoomFrameAwaiter
{
    private readonly List<TaskCompletionSource<bool>> _nextFrameWaiters = new();

    public ValueTask WaitNextFrameAsync()
    {
        Fiber fiber = Fiber.Current ?? throw new InvalidOperationException("room frame await must be called inside a room fiber");
        var completion = new TaskCompletionSource<bool>();
        _nextFrameWaiters.Add(completion);
        fiber.PostNextUpdate(() =>
        {
            _nextFrameWaiters.Remove(completion);
            completion.TrySetResult(true);
        });
        return new ValueTask(completion.Task);
    }

    public void Cancel()
    {
        foreach (TaskCompletionSource<bool> waiter in _nextFrameWaiters)
        {
            waiter.TrySetCanceled();
        }

        _nextFrameWaiters.Clear();
    }
}
