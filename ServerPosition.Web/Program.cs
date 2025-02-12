// // var builder = WebApplication.CreateBuilder(args);
// //
// // // Add services to the container.
// // // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// // builder.Services.AddEndpointsApiExplorer();
// // builder.Services.AddSwaggerGen();
// //
// // var app = builder.Build();
// //
// // // Configure the HTTP request pipeline.
// // if (app.Environment.IsDevelopment())
// // {
// //     app.UseSwagger();
// //     app.UseSwaggerUI();
// // }
// //
// // app.UseHttpsRedirection();
// //
// // var summaries = new[]
// // {
// //     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// // };
// //
// // app.MapGet("/weatherforecast", () =>
// //     {
// //         var forecast = Enumerable.Range(1, 5).Select(index =>
// //                 new WeatherForecast
// //                 (
// //                     DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
// //                     Random.Shared.Next(-20, 55),
// //                     summaries[Random.Shared.Next(summaries.Length)]
// //                 ))
// //             .ToArray();
// //         return forecast;
// //     })
// //     .WithName("GetWeatherForecast")
// //     .WithOpenApi();
// //
// // app.Run();
// //
// // record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// // {
// //     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// // }
//
//
// using System.Text;
// using GameCore.Position;
// using Network.Client;
// using Network.Server;
// using Serilog;
// using Serilog.Events;
// using UnityToolkit;
//
//
// string logPath = $"./log/{DateTime.Now:yyyy-MM-dd}-.txt";
// ToolkitLog.writeLog = false; // 取消ToolkitLog的日志写文件\
// ToolkitLog.infoAction = Log.Information; // 用Serilog库的Information方法输出日志
// ToolkitLog.warningAction = Log.Warning; // 用Serilog库的Warning方法输出日志
// ToolkitLog.errorAction = Log.Error; // 用Serilog库的Error方法输出日志
//
//
// // 使用Serilog库配置日志输出
// var loggerConfig = new LoggerConfiguration().MinimumLevel.Debug()
//     .WriteTo.File(
//         logPath,
//         restrictedToMinimumLevel: LogEventLevel.Warning, // 日志输出最低级别
//         outputTemplate: @"{Timestamp:yyyy-MM-dd HH:mm-ss.fff }[{Level:u3}] {Message:lj}{NewLine}{Exception}",
//         rollingInterval: RollingInterval.Day, //日志按天保存
//         rollOnFileSizeLimit: true, // 限制单个文件的最大长度
//         fileSizeLimitBytes: 10 * 1024 * 1024, // 单个文件最大长度10M
//         encoding: Encoding.UTF8, // 文件字符编码
//         retainedFileCountLimit: 1024) // 最大保存文件数,超过最大文件数会自动覆盖原有文件
//     .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information); // 控制台输出日志
//
// Log.Logger = loggerConfig.CreateLogger();
//
// string fullLogPath = System.IO.Path.GetFullPath(logPath);
// Log.Information("Log file path: {fullLogPath}, logPath: {logPath}", fullLogPath, logPath);
//
//
//
// NetworkService.Singleton.Setup(args);
// await NetworkService.Singleton.Run(ICenterService.InternalRpcHost, ICenterService.InternalRpcPort);
Console.WriteLine("Hello, World!");