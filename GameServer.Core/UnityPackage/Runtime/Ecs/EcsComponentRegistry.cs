using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using MemoryPack;
using UnityToolkit;

namespace GameServer.Core.Ecs
{
    public sealed class EcsComponentRegistry
    {
        private readonly Dictionary<ushort, ComponentRegistration> _registrations =
            new Dictionary<ushort, ComponentRegistration>();

        public int Count => _registrations.Count;

        public ushort Register<T>() where T : struct, IComponent
        {
            ushort componentTypeId = TypeId<T>.stableId16;
            if (_registrations.TryGetValue(componentTypeId, out ComponentRegistration current))
            {
                if (current.ComponentType != typeof(T))
                {
                    throw new InvalidOperationException(
                        $"ECS component type id collision id={componentTypeId} first={current.ComponentType.FullName} second={typeof(T).FullName}");
                }

                return componentTypeId;
            }

            _registrations.Add(componentTypeId, new ComponentRegistration(
                typeof(T),
                Deserialize<T>,
                Has<T>,
                Set<T>,
                Remove<T>));
            return componentTypeId;
        }

        public bool Contains(ushort componentTypeId)
        {
            return _registrations.ContainsKey(componentTypeId);
        }

        internal bool TryGet(ushort componentTypeId, out ComponentRegistration registration)
        {
            return _registrations.TryGetValue(componentTypeId, out registration);
        }

        private static object Deserialize<T>(ArraySegment<byte> payload) where T : struct, IComponent
        {
            return MemoryPackSerializer.Deserialize<T>(payload);
        }

        private static bool Has<T>(Entity entity) where T : struct, IComponent
        {
            return entity.HasComponent<T>();
        }

        private static void Set<T>(Entity entity, object value) where T : struct, IComponent
        {
            entity.AddComponent((T)value);
        }

        private static void Remove<T>(Entity entity) where T : struct, IComponent
        {
            entity.RemoveComponent<T>();
        }
    }

    public static class EcsComponentTypeId
    {
        public static ushort Get<T>() where T : struct, IComponent
        {
            return TypeId<T>.stableId16;
        }
    }

    internal sealed class ComponentRegistration
    {
        public readonly Type ComponentType;
        public readonly Func<ArraySegment<byte>, object> Deserialize;
        public readonly Func<Entity, bool> Has;
        public readonly Action<Entity, object> Set;
        public readonly Action<Entity> Remove;

        public ComponentRegistration(
            Type componentType,
            Func<ArraySegment<byte>, object> deserialize,
            Func<Entity, bool> has,
            Action<Entity, object> set,
            Action<Entity> remove)
        {
            ComponentType = componentType;
            Deserialize = deserialize;
            Has = has;
            Set = set;
            Remove = remove;
        }
    }
}
