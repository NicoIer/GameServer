// See https://aka.ms/new-console-template for more information

using Network;
using Network.Client;

IClientSocket socket = new TelepathyClientSocket();
UriBuilder builder = new UriBuilder
{
    Host = "localhost",
    Port = 8848
};

NetworkTimeClient client = new NetworkTimeClient(socket);
socket.Connect(builder.Uri);

Task t1= Task.Run(() =>
{
    Console.WriteLine("Press any key to exit...");
    Console.ReadLine();
});

Task t2 = Task.Run(() =>
{
    while (true)
    {
        socket.TickIncoming();
        socket.TickOutgoing();
    }
});

await Task.WhenAny(t1, t2);