// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{
    public enum MotionType : uint
    {
        Static = 0,
        Kinematic = 1,
        Dynamic = 2,
    }

    public enum Activation : uint
    {
        Activate = 0,
        DontActivate = 1,
    }

    public enum BodyType : uint
    {
        Rigid = 0,
        Soft = 1,
    }

    public enum MotionQuality : uint
    {
        Discrete = 0,
        LinearCast = 1,
    }

    public enum BackFaceMode : uint
    {
        IgnoreBackFaces = 0,
        CollideWithBackFaces = 1,
    }

    public enum ActiveEdgeMode : uint
    {
        CollideOnlyWithActive = 0,
        CollideWithAll = 1,
    }

    public enum CollectFacesMode : uint
    {
        CollectFaces = 0,
        NoFaces = 1,
    }

    public enum CollisionCollectorType : uint
    {
        AllHit = 0,
        AllHitSorted = 1,
        ClosestHit = 2,
        AnyHit = 3,
    }

    public enum ShapeType : uint
    {
        Convex = 0,
        Compound = 1,
        Decorated = 2,
        Mesh = 3,
        HeightField = 4,
        SoftBody = 5,
    }

    public enum ShapeSubType : uint
    {
        Sphere = 0,
        Box = 1,
        Triangle = 2,
        Capsule = 3,
        TaperedCapsule = 4,
        Cylinder = 5,
        ConvexHull = 6,
        StaticCompound = 7,
        MutableCompound = 8,
        RotatedTranslated = 9,
        Scaled = 10,
        OffsetCenterOfMass = 11,
        Mesh = 12,
        HeightField = 13,
        SoftBody = 14,
    }

    public enum ConstraintType : uint
    {
        Constraint = 0,
        TwoBodyConstraint = 1,
    }

    public enum ConstraintSubType : uint
    {
        Fixed = 0,
        Point = 1,
        Hinge = 2,
        Slider = 3,
        Distance = 4,
        Cone = 5,
        SwingTwist = 6,
        SixDOF = 7,
        Path = 8,
        Vehicle = 9,
        RackAndPinion = 10,
        Gear = 11,
        Pulley = 12,
    }

    public enum ConstraintSpace : uint
    {
        LocalToBodyCOM = 0,
        WorldSpace = 1,
    }

    public enum MotorState : uint
    {
        Off = 0,
        Velocity = 1,
        Position = 2,
    }

    public enum SpringMode : uint
    {
        FrequencyAndDamping = 0,
        StiffnessAndDamping = 1,
    }

    public enum SwingType : uint
    {
        Cone = 0,
        Pyramid = 1,
    }

    public enum SixDOFConstraintAxis : uint
    {
        TranslationX = 0,
        TranslationY = 1,
        TranslationZ = 2,
        RotationX = 3,
        RotationY = 4,
        RotationZ = 5,
    }

    public enum OverrideMassProperties : uint
    {
        CalculateMassAndInertia = 0,
        CalculateInertia = 1,
        MassAndInertiaProvided = 2,
    }

    public enum ValidateResult : uint
    {
        AcceptAllContactsForThisBodyPair = 0,
        AcceptContact = 1,
        RejectContact = 2,
        RejectAllContactsForThisBodyPair = 3,
    }

    public enum SoftBodyBendType : uint
    {
        None = 0,
        Distance = 1,
        Dihedral = 2,
    }
}