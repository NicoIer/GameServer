using System.Security.Authentication;
using System.Text;
// using GameCore.Service;
// using Grpc.Core;
// using Grpc.Net.Client;
// using MagicOnion.Client;
//
// var httpClientHandler = new HttpClientHandler
// {
//     ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true  //忽略掉证书异常
// };
//
// GrpcChannelOptions options = new GrpcChannelOptions()
// {
//     HttpHandler = httpClientHandler
// };
//
//
// var channel = GrpcChannel.ForAddress("https://localhost:7121",options);
// var client = MagicOnionClient.Create<IGameService>(channel);
// var result = client.SumAsync(1, 2).ResponseAsync.Result;
// Console.WriteLine(result);

int score = 12100000;
string text = "";
if(score > 10000000)
{
    text = (score / 1000000f).ToString("0.0") + "m";
}
else if(score > 10000)
{
    text = (score / 1000f).ToString("0.0") + "k";
}
else
{
    text = score.ToString();
}

Console.WriteLine(text);