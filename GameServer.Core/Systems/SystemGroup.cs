using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace GameServer.Core.Systems;

public sealed class SystemGroup
{
    private readonly ISystem[] _systems;
    private bool _created;
    private bool _destroyed;

    public SystemGroup(params ISystem[] systems)
    {
        _systems = SortSystems(systems);
    }

    public void OnCreate()
    {
        if (_created)
        {
            return;
        }

        _created = true;
        foreach (ISystem system in _systems)
        {
            system.OnCreate();
        }
    }

    public void Update(in long deltaTimeMs, in int frame, in long timeNowMs)
    {
        foreach (ISystem system in _systems)
        {
            system.Update(in deltaTimeMs, in frame, in timeNowMs);
        }
    }

    public bool TryGetSystem<T>([NotNullWhen(true)] out T? system) where T : class, ISystem
    {
        foreach (ISystem item in _systems)
        {
            if (item.GetType() == typeof(T))
            {
                system = (T)item;
                return true;
            }
        }

        system = null;
        return false;
    }

    public void OnDestroy()
    {
        if (_destroyed)
        {
            return;
        }

        _destroyed = true;
        for (int i = _systems.Length - 1; i >= 0; i--)
        {
            _systems[i].OnDestroy();
        }
    }

    private static ISystem[] SortSystems(ISystem[] systems)
    {
        int count = systems.Length;
        Type[] types = new Type[count];
        Dictionary<Type, int> indexByType = new(count);
        for (int i = 0; i < count; i++)
        {
            Type type = systems[i].GetType();
            if (indexByType.ContainsKey(type))
            {
                throw new InvalidOperationException($"duplicate system type: {type.FullName}");
            }

            types[i] = type;
            indexByType[type] = i;
        }

        var outgoing = new List<int>[count];
        var hasEdge = new bool[count, count];
        var incoming = new int[count];
        for (int i = 0; i < count; i++)
        {
            outgoing[i] = new List<int>();
        }

        for (int i = 0; i < count; i++)
        {
            foreach (ExecuteBeforeAttribute attribute in types[i].GetCustomAttributes<ExecuteBeforeAttribute>())
            {
                if (indexByType.TryGetValue(attribute.Type, out int targetIndex))
                {
                    AddEdge(i, targetIndex);
                }
            }

            foreach (ExecuteAfterAttribute attribute in types[i].GetCustomAttributes<ExecuteAfterAttribute>())
            {
                if (indexByType.TryGetValue(attribute.Type, out int targetIndex))
                {
                    AddEdge(targetIndex, i);
                }
            }
        }

        var sorted = new ISystem[count];
        var used = new bool[count];
        for (int sortedIndex = 0; sortedIndex < count; sortedIndex++)
        {
            int nextIndex = -1;
            for (int i = 0; i < count; i++)
            {
                if (!used[i] && incoming[i] == 0)
                {
                    nextIndex = i;
                    break;
                }
            }

            if (nextIndex < 0)
            {
                throw new InvalidOperationException("system execution order contains cycle");
            }

            used[nextIndex] = true;
            sorted[sortedIndex] = systems[nextIndex];
            foreach (int targetIndex in outgoing[nextIndex])
            {
                incoming[targetIndex]--;
            }
        }

        return sorted;

        void AddEdge(int beforeIndex, int afterIndex)
        {
            if (beforeIndex == afterIndex || hasEdge[beforeIndex, afterIndex])
            {
                return;
            }

            hasEdge[beforeIndex, afterIndex] = true;
            outgoing[beforeIndex].Add(afterIndex);
            incoming[afterIndex]++;
        }
    }
}
