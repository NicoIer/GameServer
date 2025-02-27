using Microsoft.AspNetCore.Builder;
using Serilog;
using UnityToolkit;

namespace PositionServer;

static class Program
{
    static async Task Main(string[] args)
    {
        ToolkitLog.infoAction = Log.Information;
        ToolkitLog.errorAction = Log.Error;
        ToolkitLog.warningAction = Log.Warning;
        
        
        Launch launch = new Launch(8848,8849);
        await launch.Run();
    }
}