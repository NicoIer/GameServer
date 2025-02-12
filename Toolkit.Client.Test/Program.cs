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

using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MemoryPack;
using Network;
using Network.Time;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");



using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
{
    socket.Connect("8.8.8.8", 65530);
    var localEndPoint = socket.LocalEndPoint as IPEndPoint;
    Debug.Assert(localEndPoint != null, "localEndPoint != null");
    Console.WriteLine(localEndPoint.Address);
}


// var entry = Dns.GetHostEntry(Dns.GetHostName());
//
// foreach (var ipAddress in entry.AddressList)
// {
//     Console.WriteLine(ipAddress);
// }

//
// var client = new NetworkTimeClient();
//
// await client.Run("127.0.0.1", 8848);