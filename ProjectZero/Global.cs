using Serilog;
using UnityToolkit;

namespace ProjectZero
{
    public class Global : LazySingleton<Global>, IOnInit
    {
        public TypeEventSystem @event { get; private set; }

        public Dictionary<int, ITaskSystem> systems { get; private set; }

        private List<Task> _tasks;
        

        public void OnInit()
        {
            @event = new TypeEventSystem();
            systems = new Dictionary<int, ITaskSystem>();
            _tasks = new List<Task>();
        }

        public async Task Run()
        {
            await Task.WhenAll(_tasks);
        }
        
        public void AddSystem<T>(T system) where T : ITaskSystem
        {
            int id = TypeId<T>.stableId;
            systems.Add(id, system);
            _tasks.Add(system.Run());
        }

        public bool GetSystem<T>(out T system) where T : ITaskSystem
        {
            int id = TypeId<T>.stableId;
            if (systems.TryGetValue(id, out var system1))
            {
                system = (T)system1;
                return true;
            }

            system = default!;
            return false;
        }
    }
}