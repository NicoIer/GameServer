// #if NETCOREAPP
//
// using System.Collections.Concurrent;
// using System.Security.Cryptography;
// using GameCore.Position;
// using MagicOnion.Serialization;
// using MagicOnion.Serialization.MemoryPack;
// using MemoryPack;
// using Network.Client;
// using Serilog;
// using UnityToolkit;
//
// namespace Network.Server
// {
//     public class NetworkService : LazySingleton<NetworkService>
//     {
//         private WebApplication _application;
//         public NetworkClient rpcClient { get; private set; }
//
//         public delegate void HandleRpcMessage<T>(in T rsp) where T : ICmdRsp;
//
//         private delegate void HandleRpcMessage(in RspRpcPacket rsp);
//
//         private Dictionary<int, HandleRpcMessage> _waitingRpcIds;
//
//         private readonly CancellationTokenSource _cts;
//
//         private Dictionary<int, INetworkMessage> _waitingRpcMessages =
//             new Dictionary<int, INetworkMessage>();
//
//         public NetworkService()
//         {
//             _cts = new CancellationTokenSource();
//         }
//
//         public void Setup(string[] args)
//         {
//             MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;
//             Log.Information("Start GameRPC,SetMagicOnionSerializerProvider={0}", MagicOnionSerializerProvider.Default);
//             // MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions
//             // .WithResolver(StaticCompositeResolver.Instance);
//
//             var builder = WebApplication.CreateBuilder(args);
//             builder.WebHost.UseKestrel(options =>
//             {
//                 options.ConfigureEndpointDefaults(listenOptions =>
//                 {
//                     listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols
//                         .Http1AndHttp2AndHttp3;
//                 });
//             });
//             builder.Services.AddSerilog(); // Add this line(Serilog)
//             builder.Services.AddGrpc(); // Add this line(Grpc.AspNetCore)
//             builder.Services.AddMagicOnion(x =>
//             {
//                 x.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
//             }); // Add this line(MagicOnion.Server)
//
//             _application = builder.Build();
//
//             // Configure the HTTP request pipeline.
//             if (!_application.Environment.IsDevelopment())
//             {
//                 _application.UseExceptionHandler("/Error", createScopeForErrors: true);
//                 // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//                 _application.UseHsts();
//             }
//
//             // // Configure the HTTP request pipeline.
//             // if (_application.Environment.IsDevelopment())
//             // {
//             //     _application.UseSwagger();
//             //     _application.UseSwaggerUI();
//             // }
//
//             _application.MapMagicOnionService();
//             _application.UseRouting();
//             _application.UseHttpsRedirection();
//
//             _application.UseSerilogRequestLogging(); // Add this line(Serilog)
//
//             TelepathyClientSocket socket = new TelepathyClientSocket();
//             NetworkClient client = new NetworkClient(socket);
//             rpcClient = client;
//             _waitingRpcIds = new Dictionary<int, HandleRpcMessage>();
//             rpcClient.AddMsgHandler<RspRpcPacket>(OnRpcRsp);
//         }
//
//
//         public Task Run(string rpcHost, int rpcPort)
//         {
//             var t1 = _application.RunAsync();
//             UriBuilder builder = new UriBuilder
//             {
//                 Scheme = "tcp4",
//                 Host = rpcHost,
//                 Port = rpcPort
//             };
//             var t2 = rpcClient.Run(builder.Uri);
//             return Task.WhenAny(t1, t2);
//         }
//
//         public override void Dispose()
//         {
//             base.Dispose();
//             _application.DisposeAsync();
//             rpcClient.Dispose();
//             _cts.Cancel();
//         }
//
//
//         public void RequestByCallback<TReq, TRsp>(TReq req, HandleRpcMessage<TRsp> handler)
//             where TReq : ICmdReq where TRsp : ICmdRsp
//         {
//             var payloadBuffer = NetworkBufferPool.Shared.Get();
//             MemoryPackSerializer.Serialize(payloadBuffer, req);
//
//             // 随机一个id
//             while (!_cts.Token.IsCancellationRequested)
//             {
//                 var requestId = RandomNumberGenerator.GetInt32(ushort.MinValue, ushort.MaxValue);
//                 if (_waitingRpcIds.ContainsKey(requestId)) continue;
//
//                 var reqRpcPacket = new ReqRpcPacket(requestId, payloadBuffer.ToArraySegment());
//                 rpcClient.Send(reqRpcPacket);
//
//                 _waitingRpcIds.Add(requestId, Warp(handler));
//
//                 NetworkBufferPool.Shared.Return(payloadBuffer);
//                 break;
//             }
//         }
//
//         private HandleRpcMessage Warp<TRsp>(HandleRpcMessage<TRsp> handler) where TRsp : ICmdRsp
//         {
//             return (in RspRpcPacket rsp) =>
//             {
//                 var rspMessage = MemoryPackSerializer.Deserialize<TRsp>(rsp.data);
//                 handler(rspMessage);
//             };
//         }
//
//
//         private void OnRpcRsp(RspRpcPacket obj)
//         {
//             if (_waitingRpcIds.TryGetValue(obj.requestId, out var handler))
//             {
//                 handler(obj);
//                 _waitingRpcIds.Remove(obj.requestId);
//             }
//
//             ToolkitLog.Error("收到了一个未知的RPC回复");
//         }
//     }
// }
// #endif