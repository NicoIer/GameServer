using System.Runtime.CompilerServices;
using Serilog;
using UnityToolkit;

namespace FishGame
{
    public class Global : LazySingleton<Global>, IOnInit
    {
        public TypeEventSystem @event { get; private set; }

        private readonly Dictionary<int, ITaskSystem> _taskSystems;
        private readonly List<Task> _tasks;

        private SystemLocator _systemLocator;


        public Global()
        {
            @event = new TypeEventSystem();
            _taskSystems = new Dictionary<int, ITaskSystem>();
            _tasks = new List<Task>();
            _systemLocator = new SystemLocator();
        }

        public void OnInit()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task Run()
        {
            await Task.WhenAll(_tasks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddTask<T>(T system) where T : ITaskSystem
        {
            int id = TypeId<T>.stableId;
            _taskSystems.Add(id, system);
            _tasks.Add(system.Run());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetTask<T>(out T system) where T : ITaskSystem
        {
            int id = TypeId<T>.stableId;
            if (_taskSystems.TryGetValue(id, out var system1))
            {
                system = (T)system1;
                return true;
            }

            system = default!;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<T>(T system) where T : ISystem
        {
            lock (_systemLocator)
            {
                _systemLocator.Register<T>(system);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>() where T : ISystem
        {
            lock (_systemLocator)
            {
                return _systemLocator.Get<T>();
            }
        }
    }
}