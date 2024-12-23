using Cysharp.Threading;
using GameCore.FishGame;
using MagicOnion.Server.Hubs;
using Microsoft.EntityFrameworkCore;
using UnityToolkit;

namespace FishGame.Service;

public class FishGameHub : StreamingHubBase<IFishGameHud, IFishGameHudReceiver>, IFishGameHud
{
    private IGroup _room;
    private FishGamePlayer _player;
    // private IInMemoryStorage<GameWorld> _storage;

    private GameDatabase _database;
    public static uint currentRoomId = 0;

    public IFishGameHud FireAndForget()
    {
        _database = Global.Singleton.Get<GameDatabase>();
        return this;
    }

    public async ValueTask JoinAsync(uint userId)
    {
        var user = await _database.fishGameDbContext.users.FirstOrDefaultAsync(u => u.uid == userId);
        if (user == null)
        {
            return;
        }

        uint roomId = Interlocked.Increment(ref currentRoomId);
        Global.Singleton.Get<LoopSystem>().AddOnUpdate(OnUpdate);
        _player = new FishGamePlayer(userId, user.nickname);
        
        
    }

    private void OnUpdate(in TimeSpan timeSpan)
    {
        // 推送游戏世界状态
        // BroadcastToSelf(_room).PushGame(_storage.Get(ConnectionId));
    }


    public async ValueTask ReadyAsync(uint userId)
    {
    }

    public async ValueTask LeaveAsync(uint userId)
    {
        var user = await _database.fishGameDbContext.users.FirstOrDefaultAsync(u => u.uid == userId);
        if (user == null)
        {
            return;
        }

        Global.Singleton.Get<LoopSystem>().RemoveOnUpdate(OnUpdate);
    }
}