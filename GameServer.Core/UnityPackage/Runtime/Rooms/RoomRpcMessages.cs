using System;
using Friflo.Engine.ECS;

namespace GameServer.Core.Rooms
{
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class RoomRpcContractAttribute : Attribute
    {
        public Type RequiredComponentType { get; }

        public RoomRpcContractAttribute(Type requiredComponentType)
        {
            RequiredComponentType = requiredComponentType;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ServerRpcAttribute : Attribute
    {
        public bool RequiresAuthority { get; set; } = true;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ClientRpcAttribute : Attribute
    {
        public bool IncludeOwner { get; set; } = true;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TargetRpcAttribute : Attribute
    {
    }

    public interface IRoomEntityRpcMessage
    {
        int EntityId { get; }
    }

    public readonly struct RoomServerRpcContext
    {
        public int ConnectionId { get; }
        public long Uid { get; }
        public string RoomId { get; }
        public int EntityId { get; }

        public RoomServerRpcContext(int connectionId, long uid, string roomId, int entityId)
        {
            ConnectionId = connectionId;
            Uid = uid;
            RoomId = roomId;
            EntityId = entityId;
        }
    }

    public delegate void RoomServerRpcHandler<TMessage>(RoomServerRpcContext context, TMessage message)
        where TMessage : IRoomCommand, IRoomEntityRpcMessage;

    public delegate void RoomClientRpcHandler<TMessage>(int entityId, TMessage message)
        where TMessage : IRoomPush, IRoomEntityRpcMessage;

    public interface IRoomServerRpcSender
    {
        void Send<TMessage, TComponent>(TMessage message)
            where TMessage : IRoomCommand, IRoomEntityRpcMessage
            where TComponent : struct, IComponent;
    }

    public interface IRoomClientRpcSender
    {
        void SendObservers<TMessage, TComponent>(TMessage message, bool includeOwner)
            where TMessage : IRoomPush, IRoomEntityRpcMessage
            where TComponent : struct, IComponent;

        void SendTarget<TMessage, TComponent>(int connectionId, TMessage message)
            where TMessage : IRoomPush, IRoomEntityRpcMessage
            where TComponent : struct, IComponent;
    }

    public interface IRoomServerRpcRegistry
    {
        void Register<TMessage, TComponent>(
            bool requiresAuthority,
            RoomServerRpcHandler<TMessage> handler)
            where TMessage : IRoomCommand, IRoomEntityRpcMessage
            where TComponent : struct, IComponent;
    }

    public interface IRoomClientRpcRegistry
    {
        void Register<TMessage, TComponent>(RoomClientRpcHandler<TMessage> handler)
            where TMessage : IRoomPush, IRoomEntityRpcMessage
            where TComponent : struct, IComponent;
    }

    public enum RoomClientRpcDispatchResult
    {
        Applied,
        Unknown,
        EntityUnavailable,
    }
}
