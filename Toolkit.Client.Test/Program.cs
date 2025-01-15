// // See https://aka.ms/new-console-template for more information
//
// using Network;
// using Network.Client;
//
// IClientSocket socket = new TelepathyClientSocket();
// UriBuilder builder = new UriBuilder
// {
//     Host = "localhost",
//     Port = 8849
// };
//
// NetworkClient client = new NetworkClient(socket);
//
//
// NetworkTimeClient timeClient = new NetworkTimeClient(client);
//
// Task t1= Task.Run(() =>
// {
//     Console.WriteLine("Press any key to exit...");
//     Console.ReadLine();
// });
//
// Task t2 = client.Run(builder.Uri);
//
// await Task.WhenAny(t1, t2);

using System.Net;
using System.Net.Sockets;
using MemoryPack;
using Network;
using Network.Time;


var client = new NetworkTimeClient();

await client.Run("127.0.0.1", 8848);