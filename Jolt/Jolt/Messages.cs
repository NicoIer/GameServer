using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using MemoryPack;
using Network;
using UnityToolkit;


namespace GameCore.Jolt
{
    // public interface INetworkEntity
    // {
    //     /// <summary>
    //     /// 实体拥有者的ID
    //     /// 0表示服务器
    //     /// </summary>
    //     public int ownerId { get; } // player id
    //
    //     public uint entityId { get; } // entity id
    //     public byte worldId { get; } // world id
    // }


    [MemoryPackable]
    public partial struct WorldData : INetworkMessage
    {
        // public byte worldId;
        public long frameCount;
        public long timeStamp;
        public Vector3 gravity;
        public ArraySegment<BodyData> bodies;
    }


    [MemoryPackable]
    public partial struct BodyData
        // : INetworkEntity
    {
        public int ownerId { get; set; } // player id
        public uint entityId { get; set; } // entity id -> jolt bodyId
        // public byte worldId { get; set; } // world id

        public BodyType bodyType;

        [MemoryPackIgnore] public bool isRigid => bodyType == BodyType.Rigid;
        [MemoryPackIgnore] public bool isSoft => bodyType == BodyType.Soft;

        public bool isActive;
        // public bool isStatic;
        // public bool isKinematic;
        // public bool isDynamic;
        
        public MotionType motionType;

        /// <summary>
        /// Same To PhysX isTrigger
        /// </summary>
        public bool isSensor;

        [MemoryPackIgnore] public bool isTrigger => isSensor;

        public ushort objectLayer;

        // public bool allowSleeping;

        public float friction;
        public float restitution;

        public Vector3 position;
        public Quaternion rotation;

        public Vector3 centerOfMass;

        public Vector3 linearVelocity;
        public Vector3 angularVelocity;

        // public IShapeData shapeData;
        public ShapeData shapeData;
    }

    // <!-- output memorypack serialization info to directory -->
    //     <ItemGroup>
    //     <CompilerVisibleProperty Include="MemoryPackGenerator_SerializationInfoOutputDirectory" />
    //     </ItemGroup>
    //     <PropertyGroup>
    //     <MemoryPackGenerator_SerializationInfoOutputDirectory>$(MSBuildProjectDirectory)\MemoryPackLogs</MemoryPackGenerator_SerializationInfoOutputDirectory>
    //     </PropertyGroup>

    // [MemoryPackable]
    // [MemoryPackUnion(0, typeof(BoxShapeData))]
    // [MemoryPackUnion(1, typeof(SphereShapeData))]
    public partial interface IShapeData
    {
        // public ShapeType type;
        // public ShapeSubType subType;
        // public float innerRadius;
        // public Vector3 scale;

        // public float volume;

        // public Vector3 centerOfMass;
        // public BoundingBox boundingBox;
        // public ArraySegment<byte> data;
    }

    [MemoryPackable]
    public partial struct ShapeData
    {
        public ushort id;
        public ArraySegment<byte> payload;


        private static readonly Dictionary<ushort, Func<ArraySegment<byte>, IShapeData>> _deserializers =
            new Dictionary<ushort, Func<ArraySegment<byte>, IShapeData>>();
        
        public static bool registered => _deserializers.Count > 0;
        public static void RegisterAll()
        {
            // 通过反射 找到本程序集下所有的 IShapeData 类型
            var types = typeof(IShapeData).Assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsInterface || type.IsAbstract) continue;
                if (type.GetInterface(nameof(IShapeData)) == null) continue;
                RegisterType(type);
            }
        }

        private static void RegisterType(Type type)
        {
            var method = typeof(ShapeData).GetMethod(nameof(Register));
            method = method!.MakeGenericMethod(type);
            method.Invoke(null, null);
        }

        public static void Register<T>() where T : IShapeData
        {
            if (_deserializers.ContainsKey(TypeId<T>.stableId16))
            {
                ToolkitLog.Warning($"ShapeData Register {typeof(T)} Failed, Already Registered");
                return;
            }

            ToolkitLog.Info($"Register ShapeData {typeof(T)}");
            _deserializers.Add(TypeId<T>.stableId16,
                payload => { return MemoryPackSerializer.Deserialize<T>(payload)!; });
        }

        public static IShapeData Revert(in ShapeData data)
        {
            return _deserializers[data.id](data.payload);
        }

        public static void Create<T>(T shapeData, out ShapeData data) where T : IShapeData
        {
            data.id = TypeId<T>.stableId16;
            data.payload = MemoryPackSerializer.Serialize(shapeData);
        }
    }

    [MemoryPackable]
    public partial struct BoxShapeData : IShapeData
    {
        public Vector3 halfExtents;

        public BoxShapeData(Vector3 halfExtents)
        {
            this.halfExtents = halfExtents;
        }
    }

    [MemoryPackable]
    public partial struct SphereShapeData : IShapeData
    {
        public float radius;

        public SphereShapeData(float radius)
        {
            this.radius = radius;
        }
    }
    
    public partial struct PlaneShapeData : IShapeData
    {
        public float halfExtent;
        public Vector3 normal;
        public float distance;
    }
    
    

    public static class ShapeDataExtensions
    {
        public static ShapeData ToShapeData<T>(this T shapeData) where T : IShapeData
        {
            ShapeData.Create(shapeData, out var data);
            return data;
        }
    }
}