// using System.Collections;
// using System.Diagnostics;
// using System.Net;
// using System.Net.NetworkInformation;
// using System.Net.Sockets;
// using GameCore.Position;
// using Network;
// using Network.Server;
// using UnityToolkit;
//
// namespace PositionServer;
//
// public class NetworkRpc : ISystem, IOnInit<NetworkServer>
// {
//     public CommonCommand command;
//     private NetworkServer _server;
//     private readonly NetworkServerMgr _mgr;
//
//     public NetworkRpc(NetworkServerMgr mgr)
//     {
//         this._mgr = mgr;
//     }
//
//
//     public void OnInit(NetworkServer t)
//     {
//         _server = t;
//         _server.AddMsgHandler<CmdGameServerInfo>(OnCmdGameServerInfo);
//     }
//
//     private void OnCmdGameServerInfo(in int connectId, in CmdGameServerInfo message)
//     {
//         if (!_mgr.GetAvailableServer(out var info))
//         {
//             var rpcErrorMessage = RpcErrorMessage.From(message.requestId, RpcErrorCode.Failed, "No available server");
//             _server.Send(connectId, rpcErrorMessage);
//         }
//         
//         _server.Send(connectId, new RspCmdServerInfo(message.requestId, info));
//     }
//
//     public void Dispose()
//     {
//         command.Execute();
//         // TODO release managed resources here
//     }
// }