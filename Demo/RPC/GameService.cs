using GameCore.Service;
using MagicOnion;
using MagicOnion.Server;
using Serilog;

namespace GameCore.Service
{
    public class GameService : ServiceBase<IGameService>, IGameService
    {
        // `UnaryResult<T>` allows the method to be treated as `async` method.
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            await Task.CompletedTask;
            // Log.Debug($"Received:{x}, {y}");
            return x + y;
        }
    }
}