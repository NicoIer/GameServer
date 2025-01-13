using FishGame;
using GameCore.FishGame;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using Serilog;
using StatusCode = GameCore.FishGame.StatusCode;

namespace FishGameTest;

public class GlobalHubTests : IGlobalHubReceiver
{
    private IGlobalHub client;
    // private GrpcChannel channel;

    // [TearDown]
    // public void Dispose()
    // {
    //     channel.Dispose();
    // }
    
    [SetUp]
    public async Task Setup()
    {
       var channel = GrpcChannel.ForAddress("http://localhost:5199",new GrpcChannelOptions()
        {
            Credentials = ChannelCredentials.Insecure,
        });
        MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;
        this.client = await StreamingHubClient.ConnectAsync<IGlobalHub, IGlobalHubReceiver>(channel, this);
    }


    [Test]
    public async Task Test1()
    {
        // string name = Random.Shared.Next().ToString();
        // RegisterResponse rsp = await client.Register(name);
        // if (rsp.userId == 0)
        // {
        //     Assert.IsTrue(rsp.error.code == StatusCode.Failed,"rsp.error.code == StatusCode.Failed");
        // }
        // else
        // {
        //     Assert.IsTrue(rsp.error.code == StatusCode.Success,"rsp.error.code == StatusCode.Success");
        // }
        Assert.Pass();
    }

    public void PushTimeMilliSeconds(int milliSeconds)
    {
        throw new NotImplementedException();
    }

    public void PushMessage(string message)
    {
        throw new NotImplementedException();
    }
}