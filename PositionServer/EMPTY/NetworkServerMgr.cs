// using System.Collections.Concurrent;
// using System.Net;
// using System.Net.Sockets;
// using GameCore.Position;
// using Network.Server;
//
// namespace PositionServer;
//
// public class NetworkServerMgr
// {
//     private readonly IPEndPoint _localEndPoint;
//     private readonly ConcurrentBag<NetworkServer> _servers;
//
//     public NetworkServerMgr()
//     {
//         // 计算本机内网IP
//         _servers = new ConcurrentBag<NetworkServer>();
//         using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
//         {
//             socket.Connect("8.8.8.8", 65530);
//             _localEndPoint = (socket.LocalEndPoint as IPEndPoint)!;
//         }
//     }
//
//     public bool GetAvailableServer(out GameServerInfo info)
//     {
//         foreach (var server in _servers)
//         {
//             info = new GameServerInfo(_localEndPoint.Address.ToString(), server.socket.Port);
//             return true;
//         }
//
//         throw new NotImplementedException();
//     }
// }