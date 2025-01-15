// // See https://aka.ms/new-console-template for more information
//
// using Network;
// using Network.Server;
//
//
// TelepathyServerSocket socket = new TelepathyServerSocket
// {
//     port = 8849
// };
//
// TimeSpan pingInterval = TimeSpan.FromSeconds(1);
// TimeSpan pushInterval = TimeSpan.FromSeconds(2);
// NetworkServer server = new NetworkServer(socket, 60, true);
//
// NetworkTimeServer timeServer = new NetworkTimeServer(server, pushInterval, pingInterval);
//
//
// Task t1 = Task.Run(() =>
// {
//     Console.WriteLine("Press any key to exit...");
//     Console.ReadLine();
// });
//
// Task t2 = server.Run();
//
// await Task.WhenAny(t1, t2);

using Network.Time;

var server = new NetworkTimeServer();
await server.Start(8848);
