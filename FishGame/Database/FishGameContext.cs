using System;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FishGame;

public class FishGameContext : DbContext
{
    public DbSet<User> users { get; set; }
    public DbSet<LongConfig> configs { get; set; }

    public string DbPath { get; }

    public FishGameContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "FishGame.db");
        Log.Information($"FishGameContext db path: {DbPath}");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}"); //TODO 正式环境用MongoDB
}