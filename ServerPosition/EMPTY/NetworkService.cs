// using GameCore.Position;
// using Network.Server;
// using UnityToolkit;
//
// namespace GameServer;
//
// public class NetworkService : ISystem, IOnInit<NetworkServer>
// {
//     private NetworkServer _homeServer;
//     private List<NetworkServer> _gameList;
//     private CommonCommand _command;
//     public void OnInit(NetworkServer server)
//     {
//         _homeServer = server;
//         _gameList = new List<NetworkServer>();
//         // ICommand c1 = _homeServer.AddMsgHandler<ReqCreateRoomMessage>(OnReqCreateRoom);
//         // ICommand c2 = _homeServer.AddMsgHandler<ReqJoinRoomMessage>(OnReqJoinRoom);
//         _command = new CommonCommand(() =>
//         {
//             // c1.Execute();
//             // c2.Execute();
//         });
//     }
//
//     // private void OnReqJoinRoom(in int connectionId, in ReqJoinRoomMessage message)
//     // {
//     //     
//     // }
//     //
//     // private void OnReqCreateRoom(in int connectionId, in ReqCreateRoomMessage message)
//     // {
//     //     
//     // }
//
//
//     public void Dispose()
//     {
//         // TODO release managed resources heres
//         _command.Execute();
//     }
// }