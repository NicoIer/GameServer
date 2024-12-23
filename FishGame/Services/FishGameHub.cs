using System.Collections.Concurrent;
using Cysharp.Threading;
using GameCore.FishGame;
using MagicOnion.Server.Hubs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UnityToolkit;

namespace FishGame.Service;

public class FishGameHub : StreamingHubBase<IFishGameHud, IFishGameHudReceiver>, IFishGameHud
{
    public required IGroup _room = null!;
    private GameDatabase _database = null!;
    public IInMemoryStorage<GameWorld> _storage;
    private GameWorld _gameWorld = null!;
    private static uint _currentWorldId;
    private static readonly ConcurrentBag<uint> _worldIds = new ConcurrentBag<uint>();

    public IFishGameHud FireAndForget()
    {
        _database = Global.Singleton.Get<GameDatabase>();
        return this;
    }

    protected override async ValueTask OnDisconnected()
    {
        await _room.RemoveAsync(Context);
    }

    public async ValueTask<MatchRoomResponse> MatchRoom(uint userId)
    {
        Log.Information("MatchRoom: {userId}", userId);
        var user = await _database.fishGameDbContext.users.FirstOrDefaultAsync(u => u.uid == userId);
        if (user == null)
        {
            Log.Information("User not found: {userId}", userId);
            return new MatchRoomResponse { error = Error.UserNotFound };
        }

        IGroup? targetGroup = null;
        uint targetId = 0;
        foreach (var id in _worldIds)
        {
            if (!Group.RawGroupRepository.TryGet(id.ToString(), out var group)) continue;
            int memberCount = await group.GetMemberCountAsync();
            if (memberCount < 2)
            {
                Log.Information("MatchRoom: {userId} find one room: {roomId}", userId, id);
                targetGroup = group;
                targetId = id;
            }

            break;
        }

        if (targetGroup != null)
        {
            return new MatchRoomResponse { roomId = targetId, error = Error.Success };
        }

        Log.Information("Create new room for user: {userId}", userId);
        uint worldId = Interlocked.Increment(ref _currentWorldId);
        _gameWorld = new GameWorld(worldId);
        await Group.AddAsync(worldId.ToString(), _gameWorld);
        return new MatchRoomResponse { roomId = worldId, error = Error.Success };
    }

    public async ValueTask<Error> JoinAsync(uint userId, uint roomId)
    {
        Log.Information("JoinAsync: {userId} {roomId}", userId, roomId);
        var user = await _database.fishGameDbContext.users.FirstOrDefaultAsync(u => u.uid == userId);
        if (user == null)
        {
            Log.Information("User not found: {userId}", userId);
            return Error.UserNotFound;
        }

        if (!Group.RawGroupRepository.TryGet(roomId.ToString(), out var targetRoom))
        {
            Log.Information("Room not found: {roomId}", roomId);
            return Error.RoomNotFound;
        }
        
        _room = targetRoom;
        _storage = _room.GetInMemoryStorage<GameWorld>();
        
        Log.Information("JoinAsync: {userId} {roomId}", userId, roomId);
        
        Global.Singleton.Get<LoopSystem>().AddOnUpdate(OnUpdate);
        
        return Error.Success;
    }

    private void OnUpdate(in TimeSpan timeSpan)
    {
        // 推送游戏世界状态
        BroadcastToSelf(_room).PushGame(_storage.Get(ConnectionId));
    }


    public async ValueTask<Error> ReadyAsync(uint userId)
    {
        throw new NotImplementedException();
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