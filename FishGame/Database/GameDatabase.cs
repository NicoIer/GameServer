// using LiteDB;

using System.Diagnostics;
using Serilog;
using UnityToolkit;

namespace FishGame;

public class GameDatabase : ISystem, IOnInit
{
    public FishGameContext fishGameDbContext { get; private set; }

    public long uidCounter;

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
            fishGameDbContext.configs.Add(new LongConfig { id = nameof(uidCounter), value = uidCounter });
            fishGameDbContext.SaveChanges();
        }
    }


    public void Dispose()
    {
        Log.Information("GameDatabase Dispose");

        // 写回uidCounter
        var uidConfig = fishGameDbContext.configs.Find(nameof(uidCounter));
        Debug.Assert(uidConfig != null, "uidConfig != null");
        uidConfig.value = uidCounter;
        fishGameDbContext.SaveChanges();

        fishGameDbContext.Dispose();
    }
}