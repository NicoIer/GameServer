using System.Collections.Concurrent;
using System.Reflection;
using Google.Protobuf;

namespace GameServer.Rpc.Server;

public sealed class GrpcMessageDispatcher
{
    private readonly Dictionary<Type, IGrpcMessageHandler> _handlers = new();
    private readonly ConcurrentQueue<IGrpcMessage> _messageQueue = new();

    public void RegisterHandler(IGrpcMessageHandler handler)
    {
        _handlers[handler.RequestType] = handler;
    }

    public void RegisterHandlersFromAssembly(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract)
            {
                continue;
            }

            if (type.GetCustomAttribute<GrpcMessageHandlerAttribute>() == null)
            {
                continue;
            }

            var handler = (IGrpcMessageHandler)Activator.CreateInstance(type)!;
            RegisterHandler(handler);
        }
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken,
        int timeoutMs = 30000)
        where TRequest : class, IMessage
        where TResponse : class, IMessage
    {
        var tcs = new TaskCompletionSource<TResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
        var message = new GrpcMessage<TRequest, TResponse>
        {
            Request = request,
            Tcs = tcs,
            CancellationToken = cancellationToken,
        };

        _messageQueue.Enqueue(message);

        using var timeoutCts = new CancellationTokenSource(timeoutMs);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        using var registration = linkedCts.Token.Register(() => tcs.TrySetCanceled(linkedCts.Token));

        return await tcs.Task;
    }

    public void ProcessMessages()
    {
        while (_messageQueue.TryDequeue(out IGrpcMessage? message))
        {
            if (message.IsCanceled())
            {
                message.SetCanceled();
                continue;
            }

            Type requestType = message.GetRequest().GetType();
            if (!_handlers.TryGetValue(requestType, out IGrpcMessageHandler? handler))
            {
                message.SetException(new InvalidOperationException($"No gRPC handler registered for {requestType.FullName}"));
                continue;
            }

            handler.Handle(message);
        }
    }
}
