using System.Collections.Concurrent;
using Cysharp.Threading;
using GameCore.FishGame;
using MagicOnion.Server.Hubs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UnityToolkit;

namespace FishGame.Service;

public class FishGameHub : StreamingHubBase<IFishGameHub, IGameHudReceiver>, IFishGameHub
{
    public required IGroup _room = null!;
    private GameDatabase _database = null!;
    private GameWorld _gameWorld = null!;
    private static uint _currentWorldId;
    private static readonly ConcurrentBag<uint> _worldIds = new ConcurrentBag<uint>();

    public IFishGameHub FireAndForget()
    {
        _database = Global.Singleton.Get<GameDatabase>();
        return this;
    }

    protected override async ValueTask OnDisconnected()
    {
        await _room.RemoveAsync(Context);
    }

    public async ValueTask<MatchRoomResponse> MatchRoom(string macToken)
    {
        Log.Information("MatchRoom: {userId}", macToken);
        var user = await _database.fishGameDbContext.users.FirstOrDefaultAsync(u => u.macToken == macToken);
        if (user == null)
        {
            Log.Information("User not found: {macToken}", macToken);
            return new MatchRoomResponse { error = Error.userNotFound };
        }

        IGroup? targetGroup = null;
        uint targetId = 0;
        foreach (var id in _worldIds)
        {
            if (!Group.RawGroupRepository.TryGet(id.ToString(), out var group)) continue;
            int memberCount = await group.GetMemberCountAsync();
            if (memberCount < 2)
            {
                Log.Information("MatchRoom: {macToken} find one room: {roomId}", macToken, id);
                targetGroup = group;
                targetId = id;
            }

            break;
        }

        if (targetGroup != null)
        {
            return new MatchRoomResponse { roomId = targetId, error = Error.success };
        }

        Log.Information("Create new room for user: {macToken}", macToken);
        uint worldId = Interlocked.Increment(ref _currentWorldId);
        _gameWorld = new GameWorld(worldId);
        await Group.AddAsync(worldId.ToString(), _gameWorld);
        return new MatchRoomResponse { roomId = worldId, error = Error.success };
    }

    public async ValueTask<Error> JoinAsync(string macToken, uint roomId)
    {
        Log.Information("JoinAsync: {userId} {roomId}", macToken, roomId);
        var user = await _database.fishGameDbContext.users.FirstOrDefaultAsync(u => u.macToken == macToken);
        if (user == null)
        {
            Log.Information("User not found: {macToken}", macToken);
            return Error.userNotFound;
        }

        if (!Group.RawGroupRepository.TryGet(roomId.ToString(), out var targetRoom))
        {
            Log.Information("Room not found: {roomId}", roomId);
            return Error.roomNotFound;
        }

        _room = targetRoom;

        Log.Information("JoinRoom: {macToken} {roomId}", macToken, roomId);

        Global.Singleton.Get<LoopSystem>().AddOnUpdate(OnUpdate);

        return Error.success;
    }

    private void OnUpdate(in TimeSpan timeSpan)
    {
        // 推送游戏世界状态
        BroadcastToSelf(_room).PushGame(_gameWorld);
    }


    public async ValueTask<Error> ReadyAsync(string macToken)
    {
        throw new NotImplementedException();
    }

    public async ValueTask LeaveAsync(string macToken)
    {
        Log.Information("LeaveAsync: {macToken}", macToken);
        var user = await _database.fishGameDbContext.users.FirstOrDefaultAsync(u => u.macToken == macToken);
        if (user == null)
        {
            Log.Information("User not found: {macToken}", macToken);
            return;
        }

        // 从房间移除当前用户
        await _room.RemoveAsync(Context);
        Log.Information("Remove user[{macToken}] from room[{name}] ", macToken, _room.GroupName);
        Global.Singleton.Get<LoopSystem>().RemoveOnUpdate(OnUpdate);
    }
}