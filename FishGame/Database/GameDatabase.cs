// using LiteDB;

using System.Diagnostics;
using GameCore.FishGame;
using Serilog;
using UnityToolkit;

namespace FishGame;

public class GameDatabase : ISystem, IOnInit
{
    public FishGameContext fishGameDbContext { get; private set; } = null!;

    public uint uidCounter;

    private System.Threading.Timer _saveTimer = null!;
    public const float OfflineTime = 5;

    public void OnInit()
    {
        Log.Information("GameDatabase OnInit");
        fishGameDbContext = new FishGameContext();

        var uidConfig = fishGameDbContext.configs.Find(nameof(uidCounter));
        if (uidConfig != null)
        {
            uidCounter = uidConfig.value;
        }
        else
        {
            Log.Information("GameDatabase: uidCounter not found, create new one");
            uidCounter = 0;
            fishGameDbContext.configs.Add(new UintConfig { id = nameof(uidCounter), value = uidCounter });
            fishGameDbContext.SaveChanges();
        }

        _saveTimer = new System.Threading.Timer(SaveDatabase, null, 0, TimeSpan.FromSeconds(30).Milliseconds);
    }

    private void SaveDatabase(object? state)
    {
        OfflineTimerCallback();
        fishGameDbContext.SaveChanges();
    }

    private void OfflineTimerCallback()
    {
        Log.Information("OfflineTimerCallback");
        var users = fishGameDbContext.users.Where(u => u.globalState != GlobalState.Offline);
        foreach (var user in users)
        {
            if (!(user.lastActionTimeSeconds + OfflineTime < DateTime.UtcNow.Second)) continue;
            user.globalState = GlobalState.Offline;
            Log.Information("User {0} is offline", user.id);
            fishGameDbContext.Update(user);
        }
    }

    public void Dispose()
    {
        SaveDatabase(null);
        Log.Information("GameDatabase Dispose");
        _saveTimer.Dispose();
        // 写回uidCounter
        var uidConfig = fishGameDbContext.configs.Find(nameof(uidCounter));
        Debug.Assert(uidConfig != null, "uidConfig != null");
        uidConfig.value = uidCounter;
        fishGameDbContext.SaveChanges();

        fishGameDbContext.Dispose();
    }
}