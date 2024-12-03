using MagicOnion.Server.Hubs;

namespace GameCore.Service
{
    public class GameHub : StreamingHubBase<IGameHub, IGameHubReceiver>, IGameHub
    {
        public ValueTask SendMessageAsync(string message)
        {
            throw new NotImplementedException();
        }
    }
}