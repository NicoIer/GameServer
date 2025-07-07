using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
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
    //     public uint bodyId { get; } // entity id
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

    [Serializable]
    [MemoryPackable]
    public partial struct BodyData
        // : IPhysicsData
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

        public uint objectLayer;

        // public bool allowSleeping;

        public float friction;
        public float restitution;

        public Vector3 position;
        public Quaternion rotation;

        public Vector3 centerOfMass;

        public Vector3 linearVelocity;
        public Vector3 angularVelocity;

        // public MemberMask

        // public IShapeData networkShapeData;
        public ShapeDataPacket? shapeDataPacket;

        [MemoryPackIgnore] private IShapeData _shapeData;

        [MemoryPackIgnore]
        public IShapeData shapeData
        {
            get
            {
                if (_shapeData == null)
                {
                    Debug.Assert(shapeDataPacket.HasValue);
                    _shapeData = ShapeDataPacket.Deserialize(shapeDataPacket.Value);
                }

                return _shapeData;
            }
        }


        // TODO 有时候可以Mask掉shape的数据 因为大多数时候这个都没有变化
        // [Flags]
        // public enum MemberMask : byte
        // {
        //     WithoutShape = 1 << 0,
        //     
        //     All 
        // }
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
// #if UNITY_5_3_OR_NEWER
        [MemoryPackIgnore] public ShapeTypeEnum shapeType => ShapeTypeEnum.Box;
// #endif
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
    public partial struct ShapeDataPacket
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
            var method = typeof(ShapeDataPacket).GetMethod(nameof(Register));
            method = method!.MakeGenericMethod(type);
            method.Invoke(null, null);
        }

        public static void Register<T>() where T : IShapeData
        {
            if (_deserializers.ContainsKey(TypeId<T>.stableId16))
            {
                ToolkitLog.Warning($"NetworkShapeData Register {typeof(T)} Failed, Already Registered");
                return;
            }

            ToolkitLog.Info($"Register NetworkShapeData {typeof(T)}");
            _deserializers.Add(TypeId<T>.stableId16,
                payload => { return MemoryPackSerializer.Deserialize<T>(payload)!; });
        }

        public static IShapeData Deserialize(in ShapeDataPacket dataPacket)
        {
            return _deserializers[dataPacket.id](dataPacket.payload);
        }

        public static void Create<T>(in T shapeData, out ShapeDataPacket dataPacket) where T : IShapeData
        {
            dataPacket.id = TypeId<T>.stableId16;
            dataPacket.payload = MemoryPackSerializer.Serialize(shapeData);
        }
    }

    public enum ShapeTypeEnum : byte
    {
        Box = 1,
        Sphere = 2,
        Plane = 3,
    }

    [MemoryPackable]
    public partial struct BoxShapeData : IShapeData
    {
        [MemoryPackIgnore] public ShapeTypeEnum shapeType => ShapeTypeEnum.Box;

        public Vector3 halfExtents;

        public BoxShapeData(Vector3 halfExtents)
        {
            this.halfExtents = halfExtents;
        }
    }

    [MemoryPackable]
    public partial struct SphereShapeData : IShapeData
    {
        [MemoryPackIgnore] public ShapeTypeEnum shapeType => ShapeTypeEnum.Sphere;
        public float radius;

        public SphereShapeData(float radius)
        {
            this.radius = radius;
        }
    }

    public partial struct PlaneShapeData : IShapeData
    {
        [MemoryPackIgnore] public ShapeTypeEnum shapeType => ShapeTypeEnum.Plane;
        public float halfExtent;
        public Vector3 normal;
        public float distance;

        public PlaneShapeData(float halfExtent, Vector3 normal, float distance)
        {
            this.halfExtent = halfExtent;
            this.normal = normal;
            this.distance = distance;
        }
    }


    public static class ShapeDataExtensions
    {
        public static ShapeDataPacket ToShapeData<T>(this T shapeData) where T : IShapeData
        {
            ShapeDataPacket.Create(shapeData, out var data);
            return data;
        }
    }

    [MemoryPackable]
    public partial struct LockStepData : INetworkMessage
    {
        public long frame;
        public ArraySegment<byte> payload;
    }
}