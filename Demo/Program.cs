using GameServer.Components;
using System.Text;
using Network.Server;
using UnityToolkit;
using Serilog;
using Serilog.Events;
using Server.Game;

string logPath = $"./log/{DateTime.Now:yyyy-MM-dd}-.txt";
Console.WriteLine(logPath);
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

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSerilog(); // Add this line(Serilog)
builder.Services.AddGrpc(); // Add this line(Grpc.AspNetCore)
builder.Services.AddMagicOnion(); // Add this line(MagicOnion.Server)


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseSerilogRequestLogging(); // Add this line(Serilog)

app.MapMagicOnionService(); // Add this line(MagicOnion.Server)

TelepathyServerSocket socket = new TelepathyServerSocket();
socket.port = 8080;

NetworkServer server = new NetworkServer(socket, 240, true);
server.AddSystem<MyEntitySystem>();
server.AddSystem<NetworkServerTime>();
Task task = server.Run();
app.Run();
task.Dispose();