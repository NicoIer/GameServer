using System.Collections.Concurrent;
using Network;
using Network.Server;

namespace Server.Game
{
    public class MyEntitySystem : EntitySystem
    {
        public static readonly ConcurrentBag<MyEntitySystem> mgrs = new ConcurrentBag<MyEntitySystem>();
        public int ThreadId { get; private set; }
        public static event Action<MyEntitySystem> OnAddServer = delegate { };

        public override void OnInit(NetworkServer t)
        {
            base.OnInit(t);
            mgrs.Add(this);
            lock (OnAddServer)
            {
                OnAddServer(this);
            }
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            componentSerializer.Register<TransformComponent>();
        }
    }
}