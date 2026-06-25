using System.Collections.Concurrent;
using GameServer.Core.Rooms;
using GameServer.Core.Protocol;
using kcp2k;
using MemoryPack;
using Network;
using Network.Client;
using UnityToolkit;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.ClientTestApp;

public sealed class ReqRspNetworkClient : IAsyncDisposable
{
    private readonly NetworkClient _client;
    private readonly DirectTransportProtocol _protocol;
    private readonly CancellationTokenSource _shutdown = new();
    private readonly ConcurrentDictionary<ushort, TaskCompletionSource<RspHead>> _pending = new();
    private Task? _runTask;

    private ReqRspNetworkClient(NetworkClient client, DirectTransportProtocol protocol)
    {
        _client = client;
        _protocol = protocol;
        _client.AddMsgHandler<RspHead>(OnRspHead);
        _client.AddMsgHandler<RoomPushHead>(OnRoomPushHead);
    }

    public DirectTransportProtocol Protocol => _protocol;
    public event Action<RoomPushHead>? RoomPushReceived;

    public static async Task<ReqRspNetworkClient> ConnectAsync(PrepareRoomConnectionReply connection)
    {
        IClientSocket socket;
        string scheme;
        if (connection.DirectProtocol == DirectTransportProtocol.Tcp)
        {
            socket = new TelepathyClientSocket();
            scheme = "tcp4";
        }
        else if (connection.DirectProtocol == DirectTransportProtocol.Kcp)
        {
            socket = new KcpClientSocket(new KcpConfig(), KcpChannel.Reliable);
            scheme = "kcp";
        }
        else
        {
            throw new NotSupportedException($"unsupported room transport protocol={connection.DirectProtocol}");
        }

        var connected = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        socket.OnConnected += () => connected.TrySetResult();

        var client = new NetworkClient(socket, 1000, false);
        var result = new ReqRspNetworkClient(client, connection.DirectProtocol);
        client.Run(new Uri($"{scheme}://{connection.Host}:{connection.Port}"), false);
        result._runTask = Task.Run(result.Run);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await connected.Task.WaitAsync(timeout.Token);
        return result;
    }

    public async Task<(TRsp rsp, RspHead head)> SendAsync<TReq, TRsp>(ushort index, TReq message)
        where TReq : INetworkReq
        where TRsp : INetworkRsp
    {
        byte[] payload = MemoryPackSerializer.Serialize(message);
        ReqHead request = new ReqHead
        {
            reqHash = TypeId<TReq>.stableId16,
            index = index,
            payload = new ArraySegment<byte>(payload),
        };

        RspHead response = await SendRawAsync(request);
        if (response.reqHash != request.reqHash)
        {
            throw new InvalidOperationException($"unexpected req hash={response.reqHash}");
        }

        if (response.error != NetworkErrorCode.Success)
        {
            throw new InvalidOperationException($"unexpected network error={response.error} {response.errorMessage}");
        }

        if (response.rspHash != TypeId<TRsp>.stableId16)
        {
            throw new InvalidOperationException($"unexpected rsp hash={response.rspHash}");
        }

        return (MemoryPackSerializer.Deserialize<TRsp>(response.payload), response);
    }

    public async Task<RspHead> SendRawAsync(ReqHead request)
    {
        var completion = new TaskCompletionSource<RspHead>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_pending.TryAdd(request.index, completion))
        {
            throw new InvalidOperationException($"duplicate request index={request.index}");
        }

        _client.Send(request, true);
        return await completion.Task;
    }

    public async ValueTask DisposeAsync()
    {
        _shutdown.Cancel();
        _client.Stop();

        if (_runTask != null)
        {
            await _runTask;
        }

        _client.Dispose();
        _shutdown.Dispose();
    }

    private void Run()
    {
        while (!_shutdown.IsCancellationRequested)
        {
            Thread.Sleep(1);
            if (_shutdown.IsCancellationRequested)
            {
                break;
            }

            _client.OnUpdate(0.001f);
        }
    }

    private void OnRspHead(in RspHead response)
    {
        if (_pending.TryRemove(response.index, out TaskCompletionSource<RspHead>? completion))
        {
            completion.TrySetResult(response);
        }
    }

    private void OnRoomPushHead(in RoomPushHead push)
    {
        RoomPushReceived?.Invoke(push);
    }
}
