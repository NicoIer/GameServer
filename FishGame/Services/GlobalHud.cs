using System;
using System.Threading;
using System.Threading.Tasks;
using GameCore.FishGame;
using MagicOnion.Server.Hubs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FishGame.Service;

public class GlobalHud : StreamingHubBase<IGlobalHud, IGlobalServiceHubReceiver>, IGlobalHud
{
    private GameDatabase _database;

    public IGlobalHud FireAndForget()
    {
        Log.Information("{0} FireAndForget", nameof(GlobalHud));
        _database = Global.Singleton.Get<GameDatabase>();
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

    protected override ValueTask OnConnected()
    {
        Log.Information("{0} OnConnected", nameof(GlobalHud));
        return base.OnConnected();
    }


    public async ValueTask<RegisterResponse> Register(string nickName)
    {
        bool contains = await _database.fishGameDbContext.users.ContainsAsync(new User { nickname = nickName });
        if (contains)
        {
            return new RegisterResponse
            {
                error = new Error()
                {
                    code = StatusCode.Failed,
                    msg = "昵称已存在"
                }
            };
        }

        var user = new User { nickname = nickName };
        long uid = Interlocked.Increment(ref _database.uidCounter);
        if (uid > uint.MaxValue)
        {
            throw new Exception("uid limit reached: " + uid);
        }

        user.uid = (uint)uid;
        await _database.fishGameDbContext.users.AddAsync(user);
        await _database.fishGameDbContext.SaveChangesAsync();
        return new RegisterResponse { userId = (uint)user.id, error = Error.Success };
    }

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

    public ValueTask SendHeartbeat(uint userId)
    {
        throw new NotImplementedException();
    }
}