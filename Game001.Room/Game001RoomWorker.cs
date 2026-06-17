using System.Collections.Concurrent;
using GameServer.Core.Protocol;
using Google.Protobuf;
using Network;

namespace Game001.Room;

public sealed class Game001RoomWorker : IAsyncDisposable
{
    private readonly Game001RoomReqRspDispatcher _dispatcher;
    private readonly ConcurrentQueue<IGame001RoomWorkItem> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);
    private readonly CancellationTokenSource _shutdown = new();
    private Task? _workerTask;

    public Game001RoomWorker(Game001RoomReqRspDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public Task StartAsync()
    {
        _workerTask = Task.Run(RunAsync);
        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        _shutdown.Cancel();
        _signal.Release();

        if (_workerTask != null)
        {
            await _workerTask;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _shutdown.Dispose();
        _signal.Dispose();
    }

    public Task<int> AddConnectionAsync(long uid, string roomId)
    {
        var item = new AddConnectionWorkItem(uid, roomId);
        Enqueue(item);
        return item.Task;
    }

    public Task RemoveConnectionAsync(int connectionId)
    {
        var item = new RemoveConnectionWorkItem(connectionId);
        Enqueue(item);
        return item.Task;
    }

    public Task<RspHead> HandleRequestAsync(int connectionId, ReqHead request)
    {
        var item = new RoomRequestWorkItem(connectionId, request);
        Enqueue(item);
        return item.Task;
    }

    public Task<GameResponse> HandleDataAsync(long uid, ByteString data)
    {
        var item = new GameIngressWorkItem(uid, data);
        Enqueue(item);
        return item.Task;
    }

    private void Enqueue(IGame001RoomWorkItem item)
    {
        if (_shutdown.IsCancellationRequested)
        {
            item.Cancel();
            return;
        }

        _queue.Enqueue(item);
        _signal.Release();
    }

    private async Task RunAsync()
    {
        try
        {
            while (!_shutdown.IsCancellationRequested)
            {
                await _signal.WaitAsync(_shutdown.Token);

                while (_queue.TryDequeue(out IGame001RoomWorkItem? item))
                {
                    item.Execute(_dispatcher);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }

        while (_queue.TryDequeue(out IGame001RoomWorkItem? item))
        {
            item.Cancel();
        }
    }

    private interface IGame001RoomWorkItem
    {
        void Execute(Game001RoomReqRspDispatcher dispatcher);
        void Cancel();
    }

    private sealed class AddConnectionWorkItem : IGame001RoomWorkItem
    {
        private readonly long _uid;
        private readonly string _roomId;
        private readonly TaskCompletionSource<int> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public AddConnectionWorkItem(long uid, string roomId)
        {
            _uid = uid;
            _roomId = roomId;
        }

        public Task<int> Task => _completion.Task;

        public void Execute(Game001RoomReqRspDispatcher dispatcher)
        {
            _completion.SetResult(dispatcher.AddConnection(_uid, _roomId));
        }

        public void Cancel()
        {
            _completion.SetCanceled();
        }
    }

    private sealed class RemoveConnectionWorkItem : IGame001RoomWorkItem
    {
        private readonly int _connectionId;
        private readonly TaskCompletionSource<bool> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public RemoveConnectionWorkItem(int connectionId)
        {
            _connectionId = connectionId;
        }

        public Task Task => _completion.Task;

        public void Execute(Game001RoomReqRspDispatcher dispatcher)
        {
            dispatcher.RemoveConnection(_connectionId);
            _completion.SetResult(true);
        }

        public void Cancel()
        {
            _completion.SetCanceled();
        }
    }

    private sealed class RoomRequestWorkItem : IGame001RoomWorkItem
    {
        private readonly int _connectionId;
        private readonly ReqHead _request;
        private readonly TaskCompletionSource<RspHead> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public RoomRequestWorkItem(int connectionId, ReqHead request)
        {
            _connectionId = connectionId;
            _request = request;
        }

        public Task<RspHead> Task => _completion.Task;

        public void Execute(Game001RoomReqRspDispatcher dispatcher)
        {
            _completion.SetResult(dispatcher.HandleRequest(_connectionId, _request));
        }

        public void Cancel()
        {
            _completion.SetCanceled();
        }
    }

    private sealed class GameIngressWorkItem : IGame001RoomWorkItem
    {
        private readonly long _uid;
        private readonly ByteString _data;
        private readonly TaskCompletionSource<GameResponse> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public GameIngressWorkItem(long uid, ByteString data)
        {
            _uid = uid;
            _data = data;
        }

        public Task<GameResponse> Task => _completion.Task;

        public void Execute(Game001RoomReqRspDispatcher dispatcher)
        {
            _completion.SetResult(dispatcher.HandleData(_uid, _data));
        }

        public void Cancel()
        {
            _completion.SetCanceled();
        }
    }
}
