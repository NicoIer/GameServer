namespace GameCore.Jolt
{
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
}