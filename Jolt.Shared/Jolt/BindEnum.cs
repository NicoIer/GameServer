using System;

namespace GameCore.Jolt
{
    [Flags]
    public enum PhysicsUpdateError
    {
        None = 0,
        ManifoldCacheFull = 1,
        BodyPairCacheFull = 2,
        ContactConstraintsFull = 4,
    }
    
    /// <summary>
    /// This enumerates all shapeData types, each shapeData can return its type through <see cref="ShapeData.SubType"/>
    /// </summary>
    public enum ShapeSubType
    {
        // Convex shapes
        Sphere,
        Box,
        Triangle,
        Capsule,
        TaperedCapsule,
        Cylinder,
        ConvexHull,

        // Compound shapes
        StaticCompound,
        MutableCompound,

        // Decorated shapes
        RotatedTranslated,
        Scaled,
        OffsetCenterOfMass,

        // Other shapes
        Mesh,
        HeightField,
        SoftBody,

        // User defined shapes
        User1,
        User2,
        User3,
        User4,
        User5,
        User6,
        User7,
        User8,

        // User defined convex shapes
        UserConvex1,
        UserConvex2,
        UserConvex3,
        UserConvex4,
        UserConvex5,
        UserConvex6,
        UserConvex7,
        UserConvex8,
    }

    /// <summary>
    /// Shapes are categorized in groups, each shapeData can return which group it belongs to through its <see cref="Type"/> function.
    /// </summary>
    public enum ShapeType
    {
        /// <summary>
        /// Used by <see cref="ConvexShape"/>, all shapes that use the generic convex vs convex collision detection system (box, sphere, capsule, tapered capsule, cylinder, triangle)
        /// </summary>
        Convex,

        /// <summary>
        /// Used by <see cref="CompoundShape"/>
        /// </summary>
        Compound,

        /// <summary>
        /// Used by <see cref="DecoratedShape"/>
        /// </summary>
        Decorated,

        /// <summary>
        /// Used by <see cref="MeshShape"/>
        /// </summary>
        Mesh,

        /// <summary>
        /// Used by <see cref="HeightFieldShape"/>
        /// </summary>
        HeightField,

        /// <summary>
        /// Used by <see cref="SoftBodyShape"/>
        /// </summary>
        SoftBody,

        /// <summary>
        /// User defined shapeData 1
        /// </summary>
        User1,

        /// <summary>
        /// User defined shapeData 2
        /// </summary>
        User2,

        /// <summary>
        /// User defined shapeData 3
        /// </summary>
        User3,

        /// <summary>
        /// User defined shapeData 4
        /// </summary>
        User4,
    }

    public enum BodyType
    {
        Rigid = 0,
        Soft = 1
    }

    public enum MotionType
    {
        Static = 0,
        Kinematic = 1,
        Dynamic = 2,
    }

    public enum Activation
    {
        Activate = 0,
        DontActivate = 1,
    }
}