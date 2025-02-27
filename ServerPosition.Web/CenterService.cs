// using GameCore.Position;
// using MagicOnion;
// using MagicOnion.Server;
// using Network.Server;
//
// namespace PositionServer.Web;
//
// public class CenterService : ServiceBase<ICenterService>, ICenterService
// {
//     public async UnaryResult<RspGetGameServer> GetGameServer(UserInfo userInfo)
//     {
//         NetworkService.Singleton.RequestByCallback<CmdGameServerInfo, RspCmdServerInfo>(default, Callback);
//         RspCmdServerInfo? info = null;
//
//         void Callback(in RspCmdServerInfo rsp)
//         {
//             info = rsp;
//         }
//
//         // wait unitl callback
//         while (!Context.CallContext.CancellationToken.IsCancellationRequested && info == null)
//         {
//             await Task.Yield();
//         }
//
//         return new RspGetGameServer
//         {
//             serverInfo = info.Value.serverInfo,
//             userInfo = userInfo
//         };
//     }
// }