namespace GameCore.Jolt
{

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
}