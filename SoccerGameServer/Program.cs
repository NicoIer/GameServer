// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.Json.Serialization;
using GameCore.Physics;
using JoltPhysicsSharp;
using JoltServer;
using MemoryPack;
using Network;
using Network.Time;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Soccer;
using UnityToolkit;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;


string logPath = $"./log/{DateTime.Now:yyyy-MM-dd}-.txt";
ToolkitLog.writeLog = false; // 取消ToolkitLog的日志写文件\
ToolkitLog.infoAction = Log.Information; // 用Serilog库的Information方法输出日志
ToolkitLog.warningAction = Log.Warning; // 用Serilog库的Warning方法输出日志
ToolkitLog.errorAction = Log.Error; // 用Serilog库的Error方法输出日志


// 使用Serilog库配置日志输出
var loggerConfig = new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File(
        logPath,
        restrictedToMinimumLevel: LogEventLevel.Warning, // 日志输出最低级别
        outputTemplate: @"{Timestamp:yyyy-MM-dd HH:mm-ss.fff }[{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day, //日志按天保存
        rollOnFileSizeLimit: true, // 限制单个文件的最大长度
        fileSizeLimitBytes: 10 * 1024 * 1024, // 单个文件最大长度10M
        encoding: Encoding.UTF8, // 文件字符编码
        retainedFileCountLimit: 1024) // 最大保存文件数,超过最大文件数会自动覆盖原有文件
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information); // 控制台输出日志

Log.Logger = loggerConfig.CreateLogger();

ToolkitLog.infoAction = Log.Information;
ToolkitLog.warningAction = Log.Warning;
ToolkitLog.errorAction = Log.Error;

RunCfg cfg = new RunCfg();
if (args.Length != 0)
{
    cfg = JsonConvert.DeserializeObject<RunCfg>(File.ReadAllText(args[0]));
}

var app = new JoltApplication();

var gameServer = new SoccerGameServer(cfg.gameFrameRate, cfg.gamePort);
app.AddSystem(gameServer);

LocalNetwork localNetwork = new LocalNetwork(cfg.broadcastPort, ServerInfoGenerator, null, cfg.networkFrameRate);

bool ServerInfoGenerator(out byte[] data)
{
    return gameServer.GetServerInfo(out data);
}

_ = localNetwork.Start();
app.Run();
localNetwork.Stop();
// timeServer.Stop();