

// See https://aka.ms/new-console-template for more information

using Network;
using Network.Server;


TelepathyServerSocket socket = new TelepathyServerSocket
{
    port = 8848
};

TimeSpan pingInterval = TimeSpan.FromSeconds(1);
TimeSpan pushInterval = TimeSpan.FromSeconds(2);
NetworkTimeServer timeServer = new NetworkTimeServer(socket, pushInterval, pingInterval);


socket.Start();

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