using System.Diagnostics;
using GameCore.FishGame;
using MagicOnion.Server.Hubs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FishGame.Services;

public class GlobalHub : StreamingHubBase<IGlobalHub, IGlobalHubReceiver>, IGlobalHub
{
    private GameDatabase _database = null!;


    public GlobalHub()
    {
        Log.Information("{0} ctor", nameof(GlobalHub));
    }

    protected override ValueTask OnConnected()
    {
        Log.Information("{0} OnConnected", nameof(GlobalHub));
        _database = Global.Singleton.Get<GameDatabase>();
        return base.OnConnected();
    }

    protected override ValueTask OnConnecting()
    {
        return base.OnConnecting();
    }

    protected override ValueTask OnDisconnected()
    {
        Log.Information("{0} OnDisconnected", nameof(GlobalHub));
        return base.OnDisconnected();
    }


    public async ValueTask<StateResponse> GetState(string macToken)
    {
        bool contains = await _database.fishGameDbContext.users.ContainsAsync(new User { macToken = macToken });
        if (!contains)
        {
            return new StateResponse { error = Error.userNotFound };
        }

        var user = await _database.fishGameDbContext.users.FirstAsync(u => u.macToken == macToken);
        Debug.Assert(user != null, nameof(user) + " != null");
        return new StateResponse { state = user.globalState, error = Error.success };
    }

    public async ValueTask<Error> Login(string macToken)
    {
        Log.Information("Login macToken:{0}", macToken);
        bool contains = await _database.fishGameDbContext.users.Where(u => u.macToken == macToken).AnyAsync();
        Log.Information("Login contains:{0}", contains);
        User user;
        if (!contains)
        {
            uint nameId = Interlocked.Increment(ref _database.uidCounter);
            if (nameId == uint.MaxValue)
            {
                throw new Exception("uid limit reached: " + nameId);
            }

            user = new User
            {
                nickname = nameId.ToString(),
                macToken = macToken
            };

            Log.Information("Register Success nickName: {0} macToken: {1}", nameId, macToken);

            await _database.fishGameDbContext.users.AddAsync(user);
        }

        user = await _database.fishGameDbContext.users.FirstAsync(u => u.macToken == macToken);

        if (user.globalState == GlobalState.Offline)
        {
            user.globalState = GlobalState.Hall;
            _database.fishGameDbContext.Update(user);
            await _database.fishGameDbContext.SaveChangesAsync();
        }

        return Error.success;
    }

    public ValueTask<Error> Logout(string macToken)
    {
        bool contains = _database.fishGameDbContext.users.Contains(new User { macToken = macToken });
        if (!contains)
        {
            return new ValueTask<Error>(Error.userNotFound);
        }

        var user = _database.fishGameDbContext.users.First(u => u.macToken == macToken);
        Debug.Assert(user != null, nameof(user) + " != null");
        user.globalState = GlobalState.Offline;

        _database.fishGameDbContext.Update(user);
        return new ValueTask<Error>(Error.success);
    }

    public async ValueTask Heartbeat(string macToken)
    {
        Log.Information("Heartbeat {0}", macToken);
        bool contains = await _database.fishGameDbContext.users.ContainsAsync(new User { macToken = macToken });
        if (!contains)
        {
            Log.Warning("Heartbeat user not found: {0} ConnectionId: {1}", macToken, Context.CallContext.Peer);
            return;
        }

        Log.Information("Heartbeat user found: {0} ConnectionId: {1}", macToken, Context.CallContext.Peer);
        var user = await _database.fishGameDbContext.users.FirstAsync(u => u.macToken == macToken);
        user.lastActionTimeSeconds = DateTime.UtcNow.Second;
        _database.fishGameDbContext.Update(user);
    }
}