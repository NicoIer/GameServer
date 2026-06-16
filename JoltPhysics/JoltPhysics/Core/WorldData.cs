// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using MemoryPack;

namespace JoltPhysics
{
    public enum JoltShapeType
    {
        Box = 0,
        Sphere = 1,
        Ellipsoid = 2,
        Mesh = 3,
        Capsule = 4,
        ConvexHull = 5,
    }

    [Serializable]
    [MemoryPackable]
    public partial struct LayerData
    {
        public string name;
        public BroadPhaseLayer broadPhaseLayer;
        public uint collisionMask;
    }

    /// <summary>
    /// Flat shape data with type discriminator.
    /// Only fields relevant to <see cref="type"/> are populated.
    /// </summary>
    [Serializable]
    [MemoryPackable]
    public partial class ShapeData
    {
        public JoltShapeType type;

        // Box
        public Float3 center;
        public Float3 halfExtent;
        public float convexRadius;

        // Sphere
        public float radius;

        // Capsule
        public float halfHeight;

        // Ellipsoid
        public Float3 radii;

        // ConvexHull / Mesh
        public Float3[] vertices;
        public int[] triangles;

        public static ShapeData CreateBox(Float3 center, Float3 halfExtent, float convexRadius = 0.05f)
        {
            return new ShapeData
            {
                type = JoltShapeType.Box,
                center = center,
                halfExtent = halfExtent,
                convexRadius = convexRadius,
            };
        }

        
        public static readonly ShapeData UnitSphere = CreateSphere(0.1f);
        
        public static ShapeData CreateSphere(float radius)
        {
            return new ShapeData
            {
                type = JoltShapeType.Sphere,
                radius = radius,
            };
        }

        public static ShapeData CreateEllipsoid(Float3 radii)
        {
            return new ShapeData
            {
                type = JoltShapeType.Ellipsoid,
                radii = radii,
            };
        }

        public static ShapeData CreateCapsule(float halfHeight, float radius)
        {
            return new ShapeData
            {
                type = JoltShapeType.Capsule,
                halfHeight = halfHeight,
                radius = radius,
            };
        }

        public static ShapeData CreateConvexHull(Float3[] vertices, int[] triangles = null)
        {
            return new ShapeData
            {
                type = JoltShapeType.ConvexHull,
                vertices = vertices,
                triangles = triangles,
            };
        }

        public static ShapeData CreateMesh(Float3[] vertices, int[] triangles)
        {
            return new ShapeData
            {
                type = JoltShapeType.Mesh,
                vertices = vertices,
                triangles = triangles,
            };
        }
    }

    [Serializable]
    [MemoryPackable]
    public partial class BodyData
    {
        public string name;
        public Activation activation;
        public bool enabled;
        public Float3 position;
        public Quat rotation;
        public MotionType motionType;
        public ObjectLayer objectLayer;
        public float friction;
        public float restitution;
        public float linearDamping;
        public float angularDamping;
        public float gravityFactor;
        public ShapeData shape;
    }

    [Serializable]
    [MemoryPackable]
    public partial class WorldData
    {
        public Float3 gravity;
        public uint maxBodies;
        public uint maxContactConstraints;
        public int collisionSteps;
        public LayerData[] layers;
        public ArraySegment<BodyData> bodies;

        public static WorldData CreateDefault()
        {
            return new WorldData
            {
                gravity = new Float3(0, -9.81f, 0),
                maxBodies = 10240,
                maxContactConstraints = 10240,
                collisionSteps = 1,
                layers = new[]
                {
                    new LayerData { name = "Static", broadPhaseLayer = 0, collisionMask = 0b10 },
                    new LayerData { name = "Dynamic", broadPhaseLayer = 1, collisionMask = 0b11 },
                },
                bodies = new ArraySegment<BodyData>(Array.Empty<BodyData>()),
            };
        }
    }
}
