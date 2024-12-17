using GameCore.FishGame;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;

namespace FishGame.Service;

public class GlobalService : StreamingHubBase<IGlobalService, IGlobalServiceReceiver>, IGlobalService
{
    private Dictionary<uint, User> _users;


    public IGlobalService FireAndForget()
    {
        _users = new Dictionary<uint, User>();
        return this;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public Task WaitForDisconnect()
    {
        return Task.CompletedTask;
    }
    //
    // public ValueTask<RegisterResult> Register(string nickName)
    // {
    //     throw new NotImplementedException();
    // }

    public ValueTask<UserState> GetState(uint userId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<StatusCode> Login(uint userId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<StatusCode> Logout(uint userId)
    {
        throw new NotImplementedException();
    }
}