using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace JoltPhysics
{
    internal partial struct JPH_BroadPhaseLayerInterface
    {
    }

    internal partial struct JPH_ObjectVsBroadPhaseLayerFilter
    {
    }

    internal partial struct JPH_ObjectLayerPairFilter
    {
    }

    internal partial struct JPH_BroadPhaseLayerFilter
    {
    }

    internal partial struct JPH_ObjectLayerFilter
    {
    }

    internal partial struct JPH_BodyFilter
    {
    }

    internal partial struct JPH_ShapeFilter
    {
    }

    internal partial struct JPH_SimShapeFilter
    {
    }

    internal partial struct JPH_PhysicsStepListener
    {
    }

    internal partial struct JPH_PhysicsSystem
    {
    }

    internal partial struct JPH_PhysicsMaterial
    {
    }

    internal partial struct JPH_LinearCurve
    {
    }

    internal partial struct JPH_ShapeSettings
    {
    }

    internal partial struct JPH_ConvexShapeSettings
    {
    }

    internal partial struct JPH_SphereShapeSettings
    {
    }

    internal partial struct JPH_BoxShapeSettings
    {
    }

    internal partial struct JPH_PlaneShapeSettings
    {
    }

    internal partial struct JPH_TriangleShapeSettings
    {
    }

    internal partial struct JPH_CapsuleShapeSettings
    {
    }

    internal partial struct JPH_TaperedCapsuleShapeSettings
    {
    }

    internal partial struct JPH_CylinderShapeSettings
    {
    }

    internal partial struct JPH_TaperedCylinderShapeSettings
    {
    }

    internal partial struct JPH_ConvexHullShapeSettings
    {
    }

    internal partial struct JPH_CompoundShapeSettings
    {
    }

    internal partial struct JPH_StaticCompoundShapeSettings
    {
    }

    internal partial struct JPH_MutableCompoundShapeSettings
    {
    }

    internal partial struct JPH_MeshShapeSettings
    {
    }

    internal partial struct JPH_HeightFieldShapeSettings
    {
    }

    internal partial struct JPH_RotatedTranslatedShapeSettings
    {
    }

    internal partial struct JPH_ScaledShapeSettings
    {
    }

    internal partial struct JPH_OffsetCenterOfMassShapeSettings
    {
    }

    internal partial struct JPH_EmptyShapeSettings
    {
    }

    internal partial struct JPH_Shape
    {
    }

    internal partial struct JPH_ConvexShape
    {
    }

    internal partial struct JPH_SphereShape
    {
    }

    internal partial struct JPH_BoxShape
    {
    }

    internal partial struct JPH_PlaneShape
    {
    }

    internal partial struct JPH_CapsuleShape
    {
    }

    internal partial struct JPH_CylinderShape
    {
    }

    internal partial struct JPH_TaperedCylinderShape
    {
    }

    internal partial struct JPH_TriangleShape
    {
    }

    internal partial struct JPH_TaperedCapsuleShape
    {
    }

    internal partial struct JPH_ConvexHullShape
    {
    }

    internal partial struct JPH_CompoundShape
    {
    }

    internal partial struct JPH_StaticCompoundShape
    {
    }

    internal partial struct JPH_MutableCompoundShape
    {
    }

    internal partial struct JPH_MeshShape
    {
    }

    internal partial struct JPH_HeightFieldShape
    {
    }

    internal partial struct JPH_DecoratedShape
    {
    }

    internal partial struct JPH_RotatedTranslatedShape
    {
    }

    internal partial struct JPH_ScaledShape
    {
    }

    internal partial struct JPH_OffsetCenterOfMassShape
    {
    }

    internal partial struct JPH_EmptyShape
    {
    }

    internal partial struct JPH_BodyCreationSettings
    {
    }

    internal partial struct JPH_SoftBodyCreationSettings
    {
    }

    internal partial struct JPH_SoftBodySharedSettings
    {
    }

    internal partial struct JPH_BodyInterface
    {
    }

    internal partial struct JPH_BodyLockInterface
    {
    }

    internal partial struct JPH_BroadPhaseQuery
    {
    }

    internal partial struct JPH_NarrowPhaseQuery
    {
    }

    internal partial struct JPH_MotionProperties
    {
    }

    internal partial struct JPH_Body
    {
    }

    internal partial struct JPH_ContactListener
    {
    }

    internal partial struct JPH_ContactManifold
    {
    }

    internal partial struct JPH_GroupFilter
    {
    }

    internal partial struct JPH_GroupFilterTable
    {
    }

    internal partial struct JPH_TempAllocator
    {
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_PhysicsUpdateError : uint
    {
        JPH_PhysicsUpdateError_None = 0,
        JPH_PhysicsUpdateError_ManifoldCacheFull = 1 << 0,
        JPH_PhysicsUpdateError_BodyPairCacheFull = 1 << 1,
        JPH_PhysicsUpdateError_ContactConstraintsFull = 1 << 2,
        _JPH_PhysicsUpdateError_Count,
        _JPH_PhysicsUpdateError_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_BodyType : uint
    {
        JPH_BodyType_Rigid = 0,
        JPH_BodyType_Soft = 1,
        _JPH_BodyType_Count,
        _JPH_BodyType_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_MotionType : uint
    {
        JPH_MotionType_Static = 0,
        JPH_MotionType_Kinematic = 1,
        JPH_MotionType_Dynamic = 2,
        _JPH_MotionType_Count,
        _JPH_MotionType_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_Activation : uint
    {
        JPH_Activation_Activate = 0,
        JPH_Activation_DontActivate = 1,
        _JPH_Activation_Count,
        _JPH_Activation_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_ValidateResult : uint
    {
        JPH_ValidateResult_AcceptAllContactsForThisBodyPair = 0,
        JPH_ValidateResult_AcceptContact = 1,
        JPH_ValidateResult_RejectContact = 2,
        JPH_ValidateResult_RejectAllContactsForThisBodyPair = 3,
        _JPH_ValidateResult_Count,
        _JPH_ValidateResult_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_ShapeType : uint
    {
        JPH_ShapeType_Convex = 0,
        JPH_ShapeType_Compound = 1,
        JPH_ShapeType_Decorated = 2,
        JPH_ShapeType_Mesh = 3,
        JPH_ShapeType_HeightField = 4,
        JPH_ShapeType_SoftBody = 5,
        JPH_ShapeType_User1 = 6,
        JPH_ShapeType_User2 = 7,
        JPH_ShapeType_User3 = 8,
        JPH_ShapeType_User4 = 9,
        _JPH_ShapeType_Count,
        _JPH_ShapeType_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_ShapeSubType : uint
    {
        JPH_ShapeSubType_Sphere = 0,
        JPH_ShapeSubType_Box = 1,
        JPH_ShapeSubType_Triangle = 2,
        JPH_ShapeSubType_Capsule = 3,
        JPH_ShapeSubType_TaperedCapsule = 4,
        JPH_ShapeSubType_Cylinder = 5,
        JPH_ShapeSubType_ConvexHull = 6,
        JPH_ShapeSubType_StaticCompound = 7,
        JPH_ShapeSubType_MutableCompound = 8,
        JPH_ShapeSubType_RotatedTranslated = 9,
        JPH_ShapeSubType_Scaled = 10,
        JPH_ShapeSubType_OffsetCenterOfMass = 11,
        JPH_ShapeSubType_Mesh = 12,
        JPH_ShapeSubType_HeightField = 13,
        JPH_ShapeSubType_SoftBody = 14,
        _JPH_ShapeSubType_Count,
        _JPH_ShapeSubType_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_ConstraintType : uint
    {
        JPH_ConstraintType_Constraint = 0,
        JPH_ConstraintType_TwoBodyConstraint = 1,
        _JPH_ConstraintType_Count,
        _JPH_ConstraintType_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_ConstraintSubType : uint
    {
        JPH_ConstraintSubType_Fixed = 0,
        JPH_ConstraintSubType_Point = 1,
        JPH_ConstraintSubType_Hinge = 2,
        JPH_ConstraintSubType_Slider = 3,
        JPH_ConstraintSubType_Distance = 4,
        JPH_ConstraintSubType_Cone = 5,
        JPH_ConstraintSubType_SwingTwist = 6,
        JPH_ConstraintSubType_SixDOF = 7,
        JPH_ConstraintSubType_Path = 8,
        JPH_ConstraintSubType_Vehicle = 9,
        JPH_ConstraintSubType_RackAndPinion = 10,
        JPH_ConstraintSubType_Gear = 11,
        JPH_ConstraintSubType_Pulley = 12,
        JPH_ConstraintSubType_User1 = 13,
        JPH_ConstraintSubType_User2 = 14,
        JPH_ConstraintSubType_User3 = 15,
        JPH_ConstraintSubType_User4 = 16,
        _JPH_ConstraintSubType_Count,
        _JPH_ConstraintSubType_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_ConstraintSpace : uint
    {
        JPH_ConstraintSpace_LocalToBodyCOM = 0,
        JPH_ConstraintSpace_WorldSpace = 1,
        _JPH_ConstraintSpace_Count,
        _JPH_ConstraintSpace_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_MotionQuality : uint
    {
        JPH_MotionQuality_Discrete = 0,
        JPH_MotionQuality_LinearCast = 1,
        _JPH_MotionQuality_Count,
        _JPH_MotionQuality_Force32 = 0x7fffffff,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_OverrideMassProperties : uint
    {
        JPH_OverrideMassProperties_CalculateMassAndInertia,
        JPH_OverrideMassProperties_CalculateInertia,
        JPH_OverrideMassProperties_MassAndInertiaProvided,
        _JPH_JPH_OverrideMassProperties_Count,
        _JPH_JPH_OverrideMassProperties_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_AllowedDOFs : uint
    {
        JPH_AllowedDOFs_All = 0b111111,
        JPH_AllowedDOFs_TranslationX = 0b000001,
        JPH_AllowedDOFs_TranslationY = 0b000010,
        JPH_AllowedDOFs_TranslationZ = 0b000100,
        JPH_AllowedDOFs_RotationX = 0b001000,
        JPH_AllowedDOFs_RotationY = 0b010000,
        JPH_AllowedDOFs_RotationZ = 0b100000,
        JPH_AllowedDOFs_Plane2D = JPH_AllowedDOFs_TranslationX | JPH_AllowedDOFs_TranslationY | JPH_AllowedDOFs_RotationZ,
        _JPH_AllowedDOFs_Count,
        _JPH_AllowedDOFs_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_GroundState : uint
    {
        JPH_GroundState_OnGround = 0,
        JPH_GroundState_OnSteepGround = 1,
        JPH_GroundState_NotSupported = 2,
        JPH_GroundState_InAir = 3,
        _JPH_GroundState_Count,
        _JPH_GroundState_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_BackFaceMode : uint
    {
        JPH_BackFaceMode_IgnoreBackFaces,
        JPH_BackFaceMode_CollideWithBackFaces,
        _JPH_BackFaceMode_Count,
        _JPH_BackFaceMode_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_ActiveEdgeMode : uint
    {
        JPH_ActiveEdgeMode_CollideOnlyWithActive,
        JPH_ActiveEdgeMode_CollideWithAll,
        _JPH_ActiveEdgeMode_Count,
        _JPH_ActiveEdgeMode_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_CollectFacesMode : uint
    {
        JPH_CollectFacesMode_CollectFaces,
        JPH_CollectFacesMode_NoFaces,
        _JPH_CollectFacesMode_Count,
        _JPH_CollectFacesMode_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_MotorState : uint
    {
        JPH_MotorState_Off = 0,
        JPH_MotorState_Velocity = 1,
        JPH_MotorState_Position = 2,
        _JPH_MotorState_Count,
        _JPH_MotorState_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_CollisionCollectorType : uint
    {
        JPH_CollisionCollectorType_AllHit = 0,
        JPH_CollisionCollectorType_AllHitSorted = 1,
        JPH_CollisionCollectorType_ClosestHit = 2,
        JPH_CollisionCollectorType_AnyHit = 3,
        _JPH_CollisionCollectorType_Count,
        _JPH_CollisionCollectorType_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_SwingType : uint
    {
        JPH_SwingType_Cone,
        JPH_SwingType_Pyramid,
        _JPH_SwingType_Count,
        _JPH_SwingType_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_SixDOFConstraintAxis : uint
    {
        JPH_SixDOFConstraintAxis_TranslationX,
        JPH_SixDOFConstraintAxis_TranslationY,
        JPH_SixDOFConstraintAxis_TranslationZ,
        JPH_SixDOFConstraintAxis_RotationX,
        JPH_SixDOFConstraintAxis_RotationY,
        JPH_SixDOFConstraintAxis_RotationZ,
        _JPH_SixDOFConstraintAxis_Num,
        _JPH_SixDOFConstraintAxis_NumTranslation = JPH_SixDOFConstraintAxis_TranslationZ + 1,
        _JPH_SixDOFConstraintAxis_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_SpringMode : uint
    {
        JPH_SpringMode_FrequencyAndDamping = 0,
        JPH_SpringMode_StiffnessAndDamping = 1,
        _JPH_SpringMode_Count,
        _JPH_SpringMode_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_SoftBodyConstraintColor : uint
    {
        JPH_SoftBodyConstraintColor_ConstraintType,
        JPH_SoftBodyConstraintColor_ConstraintGroup,
        JPH_SoftBodyConstraintColor_ConstraintOrder,
        _JPH_SoftBodyConstraintColor_Count,
        _JPH_SoftBodyConstraintColor_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_SoftBodyBendType : uint
    {
        JPH_SoftBodyBendType_None,
        JPH_SoftBodyBendType_Distance,
        JPH_SoftBodyBendType_Dihedral,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_BodyManager_ShapeColor : uint
    {
        JPH_BodyManager_ShapeColor_InstanceColor,
        JPH_BodyManager_ShapeColor_ShapeTypeColor,
        JPH_BodyManager_ShapeColor_MotionTypeColor,
        JPH_BodyManager_ShapeColor_SleepColor,
        JPH_BodyManager_ShapeColor_IslandColor,
        JPH_BodyManager_ShapeColor_MaterialColor,
        _JPH_BodyManager_ShapeColor_Count,
        _JPH_BodyManager_ShapeColor_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_DebugRenderer_CastShadow : uint
    {
        JPH_DebugRenderer_CastShadow_On = 0,
        JPH_DebugRenderer_CastShadow_Off = 1,
        _JPH_DebugRenderer_CastShadow_Count,
        _JPH_DebugRenderer_CastShadow_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_DebugRenderer_DrawMode : uint
    {
        JPH_DebugRenderer_DrawMode_Solid = 0,
        JPH_DebugRenderer_DrawMode_Wireframe = 1,
        _JPH_DebugRenderer_DrawMode_Count,
        _JPH_DebugRenderer_DrawMode_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_Mesh_Shape_BuildQuality : uint
    {
        JPH_Mesh_Shape_BuildQuality_FavorRuntimePerformance = 0,
        JPH_Mesh_Shape_BuildQuality_FavorBuildSpeed = 1,
        _JPH_Mesh_Shape_BuildQuality_Count,
        _JPH_Mesh_Shape_BuildQuality_Force32 = 0x7FFFFFFF,
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_TransmissionMode : uint
    {
        JPH_TransmissionMode_Auto = 0,
        JPH_TransmissionMode_Manual = 1,
        _JPH_TransmissionMode_Count,
        _JPH_TransmissionMode_Force32 = 0x7FFFFFFF,
    }

    internal partial struct JPH_Vec3
    {
        public float x;

        public float y;

        public float z;
    }

    internal partial struct JPH_Vec4
    {
        public float x;

        public float y;

        public float z;

        public float w;
    }

    internal partial struct JPH_Quat
    {
        public float x;

        public float y;

        public float z;

        public float w;
    }

    internal partial struct JPH_Plane
    {
        public JPH_Vec3 normal;

        public float distance;
    }

    internal partial struct JPH_Mat4
    {
        public JPH_Vec4 Column0;

        public JPH_Vec4 Column1;

        public JPH_Vec4 Column2;

        public JPH_Vec4 Column3;
    }

    internal partial struct JPH_Point
    {
        public float x;

        public float y;
    }

    internal partial struct JPH_AABox
    {
        public JPH_Vec3 min;

        public JPH_Vec3 max;
    }

    internal partial struct JPH_Triangle
    {
        public JPH_Vec3 v1;

        public JPH_Vec3 v2;

        public JPH_Vec3 v3;

        [NativeTypeName("uint32_t")]
        public uint materialIndex;
    }

    internal partial struct JPH_IndexedTriangleNoMaterial
    {
        [NativeTypeName("uint32_t")]
        public uint i1;

        [NativeTypeName("uint32_t")]
        public uint i2;

        [NativeTypeName("uint32_t")]
        public uint i3;
    }

    internal partial struct JPH_IndexedTriangle
    {
        [NativeTypeName("uint32_t")]
        public uint i1;

        [NativeTypeName("uint32_t")]
        public uint i2;

        [NativeTypeName("uint32_t")]
        public uint i3;

        [NativeTypeName("uint32_t")]
        public uint materialIndex;

        [NativeTypeName("uint32_t")]
        public uint userData;
    }

    internal partial struct JPH_MassProperties
    {
        public float mass;

        public JPH_Mat4 inertia;
    }

    internal partial struct JPH_SoftVertex
    {
        public JPH_Vec3 position;

        public JPH_Vec3 velocity;

        public float invMass;
    }

    internal partial struct JPH_SoftFace
    {
        [NativeTypeName("uint32_t")]
        public uint vertex1;

        [NativeTypeName("uint32_t")]
        public uint vertex2;

        [NativeTypeName("uint32_t")]
        public uint vertex3;

        [NativeTypeName("uint32_t")]
        public uint materialIndex;
    }

    internal partial struct JPH_ContactSettings
    {
        public float combinedFriction;

        public float combinedRestitution;

        public float invMassScale1;

        public float invInertiaScale1;

        public float invMassScale2;

        public float invInertiaScale2;

        [NativeTypeName("JPH_Bool")]
        public uint isSensor;

        public JPH_Vec3 relativeLinearSurfaceVelocity;

        public JPH_Vec3 relativeAngularSurfaceVelocity;
    }

    internal partial struct JPH_CollideSettingsBase
    {
        public JPH_ActiveEdgeMode activeEdgeMode;

        public JPH_CollectFacesMode collectFacesMode;

        public float collisionTolerance;

        public float penetrationTolerance;

        public JPH_Vec3 activeEdgeMovementDirection;
    }

    internal partial struct JPH_CollideShapeSettings
    {
        public JPH_CollideSettingsBase @base;

        public float maxSeparationDistance;

        public JPH_BackFaceMode backFaceMode;
    }

    internal partial struct JPH_ShapeCastSettings
    {
        public JPH_CollideSettingsBase @base;

        public JPH_BackFaceMode backFaceModeTriangles;

        public JPH_BackFaceMode backFaceModeConvex;

        [NativeTypeName("bool")]
        public byte useShrunkenShapeAndConvexRadius;

        [NativeTypeName("bool")]
        public byte returnDeepestPoint;
    }

    internal partial struct JPH_RayCastSettings
    {
        public JPH_BackFaceMode backFaceModeTriangles;

        public JPH_BackFaceMode backFaceModeConvex;

        [NativeTypeName("bool")]
        public byte treatConvexAsSolid;
    }

    internal partial struct JPH_SpringSettings
    {
        public JPH_SpringMode mode;

        public float frequencyOrStiffness;

        public float damping;
    }

    internal partial struct JPH_MotorSettings
    {
        public JPH_SpringSettings springSettings;

        public float minForceLimit;

        public float maxForceLimit;

        public float minTorqueLimit;

        public float maxTorqueLimit;
    }

    internal partial struct JPH_SubShapeIDPair
    {
        [NativeTypeName("JPH_BodyID")]
        public uint Body1ID;

        [NativeTypeName("JPH_SubShapeID")]
        public uint subShapeID1;

        [NativeTypeName("JPH_BodyID")]
        public uint Body2ID;

        [NativeTypeName("JPH_SubShapeID")]
        public uint subShapeID2;
    }

    internal partial struct JPH_BroadPhaseCastResult
    {
        [NativeTypeName("JPH_BodyID")]
        public uint bodyID;

        public float fraction;
    }

    internal partial struct JPH_RayCastResult
    {
        [NativeTypeName("JPH_BodyID")]
        public uint bodyID;

        public float fraction;

        [NativeTypeName("JPH_SubShapeID")]
        public uint subShapeID2;
    }

    internal partial struct JPH_CollidePointResult
    {
        [NativeTypeName("JPH_BodyID")]
        public uint bodyID;

        [NativeTypeName("JPH_SubShapeID")]
        public uint subShapeID2;
    }

    internal unsafe partial struct JPH_CollideShapeResult
    {
        public JPH_Vec3 contactPointOn1;

        public JPH_Vec3 contactPointOn2;

        public JPH_Vec3 penetrationAxis;

        public float penetrationDepth;

        [NativeTypeName("JPH_SubShapeID")]
        public uint subShapeID1;

        [NativeTypeName("JPH_SubShapeID")]
        public uint subShapeID2;

        [NativeTypeName("JPH_BodyID")]
        public uint bodyID2;

        [NativeTypeName("uint32_t")]
        public uint shape1FaceCount;

        public JPH_Vec3* shape1Faces;

        [NativeTypeName("uint32_t")]
        public uint shape2FaceCount;

        public JPH_Vec3* shape2Faces;
    }

    internal partial struct JPH_ShapeCastResult
    {
        public JPH_Vec3 contactPointOn1;

        public JPH_Vec3 contactPointOn2;

        public JPH_Vec3 penetrationAxis;

        public float penetrationDepth;

        [NativeTypeName("JPH_SubShapeID")]
        public uint subShapeID1;

        [NativeTypeName("JPH_SubShapeID")]
        public uint subShapeID2;

        [NativeTypeName("JPH_BodyID")]
        public uint bodyID2;

        public float fraction;

        [NativeTypeName("bool")]
        public byte isBackFaceHit;
    }

    internal partial struct JPH_DrawSettings
    {
        [NativeTypeName("bool")]
        public byte drawGetSupportFunction;

        [NativeTypeName("bool")]
        public byte drawSupportDirection;

        [NativeTypeName("bool")]
        public byte drawGetSupportingFace;

        [NativeTypeName("bool")]
        public byte drawShape;

        [NativeTypeName("bool")]
        public byte drawShapeWireframe;

        public JPH_BodyManager_ShapeColor drawShapeColor;

        [NativeTypeName("bool")]
        public byte drawBoundingBox;

        [NativeTypeName("bool")]
        public byte drawCenterOfMassTransform;

        [NativeTypeName("bool")]
        public byte drawWorldTransform;

        [NativeTypeName("bool")]
        public byte drawVelocity;

        [NativeTypeName("bool")]
        public byte drawMassAndInertia;

        [NativeTypeName("bool")]
        public byte drawSleepStats;

        [NativeTypeName("bool")]
        public byte drawSoftBodyVertices;

        [NativeTypeName("bool")]
        public byte drawSoftBodyVertexVelocities;

        [NativeTypeName("bool")]
        public byte drawSoftBodyEdgeConstraints;

        [NativeTypeName("bool")]
        public byte drawSoftBodyBendConstraints;

        [NativeTypeName("bool")]
        public byte drawSoftBodyVolumeConstraints;

        [NativeTypeName("bool")]
        public byte drawSoftBodySkinConstraints;

        [NativeTypeName("bool")]
        public byte drawSoftBodyLRAConstraints;

        [NativeTypeName("bool")]
        public byte drawSoftBodyPredictedBounds;

        public JPH_SoftBodyConstraintColor drawSoftBodyConstraintColor;
    }

    internal partial struct JPH_SupportingFace
    {
        [NativeTypeName("uint32_t")]
        public uint count;

        [NativeTypeName("JPH_Vec3[32]")]
        public _vertices_e__FixedBuffer vertices;

        [InlineArray(32)]
        public partial struct _vertices_e__FixedBuffer
        {
            public JPH_Vec3 e0;
        }
    }

    internal unsafe partial struct JPH_CollisionGroup
    {
        [NativeTypeName("const JPH_GroupFilter *")]
        public JPH_GroupFilter* groupFilter;

        [NativeTypeName("JPH_CollisionGroupID")]
        public uint groupID;

        [NativeTypeName("JPH_CollisionSubGroupID")]
        public uint subGroupID;
    }

    internal unsafe partial struct JPH_CollisionEstimationResult
    {
        public JPH_Vec3 linearVelocity1;

        public JPH_Vec3 angularVelocity1;

        public JPH_Vec3 linearVelocity2;

        public JPH_Vec3 angularVelocity2;

        public JPH_Vec3 frictionPoint;

        public JPH_Vec3 tangent1;

        public JPH_Vec3 tangent2;

        public float frictionImpulse1;

        public float frictionImpulse2;

        public float angularFrictionImpulse;

        [NativeTypeName("uint32_t")]
        public uint contactImpulseCount;

        public float* contactImpulses;
    }

    internal partial struct JPH_BodyActivationListener
    {
    }

    internal partial struct JPH_BodyDrawFilter
    {
    }

    internal partial struct JPH_SharedMutex
    {
    }

    internal partial struct JPH_DebugRenderer
    {
    }

    internal partial struct JPH_Constraint
    {
    }

    internal partial struct JPH_TwoBodyConstraint
    {
    }

    internal partial struct JPH_FixedConstraint
    {
    }

    internal partial struct JPH_DistanceConstraint
    {
    }

    internal partial struct JPH_PointConstraint
    {
    }

    internal partial struct JPH_HingeConstraint
    {
    }

    internal partial struct JPH_SliderConstraint
    {
    }

    internal partial struct JPH_ConeConstraint
    {
    }

    internal partial struct JPH_SwingTwistConstraint
    {
    }

    internal partial struct JPH_SixDOFConstraint
    {
    }

    internal partial struct JPH_GearConstraint
    {
    }

    internal partial struct JPH_CharacterBase
    {
    }

    internal partial struct JPH_Character
    {
    }

    internal partial struct JPH_CharacterVirtual
    {
    }

    internal partial struct JPH_CharacterContactListener
    {
    }

    internal partial struct JPH_CharacterVsCharacterCollision
    {
    }

    internal partial struct JPH_Skeleton
    {
    }

    internal partial struct JPH_SkeletonPose
    {
    }

    internal partial struct JPH_SkeletalAnimation
    {
    }

    internal partial struct JPH_SkeletonMapper
    {
    }

    internal partial struct JPH_RagdollSettings
    {
    }

    internal partial struct JPH_Ragdoll
    {
    }

    internal partial struct JPH_ConstraintSettings
    {
        [NativeTypeName("bool")]
        public byte enabled;

        [NativeTypeName("uint32_t")]
        public uint constraintPriority;

        [NativeTypeName("uint32_t")]
        public uint numVelocityStepsOverride;

        [NativeTypeName("uint32_t")]
        public uint numPositionStepsOverride;

        public float drawConstraintSize;

        [NativeTypeName("uint64_t")]
        public ulong userData;
    }

    internal partial struct JPH_FixedConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_ConstraintSpace space;

        [NativeTypeName("bool")]
        public byte autoDetectPoint;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point1;

        public JPH_Vec3 axisX1;

        public JPH_Vec3 axisY1;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point2;

        public JPH_Vec3 axisX2;

        public JPH_Vec3 axisY2;
    }

    internal partial struct JPH_DistanceConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_ConstraintSpace space;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point1;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point2;

        public float minDistance;

        public float maxDistance;

        public JPH_SpringSettings limitsSpringSettings;
    }

    internal partial struct JPH_PointConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_ConstraintSpace space;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point1;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point2;
    }

    internal partial struct JPH_HingeConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_ConstraintSpace space;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point1;

        public JPH_Vec3 hingeAxis1;

        public JPH_Vec3 normalAxis1;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point2;

        public JPH_Vec3 hingeAxis2;

        public JPH_Vec3 normalAxis2;

        public float limitsMin;

        public float limitsMax;

        public JPH_SpringSettings limitsSpringSettings;

        public float maxFrictionTorque;

        public JPH_MotorSettings motorSettings;
    }

    internal partial struct JPH_SliderConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_ConstraintSpace space;

        [NativeTypeName("bool")]
        public byte autoDetectPoint;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point1;

        public JPH_Vec3 sliderAxis1;

        public JPH_Vec3 normalAxis1;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point2;

        public JPH_Vec3 sliderAxis2;

        public JPH_Vec3 normalAxis2;

        public float limitsMin;

        public float limitsMax;

        public JPH_SpringSettings limitsSpringSettings;

        public float maxFrictionForce;

        public JPH_MotorSettings motorSettings;
    }

    internal partial struct JPH_ConeConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_ConstraintSpace space;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point1;

        public JPH_Vec3 twistAxis1;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 point2;

        public JPH_Vec3 twistAxis2;

        public float halfConeAngle;
    }

    internal partial struct JPH_SwingTwistConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_ConstraintSpace space;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 position1;

        public JPH_Vec3 twistAxis1;

        public JPH_Vec3 planeAxis1;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 position2;

        public JPH_Vec3 twistAxis2;

        public JPH_Vec3 planeAxis2;

        public JPH_SwingType swingType;

        public float normalHalfConeAngle;

        public float planeHalfConeAngle;

        public float twistMinAngle;

        public float twistMaxAngle;

        public float maxFrictionTorque;

        public JPH_MotorSettings swingMotorSettings;

        public JPH_MotorSettings twistMotorSettings;
    }

    internal partial struct JPH_SixDOFConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_ConstraintSpace space;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 position1;

        public JPH_Vec3 axisX1;

        public JPH_Vec3 axisY1;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 position2;

        public JPH_Vec3 axisX2;

        public JPH_Vec3 axisY2;

        [NativeTypeName("float[6]")]
        public _maxFriction_e__FixedBuffer maxFriction;

        public JPH_SwingType swingType;

        [NativeTypeName("float[6]")]
        public _limitMin_e__FixedBuffer limitMin;

        [NativeTypeName("float[6]")]
        public _limitMax_e__FixedBuffer limitMax;

        [NativeTypeName("JPH_SpringSettings[3]")]
        public _limitsSpringSettings_e__FixedBuffer limitsSpringSettings;

        [NativeTypeName("JPH_MotorSettings[6]")]
        public _motorSettings_e__FixedBuffer motorSettings;

        [InlineArray(6)]
        public partial struct _maxFriction_e__FixedBuffer
        {
            public float e0;
        }

        [InlineArray(6)]
        public partial struct _limitMin_e__FixedBuffer
        {
            public float e0;
        }

        [InlineArray(6)]
        public partial struct _limitMax_e__FixedBuffer
        {
            public float e0;
        }

        [InlineArray(3)]
        public partial struct _limitsSpringSettings_e__FixedBuffer
        {
            public JPH_SpringSettings e0;
        }

        [InlineArray(6)]
        public partial struct _motorSettings_e__FixedBuffer
        {
            public JPH_MotorSettings e0;
        }
    }

    internal partial struct JPH_GearConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_ConstraintSpace space;

        public JPH_Vec3 hingeAxis1;

        public JPH_Vec3 hingeAxis2;

        public float ratio;
    }

    internal unsafe partial struct JPH_BodyLockRead
    {
        [NativeTypeName("const JPH_BodyLockInterface *")]
        public JPH_BodyLockInterface* lockInterface;

        public JPH_SharedMutex* mutex;

        [NativeTypeName("const JPH_Body *")]
        public JPH_Body* body;
    }

    internal unsafe partial struct JPH_BodyLockWrite
    {
        [NativeTypeName("const JPH_BodyLockInterface *")]
        public JPH_BodyLockInterface* lockInterface;

        public JPH_SharedMutex* mutex;

        public JPH_Body* body;
    }

    internal partial struct JPH_BodyLockMultiRead
    {
    }

    internal partial struct JPH_BodyLockMultiWrite
    {
    }

    internal partial struct JPH_ExtendedUpdateSettings
    {
        public JPH_Vec3 stickToFloorStepDown;

        public JPH_Vec3 walkStairsStepUp;

        public float walkStairsMinStepForward;

        public float walkStairsStepForwardTest;

        public float walkStairsCosAngleForwardContact;

        public JPH_Vec3 walkStairsStepDownExtra;
    }

    internal unsafe partial struct JPH_CharacterBaseSettings
    {
        public JPH_Vec3 up;

        public JPH_Plane supportingVolume;

        public float maxSlopeAngle;

        [NativeTypeName("bool")]
        public byte enhancedInternalEdgeRemoval;

        [NativeTypeName("const JPH_Shape *")]
        public JPH_Shape* shape;
    }

    internal partial struct JPH_CharacterSettings
    {
        public JPH_CharacterBaseSettings @base;

        [NativeTypeName("JPH_ObjectLayer")]
        public uint layer;

        public float mass;

        public float friction;

        public float gravityFactor;

        public JPH_AllowedDOFs allowedDOFs;
    }

    internal unsafe partial struct JPH_CharacterVirtualSettings
    {
        public JPH_CharacterBaseSettings @base;

        [NativeTypeName("JPH_CharacterID")]
        public uint ID;

        public float mass;

        public float maxStrength;

        public JPH_Vec3 shapeOffset;

        public JPH_BackFaceMode backFaceMode;

        public float predictiveContactDistance;

        [NativeTypeName("uint32_t")]
        public uint maxCollisionIterations;

        [NativeTypeName("uint32_t")]
        public uint maxConstraintIterations;

        public float minTimeRemaining;

        public float collisionTolerance;

        public float characterPadding;

        [NativeTypeName("uint32_t")]
        public uint maxNumHits;

        public float hitReductionCosMaxAngle;

        public float penetrationRecoverySpeed;

        [NativeTypeName("const JPH_Shape *")]
        public JPH_Shape* innerBodyShape;

        [NativeTypeName("JPH_BodyID")]
        public uint innerBodyIDOverride;

        [NativeTypeName("JPH_ObjectLayer")]
        public uint innerBodyLayer;
    }

    internal partial struct JPH_CharacterContactSettings
    {
        [NativeTypeName("bool")]
        public byte canPushCharacter;

        [NativeTypeName("bool")]
        public byte canReceiveImpulses;
    }

    internal unsafe partial struct JPH_CharacterVirtualContact
    {
        [NativeTypeName("uint64_t")]
        public ulong hash;

        [NativeTypeName("JPH_BodyID")]
        public uint bodyB;

        [NativeTypeName("JPH_CharacterID")]
        public uint characterIDB;

        [NativeTypeName("JPH_SubShapeID")]
        public uint subShapeIDB;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 position;

        public JPH_Vec3 linearVelocity;

        public JPH_Vec3 contactNormal;

        public JPH_Vec3 surfaceNormal;

        public float distance;

        public float fraction;

        public JPH_MotionType motionTypeB;

        [NativeTypeName("bool")]
        public byte isSensorB;

        [NativeTypeName("const JPH_CharacterVirtual *")]
        public JPH_CharacterVirtual* characterB;

        [NativeTypeName("uint64_t")]
        public ulong userData;

        [NativeTypeName("const JPH_PhysicsMaterial *")]
        public JPH_PhysicsMaterial* material;

        [NativeTypeName("bool")]
        public byte hadCollision;

        [NativeTypeName("bool")]
        public byte wasDiscarded;

        [NativeTypeName("bool")]
        public byte canPushCharacter;

        [NativeTypeName("bool")]
        public byte isBackFacingContact;
    }

    internal partial struct JobSystemThreadPoolConfig
    {
        [NativeTypeName("uint32_t")]
        public uint maxJobs;

        [NativeTypeName("uint32_t")]
        public uint maxBarriers;

        [NativeTypeName("int32_t")]
        public int numThreads;
    }

    internal unsafe partial struct JPH_JobSystemConfig
    {
        public void* context;

        [NativeTypeName("JPH_QueueJobCallback *")]
        public delegate* unmanaged[Cdecl]<void*, delegate* unmanaged[Cdecl]<void*, void>, void*, void> queueJob;

        [NativeTypeName("JPH_QueueJobsCallback *")]
        public delegate* unmanaged[Cdecl]<void*, delegate* unmanaged[Cdecl]<void*, void>, void**, uint, void> queueJobs;

        [NativeTypeName("uint32_t")]
        public uint maxConcurrency;

        [NativeTypeName("uint32_t")]
        public uint maxBarriers;
    }

    internal partial struct JPH_JobSystem
    {
    }

    internal unsafe partial struct JPH_PhysicsSystemSettings
    {
        [NativeTypeName("uint32_t")]
        public uint maxBodies;

        [NativeTypeName("uint32_t")]
        public uint numBodyMutexes;

        [NativeTypeName("uint32_t")]
        public uint maxBodyPairs;

        [NativeTypeName("uint32_t")]
        public uint maxContactConstraints;

        [NativeTypeName("uint32_t")]
        public uint _padding;

        public JPH_BroadPhaseLayerInterface* broadPhaseLayerInterface;

        public JPH_ObjectLayerPairFilter* objectLayerPairFilter;

        public JPH_ObjectVsBroadPhaseLayerFilter* objectVsBroadPhaseLayerFilter;
    }

    internal partial struct JPH_PhysicsSettings
    {
        public int maxInFlightBodyPairs;

        public int stepListenersBatchSize;

        public int stepListenerBatchesPerJob;

        public float baumgarte;

        public float speculativeContactDistance;

        public float penetrationSlop;

        public float linearCastThreshold;

        public float linearCastMaxPenetration;

        public float manifoldTolerance;

        public float maxPenetrationDistance;

        public float bodyPairCacheMaxDeltaPositionSq;

        public float bodyPairCacheCosMaxDeltaRotationDiv2;

        public float contactNormalCosMaxDeltaRotation;

        public float contactPointPreserveLambdaMaxDistSq;

        [NativeTypeName("uint32_t")]
        public uint numVelocitySteps;

        [NativeTypeName("uint32_t")]
        public uint numPositionSteps;

        public float minVelocityForRestitution;

        public float timeBeforeSleep;

        public float pointVelocitySleepThreshold;

        [NativeTypeName("bool")]
        public byte deterministicSimulation;

        [NativeTypeName("bool")]
        public byte constraintWarmStart;

        [NativeTypeName("bool")]
        public byte useBodyPairContactCache;

        [NativeTypeName("bool")]
        public byte useManifoldReduction;

        [NativeTypeName("bool")]
        public byte useLargeIslandSplitter;

        [NativeTypeName("bool")]
        public byte allowSleeping;

        [NativeTypeName("bool")]
        public byte checkActiveEdges;
    }

    internal unsafe partial struct JPH_PhysicsStepListenerContext
    {
        public float deltaTime;

        [NativeTypeName("JPH_Bool")]
        public uint isFirstStep;

        [NativeTypeName("JPH_Bool")]
        public uint isLastStep;

        public JPH_PhysicsSystem* physicsSystem;
    }

    internal unsafe partial struct JPH_PhysicsStepListener_Procs
    {
        [NativeTypeName("void (*)(void *, const JPH_PhysicsStepListenerContext *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_PhysicsStepListenerContext*, void> OnStep;
    }

    internal unsafe partial struct JPH_BroadPhaseLayerFilter_Procs
    {
        [NativeTypeName("bool (*)(void *, JPH_BroadPhaseLayer)")]
        public delegate* unmanaged[Cdecl]<void*, byte, byte> ShouldCollide;
    }

    internal unsafe partial struct JPH_ObjectLayerFilter_Procs
    {
        [NativeTypeName("bool (*)(void *, JPH_ObjectLayer)")]
        public delegate* unmanaged[Cdecl]<void*, uint, byte> ShouldCollide;
    }

    internal unsafe partial struct JPH_BodyFilter_Procs
    {
        [NativeTypeName("bool (*)(void *, JPH_BodyID)")]
        public delegate* unmanaged[Cdecl]<void*, uint, byte> ShouldCollide;

        [NativeTypeName("bool (*)(void *, const JPH_Body *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Body*, byte> ShouldCollideLocked;
    }

    internal unsafe partial struct JPH_ShapeFilter_Procs
    {
        [NativeTypeName("bool (*)(void *, const JPH_Shape *, const JPH_SubShapeID *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Shape*, uint*, byte> ShouldCollide;

        [NativeTypeName("bool (*)(void *, const JPH_Shape *, const JPH_SubShapeID *, const JPH_Shape *, const JPH_SubShapeID *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Shape*, uint*, JPH_Shape*, uint*, byte> ShouldCollide2;
    }

    internal unsafe partial struct JPH_SimShapeFilter_Procs
    {
        [NativeTypeName("bool (*)(void *, const JPH_Body *, const JPH_Shape *, const JPH_SubShapeID *, const JPH_Body *, const JPH_Shape *, const JPH_SubShapeID *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Body*, JPH_Shape*, uint*, JPH_Body*, JPH_Shape*, uint*, byte> ShouldCollide;
    }

    internal unsafe partial struct JPH_ContactListener_Procs
    {
        [NativeTypeName("JPH_ValidateResult (*)(void *, const JPH_Body *, const JPH_Body *, const JPH_RVec3 *, const JPH_CollideShapeResult *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Body*, JPH_Body*, JPH_Vec3*, JPH_CollideShapeResult*, JPH_ValidateResult> OnContactValidate;

        [NativeTypeName("void (*)(void *, const JPH_Body *, const JPH_Body *, const JPH_ContactManifold *, JPH_ContactSettings *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Body*, JPH_Body*, JPH_ContactManifold*, JPH_ContactSettings*, void> OnContactAdded;

        [NativeTypeName("void (*)(void *, const JPH_Body *, const JPH_Body *, const JPH_ContactManifold *, JPH_ContactSettings *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Body*, JPH_Body*, JPH_ContactManifold*, JPH_ContactSettings*, void> OnContactPersisted;

        [NativeTypeName("void (*)(void *, const JPH_SubShapeIDPair *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_SubShapeIDPair*, void> OnContactRemoved;
    }

    internal unsafe partial struct JPH_BodyActivationListener_Procs
    {
        [NativeTypeName("void (*)(void *, JPH_BodyID, uint64_t)")]
        public delegate* unmanaged[Cdecl]<void*, uint, ulong, void> OnBodyActivated;

        [NativeTypeName("void (*)(void *, JPH_BodyID, uint64_t)")]
        public delegate* unmanaged[Cdecl]<void*, uint, ulong, void> OnBodyDeactivated;
    }

    internal unsafe partial struct JPH_BodyDrawFilter_Procs
    {
        [NativeTypeName("bool (*)(void *, const JPH_Body *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Body*, byte> ShouldDraw;
    }

    internal unsafe partial struct JPH_CharacterContactListener_Procs
    {
        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_Body *, JPH_Vec3 *, JPH_Vec3 *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_Body*, JPH_Vec3*, JPH_Vec3*, void> OnAdjustBodyVelocity;

        [NativeTypeName("bool (*)(void *, const JPH_CharacterVirtual *, const JPH_CharacterVirtualContact *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_CharacterVirtualContact*, byte> OnContactValidate;

        [NativeTypeName("bool (*)(void *, const JPH_CharacterVirtual *, const JPH_CharacterVirtualContact *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_CharacterVirtualContact*, byte> OnCharacterContactValidate;

        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_CharacterVirtualContact *, JPH_CharacterContactSettings *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_CharacterVirtualContact*, JPH_CharacterContactSettings*, void> OnContactAdded;

        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_CharacterVirtualContact *, JPH_CharacterContactSettings *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_CharacterVirtualContact*, JPH_CharacterContactSettings*, void> OnContactPersisted;

        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_BodyID, const JPH_SubShapeID)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, uint, uint, void> OnContactRemoved;

        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_CharacterVirtualContact *, JPH_CharacterContactSettings *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_CharacterVirtualContact*, JPH_CharacterContactSettings*, void> OnCharacterContactAdded;

        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_CharacterVirtualContact *, JPH_CharacterContactSettings *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_CharacterVirtualContact*, JPH_CharacterContactSettings*, void> OnCharacterContactPersisted;

        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_CharacterID, const JPH_SubShapeID)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, uint, uint, void> OnCharacterContactRemoved;

        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_BodyID, const JPH_SubShapeID, const JPH_RVec3 *, const JPH_Vec3 *, const JPH_Vec3 *, const JPH_PhysicsMaterial *, const JPH_Vec3 *, JPH_Vec3 *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, uint, uint, JPH_Vec3*, JPH_Vec3*, JPH_Vec3*, JPH_PhysicsMaterial*, JPH_Vec3*, JPH_Vec3*, void> OnContactSolve;

        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_CharacterVirtual *, const JPH_SubShapeID, const JPH_RVec3 *, const JPH_Vec3 *, const JPH_Vec3 *, const JPH_PhysicsMaterial *, const JPH_Vec3 *, JPH_Vec3 *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_CharacterVirtual*, uint, JPH_Vec3*, JPH_Vec3*, JPH_Vec3*, JPH_PhysicsMaterial*, JPH_Vec3*, JPH_Vec3*, void> OnCharacterContactSolve;
    }

    internal unsafe partial struct JPH_CharacterVsCharacterCollision_Procs
    {
        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_RMat4 *, const JPH_CollideShapeSettings *, const JPH_RVec3 *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_Mat4*, JPH_CollideShapeSettings*, JPH_Vec3*, void> CollideCharacter;

        [NativeTypeName("void (*)(void *, const JPH_CharacterVirtual *, const JPH_RMat4 *, const JPH_Vec3 *, const JPH_ShapeCastSettings *, const JPH_RVec3 *)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_CharacterVirtual*, JPH_Mat4*, JPH_Vec3*, JPH_ShapeCastSettings*, JPH_Vec3*, void> CastCharacter;
    }

    internal unsafe partial struct JPH_DebugRenderer_Procs
    {
        [NativeTypeName("void (*)(void *, const JPH_RVec3 *, const JPH_RVec3 *, JPH_Color)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Vec3*, JPH_Vec3*, uint, void> DrawLine;

        [NativeTypeName("void (*)(void *, const JPH_RVec3 *, const JPH_RVec3 *, const JPH_RVec3 *, JPH_Color, JPH_DebugRenderer_CastShadow)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Vec3*, JPH_Vec3*, JPH_Vec3*, uint, JPH_DebugRenderer_CastShadow, void> DrawTriangle;

        [NativeTypeName("void (*)(void *, const JPH_RVec3 *, const char *, JPH_Color, float)")]
        public delegate* unmanaged[Cdecl]<void*, JPH_Vec3*, sbyte*, uint, float, void> DrawText3D;
    }

    internal unsafe partial struct JPH_SkeletonJoint
    {
        [NativeTypeName("const char *")]
        public sbyte* name;

        [NativeTypeName("const char *")]
        public sbyte* parentName;

        public int parentJointIndex;
    }

    internal partial struct JPH_WheelSettings
    {
    }

    internal partial struct JPH_WheelSettingsWV
    {
    }

    internal partial struct JPH_WheelSettingsTV
    {
    }

    internal partial struct JPH_Wheel
    {
    }

    internal partial struct JPH_WheelWV
    {
    }

    internal partial struct JPH_WheelTV
    {
    }

    internal partial struct JPH_VehicleEngine
    {
    }

    internal partial struct JPH_VehicleTransmission
    {
    }

    internal partial struct JPH_VehicleTransmissionSettings
    {
    }

    internal partial struct JPH_VehicleCollisionTester
    {
    }

    internal partial struct JPH_VehicleCollisionTesterRay
    {
    }

    internal partial struct JPH_VehicleCollisionTesterCastSphere
    {
    }

    internal partial struct JPH_VehicleCollisionTesterCastCylinder
    {
    }

    internal partial struct JPH_VehicleConstraint
    {
    }

    internal partial struct JPH_VehicleControllerSettings
    {
    }

    internal partial struct JPH_WheeledVehicleControllerSettings
    {
    }

    internal partial struct JPH_MotorcycleControllerSettings
    {
    }

    internal partial struct JPH_TrackedVehicleControllerSettings
    {
    }

    internal partial struct JPH_WheeledVehicleController
    {
    }

    internal partial struct JPH_MotorcycleController
    {
    }

    internal partial struct JPH_TrackedVehicleController
    {
    }

    internal partial struct JPH_VehicleController
    {
    }

    internal partial struct JPH_VehicleAntiRollBar
    {
        public int leftWheel;

        public int rightWheel;

        public float stiffness;
    }

    internal unsafe partial struct JPH_VehicleConstraintSettings
    {
        public JPH_ConstraintSettings @base;

        public JPH_Vec3 up;

        public JPH_Vec3 forward;

        public float maxPitchRollAngle;

        [NativeTypeName("uint32_t")]
        public uint wheelsCount;

        public JPH_WheelSettings** wheels;

        [NativeTypeName("uint32_t")]
        public uint antiRollBarsCount;

        [NativeTypeName("const JPH_VehicleAntiRollBar *")]
        public JPH_VehicleAntiRollBar* antiRollBars;

        public JPH_VehicleControllerSettings* controller;
    }

    internal unsafe partial struct JPH_VehicleEngineSettings
    {
        public float maxTorque;

        public float minRPM;

        public float maxRPM;

        [NativeTypeName("const JPH_LinearCurve *")]
        public JPH_LinearCurve* normalizedTorque;

        public float inertia;

        public float angularDamping;
    }

    internal partial struct JPH_VehicleDifferentialSettings
    {
        public int leftWheel;

        public int rightWheel;

        public float differentialRatio;

        public float leftRightSplit;

        public float limitedSlipRatio;

        public float engineTorqueRatio;
    }

    internal partial struct JPH_VehicleTrack
    {
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_TrackSide : uint
    {
        JPH_TrackSide_Left = 0,
        JPH_TrackSide_Right = 1,
    }

    internal unsafe partial struct JPH_VehicleTrackSettings
    {
        [NativeTypeName("uint32_t")]
        public uint drivenWheel;

        [NativeTypeName("const uint32_t *")]
        public uint* wheels;

        [NativeTypeName("uint32_t")]
        public uint wheelsCount;

        public float inertia;

        public float angularDamping;

        public float maxBrakeTorque;

        public float differentialRatio;
    }

    internal static unsafe partial class Methods
    {
        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_JobSystem* JPH_JobSystemThreadPool_Create([NativeTypeName("const JobSystemThreadPoolConfig *")] JobSystemThreadPoolConfig* config);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_JobSystem* JPH_JobSystemCallback_Create([NativeTypeName("const JPH_JobSystemConfig *")] JPH_JobSystemConfig* config);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_JobSystem_Destroy(JPH_JobSystem* jobSystem);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TempAllocator* JPH_TempAllocator_Create([NativeTypeName("uint32_t")] uint size);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TempAllocator* JPH_TempAllocatorMalloc_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TempAllocator_Destroy(JPH_TempAllocator* allocator);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Init();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shutdown();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SetTraceHandler([NativeTypeName("JPH_TraceFunc")] delegate* unmanaged[Cdecl]<sbyte*, void> handler);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SetAssertFailureHandler([NativeTypeName("JPH_AssertFailureFunc")] delegate* unmanaged[Cdecl]<sbyte*, sbyte*, sbyte*, uint, byte> handler);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CollideShapeResult_FreeMembers(JPH_CollideShapeResult* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CollisionEstimationResult_FreeMembers(JPH_CollisionEstimationResult* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BroadPhaseLayerInterface_Destroy(JPH_BroadPhaseLayerInterface* bpInterface);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BroadPhaseLayerInterface* JPH_BroadPhaseLayerInterfaceMask_Create([NativeTypeName("uint32_t")] uint numBroadPhaseLayers);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BroadPhaseLayerInterfaceMask_ConfigureLayer(JPH_BroadPhaseLayerInterface* bpInterface, [NativeTypeName("JPH_BroadPhaseLayer")] byte broadPhaseLayer, [NativeTypeName("uint32_t")] uint groupsToInclude, [NativeTypeName("uint32_t")] uint groupsToExclude);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BroadPhaseLayerInterface* JPH_BroadPhaseLayerInterfaceTable_Create([NativeTypeName("uint32_t")] uint numObjectLayers, [NativeTypeName("uint32_t")] uint numBroadPhaseLayers);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BroadPhaseLayerInterfaceTable_MapObjectToBroadPhaseLayer(JPH_BroadPhaseLayerInterface* bpInterface, [NativeTypeName("JPH_ObjectLayer")] uint objectLayer, [NativeTypeName("JPH_BroadPhaseLayer")] byte broadPhaseLayer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ObjectLayerPairFilter_Destroy(JPH_ObjectLayerPairFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ObjectLayerPairFilter* JPH_ObjectLayerPairFilterMask_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_ObjectLayer")]
        public static extern uint JPH_ObjectLayerPairFilterMask_GetObjectLayer([NativeTypeName("uint32_t")] uint group, [NativeTypeName("uint32_t")] uint mask);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_ObjectLayerPairFilterMask_GetGroup([NativeTypeName("JPH_ObjectLayer")] uint layer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_ObjectLayerPairFilterMask_GetMask([NativeTypeName("JPH_ObjectLayer")] uint layer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ObjectLayerPairFilter* JPH_ObjectLayerPairFilterTable_Create([NativeTypeName("uint32_t")] uint numObjectLayers);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ObjectLayerPairFilterTable_DisableCollision(JPH_ObjectLayerPairFilter* objectFilter, [NativeTypeName("JPH_ObjectLayer")] uint layer1, [NativeTypeName("JPH_ObjectLayer")] uint layer2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ObjectLayerPairFilterTable_EnableCollision(JPH_ObjectLayerPairFilter* objectFilter, [NativeTypeName("JPH_ObjectLayer")] uint layer1, [NativeTypeName("JPH_ObjectLayer")] uint layer2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_ObjectLayerPairFilterTable_ShouldCollide(JPH_ObjectLayerPairFilter* objectFilter, [NativeTypeName("JPH_ObjectLayer")] uint layer1, [NativeTypeName("JPH_ObjectLayer")] uint layer2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ObjectVsBroadPhaseLayerFilter_Destroy(JPH_ObjectVsBroadPhaseLayerFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ObjectVsBroadPhaseLayerFilter* JPH_ObjectVsBroadPhaseLayerFilterMask_Create([NativeTypeName("const JPH_BroadPhaseLayerInterface *")] JPH_BroadPhaseLayerInterface* broadPhaseLayerInterface);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ObjectVsBroadPhaseLayerFilter* JPH_ObjectVsBroadPhaseLayerFilterTable_Create(JPH_BroadPhaseLayerInterface* broadPhaseLayerInterface, [NativeTypeName("uint32_t")] uint numBroadPhaseLayers, JPH_ObjectLayerPairFilter* objectLayerPairFilter, [NativeTypeName("uint32_t")] uint numObjectLayers);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DrawSettings_InitDefault(JPH_DrawSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PhysicsSystem* JPH_PhysicsSystem_Create([NativeTypeName("const JPH_PhysicsSystemSettings *")] JPH_PhysicsSystemSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_Destroy(JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_SetPhysicsSettings(JPH_PhysicsSystem* system, JPH_PhysicsSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_GetPhysicsSettings(JPH_PhysicsSystem* system, JPH_PhysicsSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_OptimizeBroadPhase(JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PhysicsUpdateError JPH_PhysicsSystem_Update(JPH_PhysicsSystem* system, float deltaTime, int collisionSteps, JPH_TempAllocator* tempAllocator, JPH_JobSystem* jobSystem);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyInterface* JPH_PhysicsSystem_GetBodyInterface(JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyInterface* JPH_PhysicsSystem_GetBodyInterfaceNoLock(JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_BodyLockInterface *")]
        public static extern JPH_BodyLockInterface* JPH_PhysicsSystem_GetBodyLockInterface([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_BodyLockInterface *")]
        public static extern JPH_BodyLockInterface* JPH_PhysicsSystem_GetBodyLockInterfaceNoLock([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_BroadPhaseQuery *")]
        public static extern JPH_BroadPhaseQuery* JPH_PhysicsSystem_GetBroadPhaseQuery([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_NarrowPhaseQuery *")]
        public static extern JPH_NarrowPhaseQuery* JPH_PhysicsSystem_GetNarrowPhaseQuery([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_NarrowPhaseQuery *")]
        public static extern JPH_NarrowPhaseQuery* JPH_PhysicsSystem_GetNarrowPhaseQueryNoLock([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_SetContactListener(JPH_PhysicsSystem* system, JPH_ContactListener* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_SetBodyActivationListener(JPH_PhysicsSystem* system, JPH_BodyActivationListener* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_SetSimShapeFilter(JPH_PhysicsSystem* system, [NativeTypeName("const JPH_SimShapeFilter *")] JPH_SimShapeFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_PhysicsSystem_WereBodiesInContact([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system, [NativeTypeName("JPH_BodyID")] uint body1, [NativeTypeName("JPH_BodyID")] uint body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_PhysicsSystem_GetNumBodies([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_PhysicsSystem_GetNumActiveBodies([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system, JPH_BodyType type);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_PhysicsSystem_GetMaxBodies([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_PhysicsSystem_GetNumConstraints([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_SetGravity(JPH_PhysicsSystem* system, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_GetGravity(JPH_PhysicsSystem* system, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_AddConstraint(JPH_PhysicsSystem* system, JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_RemoveConstraint(JPH_PhysicsSystem* system, JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_AddConstraints(JPH_PhysicsSystem* system, JPH_Constraint** constraints, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_RemoveConstraints(JPH_PhysicsSystem* system, JPH_Constraint** constraints, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_AddStepListener(JPH_PhysicsSystem* system, JPH_PhysicsStepListener* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_RemoveStepListener(JPH_PhysicsSystem* system, JPH_PhysicsStepListener* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_GetBodies([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system, [NativeTypeName("JPH_BodyID *")] uint* ids, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_GetActiveBodies([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system, JPH_BodyType type, [NativeTypeName("JPH_BodyID *")] uint* ids, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_BodyID *")]
        public static extern uint* JPH_PhysicsSystem_GetActiveBodiesUnsafe([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system, JPH_BodyType type);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_GetConstraints([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system, [NativeTypeName("const JPH_Constraint **")] JPH_Constraint** constraints, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_ActivateBodiesInAABox(JPH_PhysicsSystem* system, [NativeTypeName("const JPH_AABox *")] JPH_AABox* box, [NativeTypeName("JPH_ObjectLayer")] uint layer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_DrawBodies(JPH_PhysicsSystem* system, [NativeTypeName("const JPH_DrawSettings *")] JPH_DrawSettings* settings, JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_BodyDrawFilter *")] JPH_BodyDrawFilter* bodyFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_DrawConstraints(JPH_PhysicsSystem* system, JPH_DebugRenderer* renderer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_DrawConstraintLimits(JPH_PhysicsSystem* system, JPH_DebugRenderer* renderer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsSystem_DrawConstraintReferenceFrame(JPH_PhysicsSystem* system, JPH_DebugRenderer* renderer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsStepListener_SetProcs([NativeTypeName("const JPH_PhysicsStepListener_Procs *")] JPH_PhysicsStepListener_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PhysicsStepListener* JPH_PhysicsStepListener_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsStepListener_Destroy(JPH_PhysicsStepListener* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Math_Sin(float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Math_Cos(float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_FromTo([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* from, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* to, JPH_Quat* quat);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_GetAxisAngle([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, JPH_Vec3* outAxis, float* outAngle);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_GetEulerAngles([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_RotateAxisX([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_RotateAxisY([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_RotateAxisZ([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_Inversed([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_GetPerpendicular([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Quat_GetRotationAngle([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_FromEulerAngles([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* angles, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_Add([NativeTypeName("const JPH_Quat *")] JPH_Quat* q1, [NativeTypeName("const JPH_Quat *")] JPH_Quat* q2, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_Subtract([NativeTypeName("const JPH_Quat *")] JPH_Quat* q1, [NativeTypeName("const JPH_Quat *")] JPH_Quat* q2, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_Multiply([NativeTypeName("const JPH_Quat *")] JPH_Quat* q1, [NativeTypeName("const JPH_Quat *")] JPH_Quat* q2, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_MultiplyScalar([NativeTypeName("const JPH_Quat *")] JPH_Quat* q, float scalar, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_DivideScalar([NativeTypeName("const JPH_Quat *")] JPH_Quat* q, float scalar, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_Dot([NativeTypeName("const JPH_Quat *")] JPH_Quat* q1, [NativeTypeName("const JPH_Quat *")] JPH_Quat* q2, float* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_Conjugated([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_GetTwist([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* axis, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_GetSwingTwist([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, JPH_Quat* outSwing, JPH_Quat* outTwist);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_Lerp([NativeTypeName("const JPH_Quat *")] JPH_Quat* from, [NativeTypeName("const JPH_Quat *")] JPH_Quat* to, float fraction, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_Slerp([NativeTypeName("const JPH_Quat *")] JPH_Quat* from, [NativeTypeName("const JPH_Quat *")] JPH_Quat* to, float fraction, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_Rotate([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* vec, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Quat_InverseRotate([NativeTypeName("const JPH_Quat *")] JPH_Quat* quat, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* vec, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_AxisX(JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_AxisY(JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_AxisZ(JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Vec3_IsClose([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v2, float maxDistSq);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Vec3_IsNearZero([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v, float maxDistSq);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Vec3_IsNormalized([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v, float tolerance);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Vec3_IsNaN([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_Negate([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_Normalized([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_Cross([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v2, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_Abs([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Vec3_Length([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Vec3_LengthSquared([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_DotProduct([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v2, float* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_Normalize([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_Add([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v2, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_Subtract([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v2, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_Multiply([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v2, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_MultiplyScalar([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v, float scalar, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_MultiplyMatrix([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* left, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* right, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_Divide([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v2, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_DivideScalar([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v, float scalar, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Vec3_GetNormalizedPerpendicular([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Add([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* m1, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* m2, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Subtract([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* m1, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* m2, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Multiply([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* m1, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* m2, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_MultiplyScalar([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* m, float scalar, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Zero(JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Identity(JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Rotation(JPH_Mat4* result, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Rotation2(JPH_Mat4* result, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* axis, float angle);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Translation(JPH_Mat4* result, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* translation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_RotationTranslation(JPH_Mat4* result, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* translation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_InverseRotationTranslation(JPH_Mat4* result, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* translation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Scale(JPH_Mat4* result, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Transposed([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* m, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_Inversed([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* matrix, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_GetAxisX([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* matrix, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_GetAxisY([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* matrix, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_GetAxisZ([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* matrix, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_GetTranslation([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* matrix, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Mat4_GetQuaternion([NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* matrix, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PhysicsMaterial* JPH_PhysicsMaterial_Create([NativeTypeName("const char *")] sbyte* name, [NativeTypeName("uint32_t")] uint color);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PhysicsMaterial_Destroy(JPH_PhysicsMaterial* material);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern sbyte* JPH_PhysicsMaterial_GetDebugName([NativeTypeName("const JPH_PhysicsMaterial *")] JPH_PhysicsMaterial* material);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_PhysicsMaterial_GetDebugColor([NativeTypeName("const JPH_PhysicsMaterial *")] JPH_PhysicsMaterial* material);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_GroupFilter_Destroy(JPH_GroupFilter* groupFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_GroupFilter_CanCollide(JPH_GroupFilter* groupFilter, [NativeTypeName("const JPH_CollisionGroup *")] JPH_CollisionGroup* group1, [NativeTypeName("const JPH_CollisionGroup *")] JPH_CollisionGroup* group2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_GroupFilterTable* JPH_GroupFilterTable_Create([NativeTypeName("uint32_t")] uint numSubGroups);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_GroupFilterTable_DisableCollision(JPH_GroupFilterTable* table, [NativeTypeName("JPH_CollisionSubGroupID")] uint subGroup1, [NativeTypeName("JPH_CollisionSubGroupID")] uint subGroup2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_GroupFilterTable_EnableCollision(JPH_GroupFilterTable* table, [NativeTypeName("JPH_CollisionSubGroupID")] uint subGroup1, [NativeTypeName("JPH_CollisionSubGroupID")] uint subGroup2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_GroupFilterTable_IsCollisionEnabled(JPH_GroupFilterTable* table, [NativeTypeName("JPH_CollisionSubGroupID")] uint subGroup1, [NativeTypeName("JPH_CollisionSubGroupID")] uint subGroup2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ShapeSettings_Destroy(JPH_ShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong JPH_ShapeSettings_GetUserData([NativeTypeName("const JPH_ShapeSettings *")] JPH_ShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ShapeSettings_SetUserData(JPH_ShapeSettings* settings, [NativeTypeName("uint64_t")] ulong userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_Draw([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* centerOfMassTransform, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale, [NativeTypeName("JPH_Color")] uint color, [NativeTypeName("bool")] byte useMaterialColors, [NativeTypeName("bool")] byte drawWireframe);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_Destroy(JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ShapeType JPH_Shape_GetType([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ShapeSubType JPH_Shape_GetSubType([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong JPH_Shape_GetUserData([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_SetUserData(JPH_Shape* shape, [NativeTypeName("uint64_t")] ulong userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Shape_MustBeStatic([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_GetCenterOfMass([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_GetLocalBounds([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, JPH_AABox* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_Shape_GetSubShapeIDBitsRecursive([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_GetWorldSpaceBounds([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* centerOfMassTransform, JPH_Vec3* scale, JPH_AABox* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Shape_GetInnerRadius([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_GetMassProperties([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, JPH_MassProperties* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Shape *")]
        public static extern JPH_Shape* JPH_Shape_GetLeafShape([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("JPH_SubShapeID")] uint subShapeID, [NativeTypeName("JPH_SubShapeID *")] uint* remainder);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_PhysicsMaterial *")]
        public static extern JPH_PhysicsMaterial* JPH_Shape_GetMaterial([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("JPH_SubShapeID")] uint subShapeID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_GetSurfaceNormal([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("JPH_SubShapeID")] uint subShapeID, JPH_Vec3* localPosition, JPH_Vec3* normal);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_GetSupportingFace([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_SubShapeID")] uint subShapeID, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* centerOfMassTransform, JPH_SupportingFace* outVertices);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Shape_GetVolume([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Shape_IsValidScale([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Shape_MakeScaleValid([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Shape* JPH_Shape_ScaleShape([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Shape_CastRay([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* origin, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, JPH_RayCastResult* hit);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Shape_CastRay2([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* origin, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, [NativeTypeName("const JPH_RayCastSettings *")] JPH_RayCastSettings* rayCastSettings, JPH_CollisionCollectorType collectorType, [NativeTypeName("JPH_CastRayResultCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_RayCastResult*, void> callback, void* userData, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Shape_CollidePoint([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* point, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Shape_CollidePoint2([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* point, JPH_CollisionCollectorType collectorType, [NativeTypeName("JPH_CollidePointResultCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_CollidePointResult*, void> callback, void* userData, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_ConvexShapeSettings_GetDensity([NativeTypeName("const JPH_ConvexShapeSettings *")] JPH_ConvexShapeSettings* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ConvexShapeSettings_SetDensity(JPH_ConvexShapeSettings* shape, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_ConvexShape_GetDensity([NativeTypeName("const JPH_ConvexShape *")] JPH_ConvexShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ConvexShape_SetDensity(JPH_ConvexShape* shape, float inDensity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BoxShapeSettings* JPH_BoxShapeSettings_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* halfExtent, float convexRadius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BoxShape* JPH_BoxShapeSettings_CreateShape([NativeTypeName("const JPH_BoxShapeSettings *")] JPH_BoxShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BoxShape* JPH_BoxShape_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* halfExtent, float convexRadius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BoxShape_GetHalfExtent([NativeTypeName("const JPH_BoxShape *")] JPH_BoxShape* shape, JPH_Vec3* halfExtent);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BoxShape_GetConvexRadius([NativeTypeName("const JPH_BoxShape *")] JPH_BoxShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SphereShapeSettings* JPH_SphereShapeSettings_Create(float radius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SphereShape* JPH_SphereShapeSettings_CreateShape([NativeTypeName("const JPH_SphereShapeSettings *")] JPH_SphereShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SphereShapeSettings_GetRadius([NativeTypeName("const JPH_SphereShapeSettings *")] JPH_SphereShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SphereShapeSettings_SetRadius(JPH_SphereShapeSettings* settings, float radius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SphereShape* JPH_SphereShape_Create(float radius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SphereShape_GetRadius([NativeTypeName("const JPH_SphereShape *")] JPH_SphereShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PlaneShapeSettings* JPH_PlaneShapeSettings_Create([NativeTypeName("const JPH_Plane *")] JPH_Plane* plane, [NativeTypeName("const JPH_PhysicsMaterial *")] JPH_PhysicsMaterial* material, float halfExtent);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PlaneShape* JPH_PlaneShapeSettings_CreateShape([NativeTypeName("const JPH_PlaneShapeSettings *")] JPH_PlaneShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PlaneShape* JPH_PlaneShape_Create([NativeTypeName("const JPH_Plane *")] JPH_Plane* plane, [NativeTypeName("const JPH_PhysicsMaterial *")] JPH_PhysicsMaterial* material, float halfExtent);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PlaneShape_GetPlane([NativeTypeName("const JPH_PlaneShape *")] JPH_PlaneShape* shape, JPH_Plane* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_PlaneShape_GetHalfExtent([NativeTypeName("const JPH_PlaneShape *")] JPH_PlaneShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TriangleShapeSettings* JPH_TriangleShapeSettings_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v2, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v3, float convexRadius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TriangleShape* JPH_TriangleShapeSettings_CreateShape([NativeTypeName("const JPH_TriangleShapeSettings *")] JPH_TriangleShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TriangleShape* JPH_TriangleShape_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v2, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* v3, float convexRadius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TriangleShape_GetConvexRadius([NativeTypeName("const JPH_TriangleShape *")] JPH_TriangleShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TriangleShape_GetVertex1([NativeTypeName("const JPH_TriangleShape *")] JPH_TriangleShape* shape, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TriangleShape_GetVertex2([NativeTypeName("const JPH_TriangleShape *")] JPH_TriangleShape* shape, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TriangleShape_GetVertex3([NativeTypeName("const JPH_TriangleShape *")] JPH_TriangleShape* shape, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CapsuleShapeSettings* JPH_CapsuleShapeSettings_Create(float halfHeightOfCylinder, float radius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CapsuleShape* JPH_CapsuleShapeSettings_CreateShape([NativeTypeName("const JPH_CapsuleShapeSettings *")] JPH_CapsuleShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CapsuleShape* JPH_CapsuleShape_Create(float halfHeightOfCylinder, float radius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CapsuleShape_GetRadius([NativeTypeName("const JPH_CapsuleShape *")] JPH_CapsuleShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CapsuleShape_GetHalfHeightOfCylinder([NativeTypeName("const JPH_CapsuleShape *")] JPH_CapsuleShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CylinderShapeSettings* JPH_CylinderShapeSettings_Create(float halfHeight, float radius, float convexRadius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CylinderShape* JPH_CylinderShapeSettings_CreateShape([NativeTypeName("const JPH_CylinderShapeSettings *")] JPH_CylinderShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CylinderShape* JPH_CylinderShape_Create(float halfHeight, float radius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CylinderShape_GetRadius([NativeTypeName("const JPH_CylinderShape *")] JPH_CylinderShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CylinderShape_GetHalfHeight([NativeTypeName("const JPH_CylinderShape *")] JPH_CylinderShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TaperedCylinderShapeSettings* JPH_TaperedCylinderShapeSettings_Create(float halfHeightOfTaperedCylinder, float topRadius, float bottomRadius, float convexRadius, [NativeTypeName("const JPH_PhysicsMaterial *")] JPH_PhysicsMaterial* material);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TaperedCylinderShape* JPH_TaperedCylinderShapeSettings_CreateShape([NativeTypeName("const JPH_TaperedCylinderShapeSettings *")] JPH_TaperedCylinderShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TaperedCylinderShape_GetTopRadius([NativeTypeName("const JPH_TaperedCylinderShape *")] JPH_TaperedCylinderShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TaperedCylinderShape_GetBottomRadius([NativeTypeName("const JPH_TaperedCylinderShape *")] JPH_TaperedCylinderShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TaperedCylinderShape_GetConvexRadius([NativeTypeName("const JPH_TaperedCylinderShape *")] JPH_TaperedCylinderShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TaperedCylinderShape_GetHalfHeight([NativeTypeName("const JPH_TaperedCylinderShape *")] JPH_TaperedCylinderShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ConvexHullShapeSettings* JPH_ConvexHullShapeSettings_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* points, [NativeTypeName("uint32_t")] uint pointsCount, float maxConvexRadius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ConvexHullShape* JPH_ConvexHullShapeSettings_CreateShape([NativeTypeName("const JPH_ConvexHullShapeSettings *")] JPH_ConvexHullShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_ConvexHullShape_GetNumPoints([NativeTypeName("const JPH_ConvexHullShape *")] JPH_ConvexHullShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ConvexHullShape_GetPoint([NativeTypeName("const JPH_ConvexHullShape *")] JPH_ConvexHullShape* shape, [NativeTypeName("uint32_t")] uint index, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_ConvexHullShape_GetNumFaces([NativeTypeName("const JPH_ConvexHullShape *")] JPH_ConvexHullShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_ConvexHullShape_GetNumVerticesInFace([NativeTypeName("const JPH_ConvexHullShape *")] JPH_ConvexHullShape* shape, [NativeTypeName("uint32_t")] uint faceIndex);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_ConvexHullShape_GetFaceVertices([NativeTypeName("const JPH_ConvexHullShape *")] JPH_ConvexHullShape* shape, [NativeTypeName("uint32_t")] uint faceIndex, [NativeTypeName("uint32_t")] uint maxVertices, [NativeTypeName("uint32_t *")] uint* vertices);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MeshShapeSettings* JPH_MeshShapeSettings_Create([NativeTypeName("const JPH_Triangle *")] JPH_Triangle* triangles, [NativeTypeName("uint32_t")] uint triangleCount);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MeshShapeSettings* JPH_MeshShapeSettings_Create2([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* vertices, [NativeTypeName("uint32_t")] uint verticesCount, [NativeTypeName("const JPH_IndexedTriangle *")] JPH_IndexedTriangle* triangles, [NativeTypeName("uint32_t")] uint triangleCount);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_MeshShapeSettings_GetMaxTrianglesPerLeaf([NativeTypeName("const JPH_MeshShapeSettings *")] JPH_MeshShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MeshShapeSettings_SetMaxTrianglesPerLeaf(JPH_MeshShapeSettings* settings, [NativeTypeName("uint32_t")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MeshShapeSettings_GetActiveEdgeCosThresholdAngle([NativeTypeName("const JPH_MeshShapeSettings *")] JPH_MeshShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MeshShapeSettings_SetActiveEdgeCosThresholdAngle(JPH_MeshShapeSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_MeshShapeSettings_GetPerTriangleUserData([NativeTypeName("const JPH_MeshShapeSettings *")] JPH_MeshShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MeshShapeSettings_SetPerTriangleUserData(JPH_MeshShapeSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Mesh_Shape_BuildQuality JPH_MeshShapeSettings_GetBuildQuality([NativeTypeName("const JPH_MeshShapeSettings *")] JPH_MeshShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MeshShapeSettings_SetBuildQuality(JPH_MeshShapeSettings* settings, JPH_Mesh_Shape_BuildQuality value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MeshShapeSettings_Sanitize(JPH_MeshShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MeshShape* JPH_MeshShapeSettings_CreateShape([NativeTypeName("const JPH_MeshShapeSettings *")] JPH_MeshShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_MeshShape_GetTriangleUserData([NativeTypeName("const JPH_MeshShape *")] JPH_MeshShape* shape, [NativeTypeName("JPH_SubShapeID")] uint id);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_HeightFieldShapeSettings* JPH_HeightFieldShapeSettings_Create([NativeTypeName("const float *")] float* samples, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* offset, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale, [NativeTypeName("uint32_t")] uint sampleCount, [NativeTypeName("const uint8_t *")] byte* materialIndices);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_DetermineMinAndMaxSample([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* settings, float* pOutMinValue, float* pOutMaxValue, float* pOutQuantizationScale);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_HeightFieldShapeSettings_CalculateBitsPerSampleForError([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* settings, float maxError);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_GetOffset([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* shape, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_SetOffset(JPH_HeightFieldShapeSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_GetScale([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* shape, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_SetScale(JPH_HeightFieldShapeSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_HeightFieldShapeSettings_GetSampleCount([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_SetSampleCount(JPH_HeightFieldShapeSettings* settings, [NativeTypeName("uint32_t")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HeightFieldShapeSettings_GetMinHeightValue([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_SetMinHeightValue(JPH_HeightFieldShapeSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HeightFieldShapeSettings_GetMaxHeightValue([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_SetMaxHeightValue(JPH_HeightFieldShapeSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_HeightFieldShapeSettings_GetBlockSize([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_SetBlockSize(JPH_HeightFieldShapeSettings* settings, [NativeTypeName("uint32_t")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_HeightFieldShapeSettings_GetBitsPerSample([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_SetBitsPerSample(JPH_HeightFieldShapeSettings* settings, [NativeTypeName("uint32_t")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HeightFieldShapeSettings_GetActiveEdgeCosThresholdAngle([NativeTypeName("const JPH_HeightFieldShapeSettings *")] JPH_HeightFieldShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShapeSettings_SetActiveEdgeCosThresholdAngle(JPH_HeightFieldShapeSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_HeightFieldShape* JPH_HeightFieldShapeSettings_CreateShape(JPH_HeightFieldShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_HeightFieldShape_GetSampleCount([NativeTypeName("const JPH_HeightFieldShape *")] JPH_HeightFieldShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_HeightFieldShape_GetBlockSize([NativeTypeName("const JPH_HeightFieldShape *")] JPH_HeightFieldShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_PhysicsMaterial *")]
        public static extern JPH_PhysicsMaterial* JPH_HeightFieldShape_GetMaterial([NativeTypeName("const JPH_HeightFieldShape *")] JPH_HeightFieldShape* shape, [NativeTypeName("uint32_t")] uint x, [NativeTypeName("uint32_t")] uint y);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HeightFieldShape_GetPosition([NativeTypeName("const JPH_HeightFieldShape *")] JPH_HeightFieldShape* shape, [NativeTypeName("uint32_t")] uint x, [NativeTypeName("uint32_t")] uint y, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_HeightFieldShape_IsNoCollision([NativeTypeName("const JPH_HeightFieldShape *")] JPH_HeightFieldShape* shape, [NativeTypeName("uint32_t")] uint x, [NativeTypeName("uint32_t")] uint y);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_HeightFieldShape_ProjectOntoSurface([NativeTypeName("const JPH_HeightFieldShape *")] JPH_HeightFieldShape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* localPosition, JPH_Vec3* outSurfacePosition, [NativeTypeName("JPH_SubShapeID *")] uint* outSubShapeID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HeightFieldShape_GetMinHeightValue([NativeTypeName("const JPH_HeightFieldShape *")] JPH_HeightFieldShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HeightFieldShape_GetMaxHeightValue([NativeTypeName("const JPH_HeightFieldShape *")] JPH_HeightFieldShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TaperedCapsuleShapeSettings* JPH_TaperedCapsuleShapeSettings_Create(float halfHeightOfTaperedCylinder, float topRadius, float bottomRadius);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TaperedCapsuleShape* JPH_TaperedCapsuleShapeSettings_CreateShape(JPH_TaperedCapsuleShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TaperedCapsuleShape_GetTopRadius([NativeTypeName("const JPH_TaperedCapsuleShape *")] JPH_TaperedCapsuleShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TaperedCapsuleShape_GetBottomRadius([NativeTypeName("const JPH_TaperedCapsuleShape *")] JPH_TaperedCapsuleShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TaperedCapsuleShape_GetHalfHeight([NativeTypeName("const JPH_TaperedCapsuleShape *")] JPH_TaperedCapsuleShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CompoundShapeSettings_AddShape(JPH_CompoundShapeSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_ShapeSettings *")] JPH_ShapeSettings* shapeSettings, [NativeTypeName("uint32_t")] uint userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CompoundShapeSettings_AddShape2(JPH_CompoundShapeSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("uint32_t")] uint userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_CompoundShape_GetNumSubShapes([NativeTypeName("const JPH_CompoundShape *")] JPH_CompoundShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CompoundShape_GetSubShape([NativeTypeName("const JPH_CompoundShape *")] JPH_CompoundShape* shape, [NativeTypeName("uint32_t")] uint index, [NativeTypeName("const JPH_Shape **")] JPH_Shape** subShape, JPH_Vec3* positionCOM, JPH_Quat* rotation, [NativeTypeName("uint32_t *")] uint* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_CompoundShape_GetSubShapeIndexFromID([NativeTypeName("const JPH_CompoundShape *")] JPH_CompoundShape* shape, [NativeTypeName("JPH_SubShapeID")] uint id, [NativeTypeName("JPH_SubShapeID *")] uint* remainder);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_StaticCompoundShapeSettings* JPH_StaticCompoundShapeSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_StaticCompoundShape* JPH_StaticCompoundShape_Create([NativeTypeName("const JPH_StaticCompoundShapeSettings *")] JPH_StaticCompoundShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MutableCompoundShapeSettings* JPH_MutableCompoundShapeSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MutableCompoundShape* JPH_MutableCompoundShape_Create([NativeTypeName("const JPH_MutableCompoundShapeSettings *")] JPH_MutableCompoundShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_MutableCompoundShape_AddShape(JPH_MutableCompoundShape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_Shape *")] JPH_Shape* child, [NativeTypeName("uint32_t")] uint userData, [NativeTypeName("uint32_t")] uint index);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MutableCompoundShape_RemoveShape(JPH_MutableCompoundShape* shape, [NativeTypeName("uint32_t")] uint index);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MutableCompoundShape_ModifyShape(JPH_MutableCompoundShape* shape, [NativeTypeName("uint32_t")] uint index, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MutableCompoundShape_ModifyShape2(JPH_MutableCompoundShape* shape, [NativeTypeName("uint32_t")] uint index, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_Shape *")] JPH_Shape* newShape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MutableCompoundShape_AdjustCenterOfMass(JPH_MutableCompoundShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Shape *")]
        public static extern JPH_Shape* JPH_DecoratedShape_GetInnerShape([NativeTypeName("const JPH_DecoratedShape *")] JPH_DecoratedShape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_RotatedTranslatedShapeSettings* JPH_RotatedTranslatedShapeSettings_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_ShapeSettings *")] JPH_ShapeSettings* shapeSettings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_RotatedTranslatedShapeSettings* JPH_RotatedTranslatedShapeSettings_Create2([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_RotatedTranslatedShape* JPH_RotatedTranslatedShapeSettings_CreateShape([NativeTypeName("const JPH_RotatedTranslatedShapeSettings *")] JPH_RotatedTranslatedShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_RotatedTranslatedShape* JPH_RotatedTranslatedShape_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RotatedTranslatedShape_GetPosition([NativeTypeName("const JPH_RotatedTranslatedShape *")] JPH_RotatedTranslatedShape* shape, JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RotatedTranslatedShape_GetRotation([NativeTypeName("const JPH_RotatedTranslatedShape *")] JPH_RotatedTranslatedShape* shape, JPH_Quat* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ScaledShapeSettings* JPH_ScaledShapeSettings_Create([NativeTypeName("const JPH_ShapeSettings *")] JPH_ShapeSettings* shapeSettings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ScaledShapeSettings* JPH_ScaledShapeSettings_Create2([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ScaledShape* JPH_ScaledShapeSettings_CreateShape([NativeTypeName("const JPH_ScaledShapeSettings *")] JPH_ScaledShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ScaledShape* JPH_ScaledShape_Create([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ScaledShape_GetScale([NativeTypeName("const JPH_ScaledShape *")] JPH_ScaledShape* shape, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_OffsetCenterOfMassShapeSettings* JPH_OffsetCenterOfMassShapeSettings_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* offset, [NativeTypeName("const JPH_ShapeSettings *")] JPH_ShapeSettings* shapeSettings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_OffsetCenterOfMassShapeSettings* JPH_OffsetCenterOfMassShapeSettings_Create2([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* offset, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_OffsetCenterOfMassShape* JPH_OffsetCenterOfMassShapeSettings_CreateShape([NativeTypeName("const JPH_OffsetCenterOfMassShapeSettings *")] JPH_OffsetCenterOfMassShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_OffsetCenterOfMassShape* JPH_OffsetCenterOfMassShape_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* offset, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_OffsetCenterOfMassShape_GetOffset([NativeTypeName("const JPH_OffsetCenterOfMassShape *")] JPH_OffsetCenterOfMassShape* shape, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_EmptyShapeSettings* JPH_EmptyShapeSettings_Create([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* centerOfMass);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_EmptyShape* JPH_EmptyShapeSettings_CreateShape([NativeTypeName("const JPH_EmptyShapeSettings *")] JPH_EmptyShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyCreationSettings* JPH_BodyCreationSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyCreationSettings* JPH_BodyCreationSettings_Create2([NativeTypeName("const JPH_ShapeSettings *")] JPH_ShapeSettings* settings, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, JPH_MotionType motionType, [NativeTypeName("JPH_ObjectLayer")] uint objectLayer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyCreationSettings* JPH_BodyCreationSettings_Create3([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, JPH_MotionType motionType, [NativeTypeName("JPH_ObjectLayer")] uint objectLayer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_Destroy(JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_GetPosition(JPH_BodyCreationSettings* settings, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetPosition(JPH_BodyCreationSettings* settings, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_GetRotation(JPH_BodyCreationSettings* settings, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetRotation(JPH_BodyCreationSettings* settings, [NativeTypeName("const JPH_Quat *")] JPH_Quat* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_GetLinearVelocity(JPH_BodyCreationSettings* settings, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetLinearVelocity(JPH_BodyCreationSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_GetAngularVelocity(JPH_BodyCreationSettings* settings, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetAngularVelocity(JPH_BodyCreationSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong JPH_BodyCreationSettings_GetUserData([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetUserData(JPH_BodyCreationSettings* settings, [NativeTypeName("uint64_t")] ulong value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_ObjectLayer")]
        public static extern uint JPH_BodyCreationSettings_GetObjectLayer([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetObjectLayer(JPH_BodyCreationSettings* settings, [NativeTypeName("JPH_ObjectLayer")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_GetCollisionGroup([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings, JPH_CollisionGroup* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetCollisionGroup(JPH_BodyCreationSettings* settings, [NativeTypeName("const JPH_CollisionGroup *")] JPH_CollisionGroup* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotionType JPH_BodyCreationSettings_GetMotionType([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetMotionType(JPH_BodyCreationSettings* settings, JPH_MotionType value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_AllowedDOFs JPH_BodyCreationSettings_GetAllowedDOFs([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetAllowedDOFs(JPH_BodyCreationSettings* settings, JPH_AllowedDOFs value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyCreationSettings_GetAllowDynamicOrKinematic([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetAllowDynamicOrKinematic(JPH_BodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyCreationSettings_GetIsSensor([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetIsSensor(JPH_BodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyCreationSettings_GetCollideKinematicVsNonDynamic([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetCollideKinematicVsNonDynamic(JPH_BodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyCreationSettings_GetUseManifoldReduction([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetUseManifoldReduction(JPH_BodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyCreationSettings_GetApplyGyroscopicForce([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetApplyGyroscopicForce(JPH_BodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotionQuality JPH_BodyCreationSettings_GetMotionQuality([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetMotionQuality(JPH_BodyCreationSettings* settings, JPH_MotionQuality value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyCreationSettings_GetEnhancedInternalEdgeRemoval([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetEnhancedInternalEdgeRemoval(JPH_BodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyCreationSettings_GetAllowSleeping([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetAllowSleeping(JPH_BodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyCreationSettings_GetFriction([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetFriction(JPH_BodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyCreationSettings_GetRestitution([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetRestitution(JPH_BodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyCreationSettings_GetLinearDamping([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetLinearDamping(JPH_BodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyCreationSettings_GetAngularDamping([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetAngularDamping(JPH_BodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyCreationSettings_GetMaxLinearVelocity([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetMaxLinearVelocity(JPH_BodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyCreationSettings_GetMaxAngularVelocity([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetMaxAngularVelocity(JPH_BodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyCreationSettings_GetGravityFactor([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetGravityFactor(JPH_BodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_BodyCreationSettings_GetNumVelocityStepsOverride([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetNumVelocityStepsOverride(JPH_BodyCreationSettings* settings, [NativeTypeName("uint32_t")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_BodyCreationSettings_GetNumPositionStepsOverride([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetNumPositionStepsOverride(JPH_BodyCreationSettings* settings, [NativeTypeName("uint32_t")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_OverrideMassProperties JPH_BodyCreationSettings_GetOverrideMassProperties([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetOverrideMassProperties(JPH_BodyCreationSettings* settings, JPH_OverrideMassProperties value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyCreationSettings_GetInertiaMultiplier([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetInertiaMultiplier(JPH_BodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_GetMassPropertiesOverride([NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings, JPH_MassProperties* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyCreationSettings_SetMassPropertiesOverride(JPH_BodyCreationSettings* settings, [NativeTypeName("const JPH_MassProperties *")] JPH_MassProperties* massProperties);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SoftBodySharedSettings* JPH_SoftBodySharedSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodySharedSettings_Destroy(JPH_SoftBodySharedSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodySharedSettings_AddVertex(JPH_SoftBodySharedSettings* settings, [NativeTypeName("const JPH_SoftVertex *")] JPH_SoftVertex* vertex);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodySharedSettings_AddVertices(JPH_SoftBodySharedSettings* settings, [NativeTypeName("const JPH_SoftVertex *")] JPH_SoftVertex* vertices, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SoftBodySharedSettings_RemoveVertex(JPH_SoftBodySharedSettings* settings, [NativeTypeName("uint32_t")] uint index);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_SoftBodySharedSettings_GetVertexCount([NativeTypeName("const JPH_SoftBodySharedSettings *")] JPH_SoftBodySharedSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SoftBodySharedSettings_GetVertex([NativeTypeName("const JPH_SoftBodySharedSettings *")] JPH_SoftBodySharedSettings* settings, [NativeTypeName("uint32_t")] uint index, JPH_SoftVertex* outVertex);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodySharedSettings_AddFace(JPH_SoftBodySharedSettings* settings, [NativeTypeName("const JPH_SoftFace *")] JPH_SoftFace* face);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodySharedSettings_AddFaces(JPH_SoftBodySharedSettings* settings, [NativeTypeName("const JPH_SoftFace *")] JPH_SoftFace* faces, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SoftBodySharedSettings_RemoveFace(JPH_SoftBodySharedSettings* settings, [NativeTypeName("uint32_t")] uint index);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_SoftBodySharedSettings_GetFaceCount([NativeTypeName("const JPH_SoftBodySharedSettings *")] JPH_SoftBodySharedSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SoftBodySharedSettings_GetFace([NativeTypeName("const JPH_SoftBodySharedSettings *")] JPH_SoftBodySharedSettings* settings, [NativeTypeName("uint32_t")] uint index, JPH_SoftFace* outFace);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodySharedSettings_CreateConstraints(JPH_SoftBodySharedSettings* settings, float compliance, JPH_SoftBodyBendType bendType);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodySharedSettings_Optimize(JPH_SoftBodySharedSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SoftBodyCreationSettings* JPH_SoftBodyCreationSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SoftBodyCreationSettings* JPH_SoftBodyCreationSettings_Create2([NativeTypeName("const JPH_SoftBodySharedSettings *")] JPH_SoftBodySharedSettings* settings, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("JPH_ObjectLayer")] uint objectLayer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_Destroy(JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_SoftBodySharedSettings *")]
        public static extern JPH_SoftBodySharedSettings* JPH_SoftBodyCreationSettings_GetSettings([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetSettings(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("const JPH_SoftBodySharedSettings *")] JPH_SoftBodySharedSettings* sharedSettings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_GetPosition(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetPosition(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_GetRotation(JPH_SoftBodyCreationSettings* settings, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetRotation(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("const JPH_Quat *")] JPH_Quat* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong JPH_SoftBodyCreationSettings_GetUserData([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetUserData(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("uint64_t")] ulong userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_ObjectLayer")]
        public static extern uint JPH_SoftBodyCreationSettings_GetObjectLayer([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetObjectLayer(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("JPH_ObjectLayer")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_GetCollisionGroup([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings, JPH_CollisionGroup* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetCollisionGroup(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("const JPH_CollisionGroup *")] JPH_CollisionGroup* group);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_SoftBodyCreationSettings_GetNumIterations([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetNumIterations(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("uint32_t")] uint iterations);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SoftBodyCreationSettings_GetLinearDamping([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetLinearDamping(JPH_SoftBodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SoftBodyCreationSettings_GetMaxLinearVelocity([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetMaxLinearVelocity(JPH_SoftBodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SoftBodyCreationSettings_GetRestitution([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetRestitution(JPH_SoftBodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SoftBodyCreationSettings_GetFriction([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetFriction(JPH_SoftBodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SoftBodyCreationSettings_GetPressure([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetPressure(JPH_SoftBodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SoftBodyCreationSettings_GetGravityFactor([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetGravityFactor(JPH_SoftBodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SoftBodyCreationSettings_GetVertexRadius([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetVertexRadius(JPH_SoftBodyCreationSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SoftBodyCreationSettings_GetUpdatePosition([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetUpdatePosition(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SoftBodyCreationSettings_GetMakeRotationIdentity([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetMakeRotationIdentity(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SoftBodyCreationSettings_GetAllowSleeping([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetAllowSleeping(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SoftBodyCreationSettings_GetFacesDoubleSided([NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SoftBodyCreationSettings_SetFacesDoubleSided(JPH_SoftBodyCreationSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_Destroy(JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ConstraintType JPH_Constraint_GetType([NativeTypeName("const JPH_Constraint *")] JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ConstraintSubType JPH_Constraint_GetSubType([NativeTypeName("const JPH_Constraint *")] JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_Constraint_GetConstraintPriority([NativeTypeName("const JPH_Constraint *")] JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_SetConstraintPriority(JPH_Constraint* constraint, [NativeTypeName("uint32_t")] uint priority);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_Constraint_GetNumVelocityStepsOverride([NativeTypeName("const JPH_Constraint *")] JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_SetNumVelocityStepsOverride(JPH_Constraint* constraint, [NativeTypeName("uint32_t")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_Constraint_GetNumPositionStepsOverride([NativeTypeName("const JPH_Constraint *")] JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_SetNumPositionStepsOverride(JPH_Constraint* constraint, [NativeTypeName("uint32_t")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Constraint_GetEnabled([NativeTypeName("const JPH_Constraint *")] JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_SetEnabled(JPH_Constraint* constraint, [NativeTypeName("bool")] byte enabled);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong JPH_Constraint_GetUserData([NativeTypeName("const JPH_Constraint *")] JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_SetUserData(JPH_Constraint* constraint, [NativeTypeName("uint64_t")] ulong userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_NotifyShapeChanged(JPH_Constraint* constraint, [NativeTypeName("JPH_BodyID")] uint bodyID, JPH_Vec3* deltaCOM);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_ResetWarmStart(JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Constraint_IsActive([NativeTypeName("const JPH_Constraint *")] JPH_Constraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_SetupVelocityConstraint(JPH_Constraint* constraint, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Constraint_WarmStartVelocityConstraint(JPH_Constraint* constraint, float warmStartImpulseRatio);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Constraint_SolveVelocityConstraint(JPH_Constraint* constraint, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Constraint_SolvePositionConstraint(JPH_Constraint* constraint, float deltaTime, float baumgarte);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_TwoBodyConstraint_GetBody1([NativeTypeName("const JPH_TwoBodyConstraint *")] JPH_TwoBodyConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_TwoBodyConstraint_GetBody2([NativeTypeName("const JPH_TwoBodyConstraint *")] JPH_TwoBodyConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TwoBodyConstraint_GetConstraintToBody1Matrix([NativeTypeName("const JPH_TwoBodyConstraint *")] JPH_TwoBodyConstraint* constraint, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TwoBodyConstraint_GetConstraintToBody2Matrix([NativeTypeName("const JPH_TwoBodyConstraint *")] JPH_TwoBodyConstraint* constraint, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_FixedConstraintSettings_Init(JPH_FixedConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_FixedConstraint* JPH_FixedConstraint_Create([NativeTypeName("const JPH_FixedConstraintSettings *")] JPH_FixedConstraintSettings* settings, JPH_Body* body1, JPH_Body* body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_FixedConstraint_GetSettings([NativeTypeName("const JPH_FixedConstraint *")] JPH_FixedConstraint* constraint, JPH_FixedConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_FixedConstraint_GetTotalLambdaPosition([NativeTypeName("const JPH_FixedConstraint *")] JPH_FixedConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_FixedConstraint_GetTotalLambdaRotation([NativeTypeName("const JPH_FixedConstraint *")] JPH_FixedConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DistanceConstraintSettings_Init(JPH_DistanceConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_DistanceConstraint* JPH_DistanceConstraint_Create([NativeTypeName("const JPH_DistanceConstraintSettings *")] JPH_DistanceConstraintSettings* settings, JPH_Body* body1, JPH_Body* body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DistanceConstraint_GetSettings([NativeTypeName("const JPH_DistanceConstraint *")] JPH_DistanceConstraint* constraint, JPH_DistanceConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DistanceConstraint_SetDistance(JPH_DistanceConstraint* constraint, float minDistance, float maxDistance);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_DistanceConstraint_GetMinDistance(JPH_DistanceConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_DistanceConstraint_GetMaxDistance(JPH_DistanceConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DistanceConstraint_GetLimitsSpringSettings(JPH_DistanceConstraint* constraint, JPH_SpringSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DistanceConstraint_SetLimitsSpringSettings(JPH_DistanceConstraint* constraint, JPH_SpringSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_DistanceConstraint_GetTotalLambdaPosition([NativeTypeName("const JPH_DistanceConstraint *")] JPH_DistanceConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PointConstraintSettings_Init(JPH_PointConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PointConstraint* JPH_PointConstraint_Create([NativeTypeName("const JPH_PointConstraintSettings *")] JPH_PointConstraintSettings* settings, JPH_Body* body1, JPH_Body* body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PointConstraint_GetSettings([NativeTypeName("const JPH_PointConstraint *")] JPH_PointConstraint* constraint, JPH_PointConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PointConstraint_SetPoint1(JPH_PointConstraint* constraint, JPH_ConstraintSpace space, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PointConstraint_SetPoint2(JPH_PointConstraint* constraint, JPH_ConstraintSpace space, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PointConstraint_GetLocalSpacePoint1([NativeTypeName("const JPH_PointConstraint *")] JPH_PointConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PointConstraint_GetLocalSpacePoint2([NativeTypeName("const JPH_PointConstraint *")] JPH_PointConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_PointConstraint_GetTotalLambdaPosition([NativeTypeName("const JPH_PointConstraint *")] JPH_PointConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraintSettings_Init(JPH_HingeConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_HingeConstraint* JPH_HingeConstraint_Create([NativeTypeName("const JPH_HingeConstraintSettings *")] JPH_HingeConstraintSettings* settings, JPH_Body* body1, JPH_Body* body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetSettings(JPH_HingeConstraint* constraint, JPH_HingeConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetLocalSpacePoint1([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetLocalSpacePoint2([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetLocalSpaceHingeAxis1([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetLocalSpaceHingeAxis2([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetLocalSpaceNormalAxis1([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetLocalSpaceNormalAxis2([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HingeConstraint_GetCurrentAngle(JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_SetMaxFrictionTorque(JPH_HingeConstraint* constraint, float frictionTorque);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HingeConstraint_GetMaxFrictionTorque(JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_SetMotorSettings(JPH_HingeConstraint* constraint, JPH_MotorSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetMotorSettings(JPH_HingeConstraint* constraint, JPH_MotorSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_SetMotorState(JPH_HingeConstraint* constraint, JPH_MotorState state);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotorState JPH_HingeConstraint_GetMotorState(JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_SetTargetAngularVelocity(JPH_HingeConstraint* constraint, float angularVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HingeConstraint_GetTargetAngularVelocity(JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_SetTargetAngle(JPH_HingeConstraint* constraint, float angle);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HingeConstraint_GetTargetAngle(JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_SetLimits(JPH_HingeConstraint* constraint, float inLimitsMin, float inLimitsMax);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HingeConstraint_GetLimitsMin(JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HingeConstraint_GetLimitsMax(JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_HingeConstraint_HasLimits(JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetLimitsSpringSettings(JPH_HingeConstraint* constraint, JPH_SpringSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_SetLimitsSpringSettings(JPH_HingeConstraint* constraint, JPH_SpringSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetTotalLambdaPosition([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_HingeConstraint_GetTotalLambdaRotation([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint, [NativeTypeName("float[2]")] float* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HingeConstraint_GetTotalLambdaRotationLimits([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_HingeConstraint_GetTotalLambdaMotor([NativeTypeName("const JPH_HingeConstraint *")] JPH_HingeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraintSettings_Init(JPH_SliderConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraintSettings_SetSliderAxis(JPH_SliderConstraintSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SliderConstraint* JPH_SliderConstraint_Create([NativeTypeName("const JPH_SliderConstraintSettings *")] JPH_SliderConstraintSettings* settings, JPH_Body* body1, JPH_Body* body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_GetSettings(JPH_SliderConstraint* constraint, JPH_SliderConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SliderConstraint_GetCurrentPosition(JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_SetMaxFrictionForce(JPH_SliderConstraint* constraint, float frictionForce);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SliderConstraint_GetMaxFrictionForce(JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_SetMotorSettings(JPH_SliderConstraint* constraint, JPH_MotorSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_GetMotorSettings([NativeTypeName("const JPH_SliderConstraint *")] JPH_SliderConstraint* constraint, JPH_MotorSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_SetMotorState(JPH_SliderConstraint* constraint, JPH_MotorState state);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotorState JPH_SliderConstraint_GetMotorState(JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_SetTargetVelocity(JPH_SliderConstraint* constraint, float velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SliderConstraint_GetTargetVelocity(JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_SetTargetPosition(JPH_SliderConstraint* constraint, float position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SliderConstraint_GetTargetPosition(JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_SetLimits(JPH_SliderConstraint* constraint, float inLimitsMin, float inLimitsMax);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SliderConstraint_GetLimitsMin(JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SliderConstraint_GetLimitsMax(JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SliderConstraint_HasLimits(JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_GetLimitsSpringSettings(JPH_SliderConstraint* constraint, JPH_SpringSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_SetLimitsSpringSettings(JPH_SliderConstraint* constraint, JPH_SpringSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_GetTotalLambdaPosition([NativeTypeName("const JPH_SliderConstraint *")] JPH_SliderConstraint* constraint, [NativeTypeName("float[2]")] float* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SliderConstraint_GetTotalLambdaPositionLimits([NativeTypeName("const JPH_SliderConstraint *")] JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SliderConstraint_GetTotalLambdaRotation([NativeTypeName("const JPH_SliderConstraint *")] JPH_SliderConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SliderConstraint_GetTotalLambdaMotor([NativeTypeName("const JPH_SliderConstraint *")] JPH_SliderConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ConeConstraintSettings_Init(JPH_ConeConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ConeConstraint* JPH_ConeConstraint_Create([NativeTypeName("const JPH_ConeConstraintSettings *")] JPH_ConeConstraintSettings* settings, JPH_Body* body1, JPH_Body* body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ConeConstraint_GetSettings(JPH_ConeConstraint* constraint, JPH_ConeConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ConeConstraint_SetHalfConeAngle(JPH_ConeConstraint* constraint, float halfConeAngle);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_ConeConstraint_GetCosHalfConeAngle([NativeTypeName("const JPH_ConeConstraint *")] JPH_ConeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ConeConstraint_GetTotalLambdaPosition([NativeTypeName("const JPH_ConeConstraint *")] JPH_ConeConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_ConeConstraint_GetTotalLambdaRotation([NativeTypeName("const JPH_ConeConstraint *")] JPH_ConeConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SwingTwistConstraintSettings_Init(JPH_SwingTwistConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SwingTwistConstraint* JPH_SwingTwistConstraint_Create([NativeTypeName("const JPH_SwingTwistConstraintSettings *")] JPH_SwingTwistConstraintSettings* settings, JPH_Body* body1, JPH_Body* body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SwingTwistConstraint_GetSettings(JPH_SwingTwistConstraint* constraint, JPH_SwingTwistConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SwingTwistConstraint_GetNormalHalfConeAngle(JPH_SwingTwistConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SwingTwistConstraint_GetTotalLambdaPosition([NativeTypeName("const JPH_SwingTwistConstraint *")] JPH_SwingTwistConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SwingTwistConstraint_GetTotalLambdaTwist([NativeTypeName("const JPH_SwingTwistConstraint *")] JPH_SwingTwistConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SwingTwistConstraint_GetTotalLambdaSwingY([NativeTypeName("const JPH_SwingTwistConstraint *")] JPH_SwingTwistConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SwingTwistConstraint_GetTotalLambdaSwingZ([NativeTypeName("const JPH_SwingTwistConstraint *")] JPH_SwingTwistConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SwingTwistConstraint_GetTotalLambdaMotor([NativeTypeName("const JPH_SwingTwistConstraint *")] JPH_SwingTwistConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraintSettings_Init(JPH_SixDOFConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraintSettings_MakeFreeAxis(JPH_SixDOFConstraintSettings* settings, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SixDOFConstraintSettings_IsFreeAxis([NativeTypeName("const JPH_SixDOFConstraintSettings *")] JPH_SixDOFConstraintSettings* settings, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraintSettings_MakeFixedAxis(JPH_SixDOFConstraintSettings* settings, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SixDOFConstraintSettings_IsFixedAxis([NativeTypeName("const JPH_SixDOFConstraintSettings *")] JPH_SixDOFConstraintSettings* settings, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraintSettings_SetLimitedAxis(JPH_SixDOFConstraintSettings* settings, JPH_SixDOFConstraintAxis axis, float min, float max);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SixDOFConstraint* JPH_SixDOFConstraint_Create([NativeTypeName("const JPH_SixDOFConstraintSettings *")] JPH_SixDOFConstraintSettings* settings, JPH_Body* body1, JPH_Body* body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetSettings(JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SixDOFConstraint_GetLimitsMin(JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SixDOFConstraint_GetLimitsMax(JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTotalLambdaPosition([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTotalLambdaRotation([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTotalLambdaMotorTranslation([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTotalLambdaMotorRotation([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTranslationLimitsMin([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTranslationLimitsMax([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetRotationLimitsMin([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetRotationLimitsMax([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SixDOFConstraint_IsFixedAxis([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SixDOFConstraint_IsFreeAxis([NativeTypeName("const JPH_SixDOFConstraint *")] JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetLimitsSpringSettings(JPH_SixDOFConstraint* constraint, JPH_SpringSettings* result, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_SetLimitsSpringSettings(JPH_SixDOFConstraint* constraint, JPH_SpringSettings* settings, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_SetMaxFriction(JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintAxis axis, float inFriction);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SixDOFConstraint_GetMaxFriction(JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetRotationInConstraintSpace(JPH_SixDOFConstraint* constraint, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetMotorSettings(JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintAxis axis, JPH_MotorSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_SetMotorState(JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintAxis axis, JPH_MotorState state);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotorState JPH_SixDOFConstraint_GetMotorState(JPH_SixDOFConstraint* constraint, JPH_SixDOFConstraintAxis axis);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_SetTargetVelocityCS(JPH_SixDOFConstraint* constraint, JPH_Vec3* inVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTargetVelocityCS(JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_SetTargetAngularVelocityCS(JPH_SixDOFConstraint* constraint, JPH_Vec3* inAngularVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTargetAngularVelocityCS(JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_SetTargetPositionCS(JPH_SixDOFConstraint* constraint, JPH_Vec3* inPosition);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTargetPositionCS(JPH_SixDOFConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_SetTargetOrientationCS(JPH_SixDOFConstraint* constraint, JPH_Quat* inOrientation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_GetTargetOrientationCS(JPH_SixDOFConstraint* constraint, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SixDOFConstraint_SetTargetOrientationBS(JPH_SixDOFConstraint* constraint, JPH_Quat* inOrientation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_GearConstraintSettings_Init(JPH_GearConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_GearConstraint* JPH_GearConstraint_Create([NativeTypeName("const JPH_GearConstraintSettings *")] JPH_GearConstraintSettings* settings, JPH_Body* body1, JPH_Body* body2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_GearConstraint_GetSettings(JPH_GearConstraint* constraint, JPH_GearConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_GearConstraint_SetConstraints(JPH_GearConstraint* constraint, [NativeTypeName("const JPH_Constraint *")] JPH_Constraint* gear1, [NativeTypeName("const JPH_Constraint *")] JPH_Constraint* gear2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_GearConstraint_GetTotalLambda([NativeTypeName("const JPH_GearConstraint *")] JPH_GearConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_DestroyBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BodyID")]
        public static extern uint JPH_BodyInterface_CreateAndAddBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_BodyInterface_CreateBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_BodyInterface_CreateBodyWithID(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, [NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_BodyInterface_CreateBodyWithoutID(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_BodyCreationSettings *")] JPH_BodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_DestroyBodyWithoutID(JPH_BodyInterface* bodyInterface, JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyInterface_AssignBodyID(JPH_BodyInterface* bodyInterface, JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyInterface_AssignBodyID2(JPH_BodyInterface* bodyInterface, JPH_Body* body, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_BodyInterface_UnassignBodyID(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_BodyInterface_CreateSoftBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_BodyInterface_CreateSoftBodyWithID(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, [NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_BodyInterface_CreateSoftBodyWithoutID(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BodyID")]
        public static extern uint JPH_BodyInterface_CreateAndAddSoftBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_SoftBodyCreationSettings *")] JPH_SoftBodyCreationSettings* settings, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_RemoveBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_RemoveAndDestroyBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyInterface_IsAdded(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyType JPH_BodyInterface_GetBodyType(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Body *")]
        public static extern JPH_Body* JPH_PhysicsSystem_GetBodyPtr([NativeTypeName("const JPH_PhysicsSystem *")] JPH_PhysicsSystem* system, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetLinearVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetLinearVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetCenterOfMassPosition(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotionType JPH_BodyInterface_GetMotionType(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetMotionType(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, JPH_MotionType motionType, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyInterface_GetRestitution([NativeTypeName("const JPH_BodyInterface *")] JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetRestitution(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, float restitution);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyInterface_GetFriction([NativeTypeName("const JPH_BodyInterface *")] JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetFriction(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, float friction);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetPosition(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* position, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetPosition(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetRotation(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Quat* rotation, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetRotation(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetPositionAndRotation(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetPositionAndRotationWhenChanged(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetPositionAndRotation(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* position, JPH_Quat* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetPositionRotationAndVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* position, JPH_Quat* rotation, JPH_Vec3* linearVelocity, JPH_Vec3* angularVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetCollisionGroup(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_CollisionGroup* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetCollisionGroup(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("const JPH_CollisionGroup *")] JPH_CollisionGroup* group);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Shape *")]
        public static extern JPH_Shape* JPH_BodyInterface_GetShape(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetShape(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("bool")] byte updateMassProperties, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_NotifyShapeChanged(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* previousCenterOfMass, [NativeTypeName("bool")] byte updateMassProperties, JPH_Activation activationMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_ActivateBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_ActivateBodies(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_BodyID *")] uint* bodyIDs, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_ActivateBodiesInAABox(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_AABox *")] JPH_AABox* box, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_DeactivateBody(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_DeactivateBodies(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_BodyID *")] uint* bodyIDs, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyInterface_IsActive([NativeTypeName("const JPH_BodyInterface *")] JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_ResetSleepTimer(JPH_BodyInterface* bodyInterface, [NativeTypeName("const JPH_BodyID")] uint bodyID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_ObjectLayer")]
        public static extern uint JPH_BodyInterface_GetObjectLayer(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetObjectLayer(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_ObjectLayer")] uint layer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetWorldTransform(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetCenterOfMassTransform(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_MoveKinematic(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* targetPosition, JPH_Quat* targetRotation, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyInterface_ApplyBuoyancyImpulse(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* surfacePosition, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* surfaceNormal, float buoyancy, float linearDrag, float angularDrag, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* fluidVelocity, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* gravity, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetLinearAndAngularVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* linearVelocity, JPH_Vec3* angularVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetLinearAndAngularVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* linearVelocity, JPH_Vec3* angularVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddLinearVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* linearVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddLinearAndAngularVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* linearVelocity, JPH_Vec3* angularVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetAngularVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* angularVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetAngularVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* angularVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetPointVelocity(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* point, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddForce(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* force);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddForce2(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* force, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* point);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddTorque(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* torque);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddForceAndTorque(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* force, JPH_Vec3* torque);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddImpulse(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* impulse);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddImpulse2(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* impulse, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* point);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_AddAngularImpulse(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Vec3* angularImpulse);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetMotionQuality(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_MotionQuality quality);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotionQuality JPH_BodyInterface_GetMotionQuality(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_GetInverseInertia(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetGravityFactor(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_BodyInterface_GetGravityFactor(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetUseManifoldReduction(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyInterface_GetUseManifoldReduction(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetUserData(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("uint64_t")] ulong inUserData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong JPH_BodyInterface_GetUserData(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_SetIsSensor(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BodyInterface_IsSensor(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_PhysicsMaterial *")]
        public static extern JPH_PhysicsMaterial* JPH_BodyInterface_GetMaterial(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId, [NativeTypeName("JPH_SubShapeID")] uint subShapeID);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyInterface_InvalidateContactCache(JPH_BodyInterface* bodyInterface, [NativeTypeName("JPH_BodyID")] uint bodyId);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyLockInterface_LockRead([NativeTypeName("const JPH_BodyLockInterface *")] JPH_BodyLockInterface* lockInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, JPH_BodyLockRead* outLock);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyLockInterface_UnlockRead([NativeTypeName("const JPH_BodyLockInterface *")] JPH_BodyLockInterface* lockInterface, JPH_BodyLockRead* ioLock);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyLockInterface_LockWrite([NativeTypeName("const JPH_BodyLockInterface *")] JPH_BodyLockInterface* lockInterface, [NativeTypeName("JPH_BodyID")] uint bodyID, JPH_BodyLockWrite* outLock);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyLockInterface_UnlockWrite([NativeTypeName("const JPH_BodyLockInterface *")] JPH_BodyLockInterface* lockInterface, JPH_BodyLockWrite* ioLock);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyLockMultiRead* JPH_BodyLockInterface_LockMultiRead([NativeTypeName("const JPH_BodyLockInterface *")] JPH_BodyLockInterface* lockInterface, [NativeTypeName("const JPH_BodyID *")] uint* bodyIDs, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyLockMultiRead_Destroy(JPH_BodyLockMultiRead* ioLock);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Body *")]
        public static extern JPH_Body* JPH_BodyLockMultiRead_GetBody(JPH_BodyLockMultiRead* ioLock, [NativeTypeName("uint32_t")] uint bodyIndex);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyLockMultiWrite* JPH_BodyLockInterface_LockMultiWrite([NativeTypeName("const JPH_BodyLockInterface *")] JPH_BodyLockInterface* lockInterface, [NativeTypeName("const JPH_BodyID *")] uint* bodyIDs, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyLockMultiWrite_Destroy(JPH_BodyLockMultiWrite* ioLock);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_BodyLockMultiWrite_GetBody(JPH_BodyLockMultiWrite* ioLock, [NativeTypeName("uint32_t")] uint bodyIndex);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_AllowedDOFs JPH_MotionProperties_GetAllowedDOFs([NativeTypeName("const JPH_MotionProperties *")] JPH_MotionProperties* properties);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotionProperties_SetLinearDamping(JPH_MotionProperties* properties, float damping);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotionProperties_GetLinearDamping([NativeTypeName("const JPH_MotionProperties *")] JPH_MotionProperties* properties);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotionProperties_SetAngularDamping(JPH_MotionProperties* properties, float damping);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotionProperties_GetAngularDamping([NativeTypeName("const JPH_MotionProperties *")] JPH_MotionProperties* properties);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotionProperties_SetMassProperties(JPH_MotionProperties* properties, JPH_AllowedDOFs allowedDOFs, [NativeTypeName("const JPH_MassProperties *")] JPH_MassProperties* massProperties);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotionProperties_GetInverseMassUnchecked(JPH_MotionProperties* properties);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotionProperties_SetInverseMass(JPH_MotionProperties* properties, float inverseMass);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotionProperties_GetInverseInertiaDiagonal(JPH_MotionProperties* properties, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotionProperties_GetInertiaRotation(JPH_MotionProperties* properties, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotionProperties_SetInverseInertia(JPH_MotionProperties* properties, JPH_Vec3* diagonal, JPH_Quat* rot);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotionProperties_ScaleToMass(JPH_MotionProperties* properties, float mass);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RayCast_GetPointOnRay([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* origin, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, float fraction, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RRayCast_GetPointOnRay([NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* origin, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, float fraction, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MassProperties_DecomposePrincipalMomentsOfInertia(JPH_MassProperties* properties, JPH_Mat4* rotation, JPH_Vec3* diagonal);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MassProperties_ScaleToMass(JPH_MassProperties* properties, float mass);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MassProperties_GetEquivalentSolidBoxSize(float mass, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* inertiaDiagonal, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CollideShapeSettings_Init(JPH_CollideShapeSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ShapeCastSettings_Init(JPH_ShapeCastSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BroadPhaseQuery_CastRay([NativeTypeName("const JPH_BroadPhaseQuery *")] JPH_BroadPhaseQuery* query, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* origin, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, [NativeTypeName("JPH_RayCastBodyCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_BroadPhaseCastResult*, float> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BroadPhaseQuery_CastRay2([NativeTypeName("const JPH_BroadPhaseQuery *")] JPH_BroadPhaseQuery* query, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* origin, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, JPH_CollisionCollectorType collectorType, [NativeTypeName("JPH_RayCastBodyResultCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_BroadPhaseCastResult*, void> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BroadPhaseQuery_CollideAABox([NativeTypeName("const JPH_BroadPhaseQuery *")] JPH_BroadPhaseQuery* query, [NativeTypeName("const JPH_AABox *")] JPH_AABox* box, [NativeTypeName("JPH_CollideShapeBodyCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, uint, float> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BroadPhaseQuery_CollideSphere([NativeTypeName("const JPH_BroadPhaseQuery *")] JPH_BroadPhaseQuery* query, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* center, float radius, [NativeTypeName("JPH_CollideShapeBodyCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, uint, float> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_BroadPhaseQuery_CollidePoint([NativeTypeName("const JPH_BroadPhaseQuery *")] JPH_BroadPhaseQuery* query, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* point, [NativeTypeName("JPH_CollideShapeBodyCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, uint, float> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_NarrowPhaseQuery_CastRay([NativeTypeName("const JPH_NarrowPhaseQuery *")] JPH_NarrowPhaseQuery* query, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* origin, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, JPH_RayCastResult* hit, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_NarrowPhaseQuery_CastRay2([NativeTypeName("const JPH_NarrowPhaseQuery *")] JPH_NarrowPhaseQuery* query, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* origin, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, [NativeTypeName("const JPH_RayCastSettings *")] JPH_RayCastSettings* rayCastSettings, [NativeTypeName("JPH_CastRayCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_RayCastResult*, float> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_NarrowPhaseQuery_CastRay3([NativeTypeName("const JPH_NarrowPhaseQuery *")] JPH_NarrowPhaseQuery* query, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* origin, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, [NativeTypeName("const JPH_RayCastSettings *")] JPH_RayCastSettings* rayCastSettings, JPH_CollisionCollectorType collectorType, [NativeTypeName("JPH_CastRayResultCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_RayCastResult*, void> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_NarrowPhaseQuery_CollidePoint([NativeTypeName("const JPH_NarrowPhaseQuery *")] JPH_NarrowPhaseQuery* query, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* point, [NativeTypeName("JPH_CollidePointCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_CollidePointResult*, float> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_NarrowPhaseQuery_CollidePoint2([NativeTypeName("const JPH_NarrowPhaseQuery *")] JPH_NarrowPhaseQuery* query, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* point, JPH_CollisionCollectorType collectorType, [NativeTypeName("JPH_CollidePointResultCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_CollidePointResult*, void> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_NarrowPhaseQuery_CollideShape([NativeTypeName("const JPH_NarrowPhaseQuery *")] JPH_NarrowPhaseQuery* query, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* centerOfMassTransform, [NativeTypeName("const JPH_CollideShapeSettings *")] JPH_CollideShapeSettings* settings, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* baseOffset, [NativeTypeName("JPH_CollideShapeCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_CollideShapeResult*, float> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_NarrowPhaseQuery_CollideShape2([NativeTypeName("const JPH_NarrowPhaseQuery *")] JPH_NarrowPhaseQuery* query, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* centerOfMassTransform, [NativeTypeName("const JPH_CollideShapeSettings *")] JPH_CollideShapeSettings* settings, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* baseOffset, JPH_CollisionCollectorType collectorType, [NativeTypeName("JPH_CollideShapeResultCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_CollideShapeResult*, void> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_NarrowPhaseQuery_CastShape([NativeTypeName("const JPH_NarrowPhaseQuery *")] JPH_NarrowPhaseQuery* query, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* worldTransform, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, [NativeTypeName("const JPH_ShapeCastSettings *")] JPH_ShapeCastSettings* settings, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* baseOffset, [NativeTypeName("JPH_CastShapeCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_ShapeCastResult*, float> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_NarrowPhaseQuery_CastShape2([NativeTypeName("const JPH_NarrowPhaseQuery *")] JPH_NarrowPhaseQuery* query, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* worldTransform, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, [NativeTypeName("const JPH_ShapeCastSettings *")] JPH_ShapeCastSettings* settings, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* baseOffset, JPH_CollisionCollectorType collectorType, [NativeTypeName("JPH_CastShapeResultCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_ShapeCastResult*, void> callback, void* userData, [NativeTypeName("const JPH_BroadPhaseLayerFilter *")] JPH_BroadPhaseLayerFilter* broadPhaseLayerFilter, [NativeTypeName("const JPH_ObjectLayerFilter *")] JPH_ObjectLayerFilter* objectLayerFilter, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BodyID")]
        public static extern uint JPH_Body_GetID([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyType JPH_Body_GetBodyType([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_IsRigidBody([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_IsSoftBody([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_IsActive([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_IsStatic([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_IsKinematic([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_IsDynamic([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_CanBeKinematicOrDynamic([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetIsSensor(JPH_Body* body, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_IsSensor([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetCollideKinematicVsNonDynamic(JPH_Body* body, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_GetCollideKinematicVsNonDynamic([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetUseManifoldReduction(JPH_Body* body, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_GetUseManifoldReduction([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_GetUseManifoldReductionWithBody([NativeTypeName("const JPH_Body *")] JPH_Body* body, [NativeTypeName("const JPH_Body *")] JPH_Body* other);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetApplyGyroscopicForce(JPH_Body* body, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_GetApplyGyroscopicForce([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetEnhancedInternalEdgeRemoval(JPH_Body* body, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_GetEnhancedInternalEdgeRemoval([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_GetEnhancedInternalEdgeRemovalWithBody([NativeTypeName("const JPH_Body *")] JPH_Body* body, [NativeTypeName("const JPH_Body *")] JPH_Body* other);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotionType JPH_Body_GetMotionType([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetMotionType(JPH_Body* body, JPH_MotionType motionType);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BroadPhaseLayer")]
        public static extern byte JPH_Body_GetBroadPhaseLayer([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_ObjectLayer")]
        public static extern uint JPH_Body_GetObjectLayer([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetCollisionGroup([NativeTypeName("const JPH_Body *")] JPH_Body* body, JPH_CollisionGroup* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetCollisionGroup(JPH_Body* body, [NativeTypeName("const JPH_CollisionGroup *")] JPH_CollisionGroup* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_GetAllowSleeping(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetAllowSleeping(JPH_Body* body, [NativeTypeName("bool")] byte allowSleeping);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_ResetSleepTimer(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Body_GetFriction([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetFriction(JPH_Body* body, float friction);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Body_GetRestitution([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetRestitution(JPH_Body* body, float restitution);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetLinearVelocity(JPH_Body* body, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetLinearVelocity(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetLinearVelocityClamped(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetAngularVelocity(JPH_Body* body, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetAngularVelocity(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetAngularVelocityClamped(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetPointVelocityCOM(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* pointRelativeToCOM, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetPointVelocity(JPH_Body* body, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* point, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_AddForce(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* force);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_AddForceAtPosition(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* force, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_AddTorque(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* force);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetAccumulatedForce(JPH_Body* body, JPH_Vec3* force);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetAccumulatedTorque(JPH_Body* body, JPH_Vec3* force);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_ResetForce(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_ResetTorque(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_ResetMotion(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetInverseInertia(JPH_Body* body, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_AddImpulse(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* impulse);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_AddImpulseAtPosition(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* impulse, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_AddAngularImpulse(JPH_Body* body, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* angularImpulse);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_MoveKinematic(JPH_Body* body, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* targetPosition, JPH_Quat* targetRotation, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_ApplyBuoyancyImpulse(JPH_Body* body, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* surfacePosition, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* surfaceNormal, float buoyancy, float linearDrag, float angularDrag, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* fluidVelocity, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* gravity, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_IsInBroadPhase(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Body_IsCollisionCacheInvalid(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Shape *")]
        public static extern JPH_Shape* JPH_Body_GetShape(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetPosition([NativeTypeName("const JPH_Body *")] JPH_Body* body, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetRotation([NativeTypeName("const JPH_Body *")] JPH_Body* body, JPH_Quat* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetWorldTransform([NativeTypeName("const JPH_Body *")] JPH_Body* body, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetCenterOfMassPosition([NativeTypeName("const JPH_Body *")] JPH_Body* body, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetCenterOfMassTransform([NativeTypeName("const JPH_Body *")] JPH_Body* body, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetInverseCenterOfMassTransform([NativeTypeName("const JPH_Body *")] JPH_Body* body, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetWorldSpaceBounds([NativeTypeName("const JPH_Body *")] JPH_Body* body, JPH_AABox* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetWorldSpaceSurfaceNormal([NativeTypeName("const JPH_Body *")] JPH_Body* body, [NativeTypeName("JPH_SubShapeID")] uint subShapeID, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, JPH_Vec3* normal);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotionProperties* JPH_Body_GetMotionProperties(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotionProperties* JPH_Body_GetMotionPropertiesUnchecked(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_SetUserData(JPH_Body* body, [NativeTypeName("uint64_t")] ulong userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong JPH_Body_GetUserData(JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Body* JPH_Body_GetFixedToWorldBody();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_Body_GetSoftBodyVertexCount([NativeTypeName("const JPH_Body *")] JPH_Body* body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetSoftBodyVertexPosition([NativeTypeName("const JPH_Body *")] JPH_Body* body, [NativeTypeName("uint32_t")] uint index, JPH_Vec3* outPos);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Body_GetSoftBodyVertexPositions([NativeTypeName("const JPH_Body *")] JPH_Body* body, JPH_Vec3* outPositions, [NativeTypeName("uint32_t")] uint capacity, [NativeTypeName("uint32_t *")] uint* outCount);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BroadPhaseLayerFilter_SetProcs([NativeTypeName("const JPH_BroadPhaseLayerFilter_Procs *")] JPH_BroadPhaseLayerFilter_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BroadPhaseLayerFilter* JPH_BroadPhaseLayerFilter_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BroadPhaseLayerFilter_Destroy(JPH_BroadPhaseLayerFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ObjectLayerFilter_SetProcs([NativeTypeName("const JPH_ObjectLayerFilter_Procs *")] JPH_ObjectLayerFilter_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ObjectLayerFilter* JPH_ObjectLayerFilter_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ObjectLayerFilter_Destroy(JPH_ObjectLayerFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyFilter_SetProcs([NativeTypeName("const JPH_BodyFilter_Procs *")] JPH_BodyFilter_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyFilter* JPH_BodyFilter_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyFilter_Destroy(JPH_BodyFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ShapeFilter_SetProcs([NativeTypeName("const JPH_ShapeFilter_Procs *")] JPH_ShapeFilter_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ShapeFilter* JPH_ShapeFilter_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ShapeFilter_Destroy(JPH_ShapeFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BodyID")]
        public static extern uint JPH_ShapeFilter_GetBodyID2(JPH_ShapeFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ShapeFilter_SetBodyID2(JPH_ShapeFilter* filter, [NativeTypeName("JPH_BodyID")] uint id);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SimShapeFilter_SetProcs([NativeTypeName("const JPH_SimShapeFilter_Procs *")] JPH_SimShapeFilter_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SimShapeFilter* JPH_SimShapeFilter_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SimShapeFilter_Destroy(JPH_SimShapeFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ContactListener_SetProcs([NativeTypeName("const JPH_ContactListener_Procs *")] JPH_ContactListener_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_ContactListener* JPH_ContactListener_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ContactListener_Destroy(JPH_ContactListener* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyActivationListener_SetProcs([NativeTypeName("const JPH_BodyActivationListener_Procs *")] JPH_BodyActivationListener_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyActivationListener* JPH_BodyActivationListener_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyActivationListener_Destroy(JPH_BodyActivationListener* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyDrawFilter_SetProcs([NativeTypeName("const JPH_BodyDrawFilter_Procs *")] JPH_BodyDrawFilter_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_BodyDrawFilter* JPH_BodyDrawFilter_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_BodyDrawFilter_Destroy(JPH_BodyDrawFilter* filter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ContactManifold_GetWorldSpaceNormal([NativeTypeName("const JPH_ContactManifold *")] JPH_ContactManifold* manifold, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_ContactManifold_GetPenetrationDepth([NativeTypeName("const JPH_ContactManifold *")] JPH_ContactManifold* manifold);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_SubShapeID")]
        public static extern uint JPH_ContactManifold_GetSubShapeID1([NativeTypeName("const JPH_ContactManifold *")] JPH_ContactManifold* manifold);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_SubShapeID")]
        public static extern uint JPH_ContactManifold_GetSubShapeID2([NativeTypeName("const JPH_ContactManifold *")] JPH_ContactManifold* manifold);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_ContactManifold_GetPointCount([NativeTypeName("const JPH_ContactManifold *")] JPH_ContactManifold* manifold);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ContactManifold_GetWorldSpaceContactPointOn1([NativeTypeName("const JPH_ContactManifold *")] JPH_ContactManifold* manifold, [NativeTypeName("uint32_t")] uint index, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ContactManifold_GetWorldSpaceContactPointOn2([NativeTypeName("const JPH_ContactManifold *")] JPH_ContactManifold* manifold, [NativeTypeName("uint32_t")] uint index, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterBase_Destroy(JPH_CharacterBase* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CharacterBase_GetCosMaxSlopeAngle(JPH_CharacterBase* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterBase_SetMaxSlopeAngle(JPH_CharacterBase* character, float maxSlopeAngle);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterBase_GetUp(JPH_CharacterBase* character, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterBase_SetUp(JPH_CharacterBase* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterBase_IsSlopeTooSteep(JPH_CharacterBase* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Shape *")]
        public static extern JPH_Shape* JPH_CharacterBase_GetShape(JPH_CharacterBase* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_GroundState JPH_CharacterBase_GetGroundState(JPH_CharacterBase* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterBase_IsSupported(JPH_CharacterBase* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterBase_GetGroundPosition(JPH_CharacterBase* character, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterBase_GetGroundNormal(JPH_CharacterBase* character, JPH_Vec3* normal);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterBase_GetGroundVelocity(JPH_CharacterBase* character, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_PhysicsMaterial *")]
        public static extern JPH_PhysicsMaterial* JPH_CharacterBase_GetGroundMaterial(JPH_CharacterBase* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BodyID")]
        public static extern uint JPH_CharacterBase_GetGroundBodyId(JPH_CharacterBase* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_SubShapeID")]
        public static extern uint JPH_CharacterBase_GetGroundSubShapeId(JPH_CharacterBase* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong JPH_CharacterBase_GetGroundUserData(JPH_CharacterBase* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterSettings_Init(JPH_CharacterSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Character* JPH_Character_Create([NativeTypeName("const JPH_CharacterSettings *")] JPH_CharacterSettings* settings, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("uint64_t")] ulong userData, JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_AddToPhysicsSystem(JPH_Character* character, JPH_Activation activationMode, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_RemoveFromPhysicsSystem(JPH_Character* character, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_Activate(JPH_Character* character, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_PostSimulation(JPH_Character* character, float maxSeparationDistance, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_SetLinearAndAngularVelocity(JPH_Character* character, JPH_Vec3* linearVelocity, JPH_Vec3* angularVelocity, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_GetLinearVelocity(JPH_Character* character, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_SetLinearVelocity(JPH_Character* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_AddLinearVelocity(JPH_Character* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_AddImpulse(JPH_Character* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BodyID")]
        public static extern uint JPH_Character_GetBodyID([NativeTypeName("const JPH_Character *")] JPH_Character* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_GetPositionAndRotation(JPH_Character* character, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* position, JPH_Quat* rotation, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_SetPositionAndRotation(JPH_Character* character, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, JPH_Activation activationMode, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_GetPosition(JPH_Character* character, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_SetPosition(JPH_Character* character, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, JPH_Activation activationMode, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_GetRotation(JPH_Character* character, JPH_Quat* rotation, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_SetRotation(JPH_Character* character, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, JPH_Activation activationMode, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_GetCenterOfMassPosition(JPH_Character* character, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_GetWorldTransform(JPH_Character* character, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* result, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_ObjectLayer")]
        public static extern uint JPH_Character_GetLayer([NativeTypeName("const JPH_Character *")] JPH_Character* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_SetLayer(JPH_Character* character, [NativeTypeName("JPH_ObjectLayer")] uint value, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Character_SetShape(JPH_Character* character, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, float maxPenetrationDepth, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtualSettings_Init(JPH_CharacterVirtualSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CharacterVirtual* JPH_CharacterVirtual_Create([NativeTypeName("const JPH_CharacterVirtualSettings *")] JPH_CharacterVirtualSettings* settings, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("uint64_t")] ulong userData, JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_CharacterID")]
        public static extern uint JPH_CharacterVirtual_GetID([NativeTypeName("const JPH_CharacterVirtual *")] JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetListener(JPH_CharacterVirtual* character, JPH_CharacterContactListener* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetCharacterVsCharacterCollision(JPH_CharacterVirtual* character, JPH_CharacterVsCharacterCollision* characterVsCharacterCollision);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_GetLinearVelocity(JPH_CharacterVirtual* character, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetLinearVelocity(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_GetPosition(JPH_CharacterVirtual* character, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetPosition(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_GetRotation(JPH_CharacterVirtual* character, JPH_Quat* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetRotation(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_GetWorldTransform(JPH_CharacterVirtual* character, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_GetCenterOfMassTransform(JPH_CharacterVirtual* character, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CharacterVirtual_GetMass(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetMass(JPH_CharacterVirtual* character, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CharacterVirtual_GetMaxStrength(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetMaxStrength(JPH_CharacterVirtual* character, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CharacterVirtual_GetPenetrationRecoverySpeed(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetPenetrationRecoverySpeed(JPH_CharacterVirtual* character, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterVirtual_GetEnhancedInternalEdgeRemoval(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetEnhancedInternalEdgeRemoval(JPH_CharacterVirtual* character, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CharacterVirtual_GetCharacterPadding(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_CharacterVirtual_GetMaxNumHits(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetMaxNumHits(JPH_CharacterVirtual* character, [NativeTypeName("uint32_t")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_CharacterVirtual_GetHitReductionCosMaxAngle(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetHitReductionCosMaxAngle(JPH_CharacterVirtual* character, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterVirtual_GetMaxHitsExceeded(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_GetShapeOffset(JPH_CharacterVirtual* character, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetShapeOffset(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong JPH_CharacterVirtual_GetUserData([NativeTypeName("const JPH_CharacterVirtual *")] JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetUserData(JPH_CharacterVirtual* character, [NativeTypeName("uint64_t")] ulong value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BodyID")]
        public static extern uint JPH_CharacterVirtual_GetInnerBodyID([NativeTypeName("const JPH_CharacterVirtual *")] JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_CancelVelocityTowardsSteepSlopes(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* desiredVelocity, JPH_Vec3* velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_StartTrackingContactChanges(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_FinishTrackingContactChanges(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_Update(JPH_CharacterVirtual* character, float deltaTime, [NativeTypeName("JPH_ObjectLayer")] uint layer, JPH_PhysicsSystem* system, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter, JPH_TempAllocator* tempAllocator);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_ExtendedUpdate(JPH_CharacterVirtual* character, float deltaTime, [NativeTypeName("const JPH_ExtendedUpdateSettings *")] JPH_ExtendedUpdateSettings* settings, [NativeTypeName("JPH_ObjectLayer")] uint layer, JPH_PhysicsSystem* system, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter, JPH_TempAllocator* tempAllocator);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_RefreshContacts(JPH_CharacterVirtual* character, [NativeTypeName("JPH_ObjectLayer")] uint layer, JPH_PhysicsSystem* system, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter, JPH_TempAllocator* tempAllocator);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterVirtual_CanWalkStairs(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* linearVelocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterVirtual_WalkStairs(JPH_CharacterVirtual* character, float deltaTime, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* stepUp, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* stepForward, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* stepForwardTest, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* stepDownExtra, [NativeTypeName("JPH_ObjectLayer")] uint layer, JPH_PhysicsSystem* system, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter, JPH_TempAllocator* tempAllocator);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterVirtual_StickToFloor(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* stepDown, [NativeTypeName("JPH_ObjectLayer")] uint layer, JPH_PhysicsSystem* system, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter, JPH_TempAllocator* tempAllocator);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_UpdateGroundVelocity(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterVirtual_SetShape(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, float maxPenetrationDepth, [NativeTypeName("JPH_ObjectLayer")] uint layer, JPH_PhysicsSystem* system, [NativeTypeName("const JPH_BodyFilter *")] JPH_BodyFilter* bodyFilter, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter, JPH_TempAllocator* tempAllocator);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_SetInnerBodyShape(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_CharacterVirtual_GetNumActiveContacts(JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVirtual_GetActiveContact(JPH_CharacterVirtual* character, [NativeTypeName("uint32_t")] uint index, JPH_CharacterVirtualContact* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterVirtual_HasCollidedWithBody(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_BodyID")] uint body);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterVirtual_HasCollidedWith(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_CharacterID")] uint other);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CharacterVirtual_HasCollidedWithCharacter(JPH_CharacterVirtual* character, [NativeTypeName("const JPH_CharacterVirtual *")] JPH_CharacterVirtual* other);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterContactListener_SetProcs([NativeTypeName("const JPH_CharacterContactListener_Procs *")] JPH_CharacterContactListener_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CharacterContactListener* JPH_CharacterContactListener_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterContactListener_Destroy(JPH_CharacterContactListener* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVsCharacterCollision_SetProcs([NativeTypeName("const JPH_CharacterVsCharacterCollision_Procs *")] JPH_CharacterVsCharacterCollision_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CharacterVsCharacterCollision* JPH_CharacterVsCharacterCollision_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_CharacterVsCharacterCollision* JPH_CharacterVsCharacterCollision_CreateSimple();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVsCharacterCollisionSimple_AddCharacter(JPH_CharacterVsCharacterCollision* characterVsCharacter, JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVsCharacterCollisionSimple_RemoveCharacter(JPH_CharacterVsCharacterCollision* characterVsCharacter, JPH_CharacterVirtual* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_CharacterVsCharacterCollision_Destroy(JPH_CharacterVsCharacterCollision* listener);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CollisionDispatch_CollideShapeVsShape([NativeTypeName("const JPH_Shape *")] JPH_Shape* shape1, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape2, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale2, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* centerOfMassTransform1, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* centerOfMassTransform2, [NativeTypeName("const JPH_CollideShapeSettings *")] JPH_CollideShapeSettings* collideShapeSettings, [NativeTypeName("JPH_CollideShapeCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_CollideShapeResult*, float> callback, void* userData, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CollisionDispatch_CastShapeVsShapeLocalSpace([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape1, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape2, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale1InShape2LocalSpace, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale2, JPH_Mat4* centerOfMassTransform1InShape2LocalSpace, JPH_Mat4* centerOfMassWorldTransform2, [NativeTypeName("const JPH_ShapeCastSettings *")] JPH_ShapeCastSettings* shapeCastSettings, [NativeTypeName("JPH_CastShapeCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_ShapeCastResult*, float> callback, void* userData, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_CollisionDispatch_CastShapeVsShapeWorldSpace([NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* direction, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape1, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape2, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale1, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* inScale2, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* centerOfMassWorldTransform1, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* centerOfMassWorldTransform2, [NativeTypeName("const JPH_ShapeCastSettings *")] JPH_ShapeCastSettings* shapeCastSettings, [NativeTypeName("JPH_CastShapeCollectorCallback *")] delegate* unmanaged[Cdecl]<void*, JPH_ShapeCastResult*, float> callback, void* userData, [NativeTypeName("const JPH_ShapeFilter *")] JPH_ShapeFilter* shapeFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_SetProcs([NativeTypeName("const JPH_DebugRenderer_Procs *")] JPH_DebugRenderer_Procs* procs);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_DebugRenderer* JPH_DebugRenderer_Create(void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_Destroy(JPH_DebugRenderer* renderer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_NextFrame(JPH_DebugRenderer* renderer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_SetCameraPos(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawLine(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* from, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* to, [NativeTypeName("JPH_Color")] uint color);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawWireBox(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_AABox *")] JPH_AABox* box, [NativeTypeName("JPH_Color")] uint color);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawWireBox2(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* matrix, [NativeTypeName("const JPH_AABox *")] JPH_AABox* box, [NativeTypeName("JPH_Color")] uint color);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawMarker(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("JPH_Color")] uint color, float size);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawArrow(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* from, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* to, [NativeTypeName("JPH_Color")] uint color, float size);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawCoordinateSystem(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* matrix, float size);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawPlane(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* point, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* normal, [NativeTypeName("JPH_Color")] uint color, float size);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawWireTriangle(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* v2, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* v3, [NativeTypeName("JPH_Color")] uint color);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawWireSphere(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* center, float radius, [NativeTypeName("JPH_Color")] uint color, int level);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawWireUnitSphere(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* matrix, [NativeTypeName("JPH_Color")] uint color, int level);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawTriangle(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* v2, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* v3, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawBox(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_AABox *")] JPH_AABox* box, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawBox2(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* matrix, [NativeTypeName("const JPH_AABox *")] JPH_AABox* box, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawSphere(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* center, float radius, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawUnitSphere(JPH_DebugRenderer* renderer, [NativeTypeName("JPH_RMat4")] JPH_Mat4 matrix, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawCapsule(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* matrix, float halfHeightOfCylinder, float radius, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawCylinder(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* matrix, float halfHeight, float radius, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawOpenCone(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* top, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* axis, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* perpendicular, float halfAngle, float length, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawSwingConeLimits(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* matrix, float swingYHalfAngle, float swingZHalfAngle, float edgeLength, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawSwingPyramidLimits(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* matrix, float minSwingYAngle, float maxSwingYAngle, float minSwingZAngle, float maxSwingZAngle, float edgeLength, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawPie(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* center, float radius, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* normal, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* axis, float minAngle, float maxAngle, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_DebugRenderer_DrawTaperedCylinder(JPH_DebugRenderer* renderer, [NativeTypeName("const JPH_RMat4 *")] JPH_Mat4* inMatrix, float top, float bottom, float topRadius, float bottomRadius, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Skeleton* JPH_Skeleton_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Skeleton_Destroy(JPH_Skeleton* skeleton);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_Skeleton_AddJoint(JPH_Skeleton* skeleton, [NativeTypeName("const char *")] sbyte* name);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_Skeleton_AddJoint2(JPH_Skeleton* skeleton, [NativeTypeName("const char *")] sbyte* name, int parentIndex);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_Skeleton_AddJoint3(JPH_Skeleton* skeleton, [NativeTypeName("const char *")] sbyte* name, [NativeTypeName("const char *")] sbyte* parentName);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_Skeleton_GetJointCount([NativeTypeName("const JPH_Skeleton *")] JPH_Skeleton* skeleton);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Skeleton_GetJoint([NativeTypeName("const JPH_Skeleton *")] JPH_Skeleton* skeleton, int index, JPH_SkeletonJoint* joint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_Skeleton_GetJointIndex([NativeTypeName("const JPH_Skeleton *")] JPH_Skeleton* skeleton, [NativeTypeName("const char *")] sbyte* name);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Skeleton_CalculateParentJointIndices(JPH_Skeleton* skeleton);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Skeleton_AreJointsCorrectlyOrdered([NativeTypeName("const JPH_Skeleton *")] JPH_Skeleton* skeleton);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SkeletonPose* JPH_SkeletonPose_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_Destroy(JPH_SkeletonPose* pose);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_SetSkeleton(JPH_SkeletonPose* pose, [NativeTypeName("const JPH_Skeleton *")] JPH_Skeleton* skeleton);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Skeleton *")]
        public static extern JPH_Skeleton* JPH_SkeletonPose_GetSkeleton([NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_SetRootOffset(JPH_SkeletonPose* pose, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* offset);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_GetRootOffset([NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_SkeletonPose_GetJointCount([NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_GetJointState([NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose, int index, JPH_Vec3* outTranslation, JPH_Quat* outRotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_SetJointState(JPH_SkeletonPose* pose, int index, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* translation, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_GetJointMatrix([NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose, int index, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_SetJointMatrix(JPH_SkeletonPose* pose, int index, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* matrix);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_GetJointMatrices([NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose, JPH_Mat4* outMatrices, int count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_SetJointMatrices(JPH_SkeletonPose* pose, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* matrices, int count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_CalculateJointMatrices(JPH_SkeletonPose* pose);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_CalculateJointStates(JPH_SkeletonPose* pose);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonPose_CalculateLocalSpaceJointMatrices([NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose, JPH_Mat4* outMatrices);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SkeletalAnimation* JPH_SkeletalAnimation_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletalAnimation_Destroy(JPH_SkeletalAnimation* animation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_SkeletalAnimation_GetDuration([NativeTypeName("const JPH_SkeletalAnimation *")] JPH_SkeletalAnimation* animation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SkeletalAnimation_IsLooping([NativeTypeName("const JPH_SkeletalAnimation *")] JPH_SkeletalAnimation* animation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletalAnimation_SetIsLooping(JPH_SkeletalAnimation* animation, [NativeTypeName("bool")] byte looping);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletalAnimation_ScaleJoints(JPH_SkeletalAnimation* animation, float scale);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletalAnimation_Sample([NativeTypeName("const JPH_SkeletalAnimation *")] JPH_SkeletalAnimation* animation, float time, JPH_SkeletonPose* pose);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_SkeletalAnimation_GetAnimatedJointCount([NativeTypeName("const JPH_SkeletalAnimation *")] JPH_SkeletalAnimation* animation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletalAnimation_AddAnimatedJoint(JPH_SkeletalAnimation* animation, [NativeTypeName("const char *")] sbyte* jointName);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletalAnimation_AddKeyframe(JPH_SkeletalAnimation* animation, int jointIndex, float time, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* translation, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_SkeletonMapper* JPH_SkeletonMapper_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonMapper_Destroy(JPH_SkeletonMapper* mapper);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonMapper_Initialize(JPH_SkeletonMapper* mapper, [NativeTypeName("const JPH_Skeleton *")] JPH_Skeleton* skeleton1, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* neutralPose1, [NativeTypeName("const JPH_Skeleton *")] JPH_Skeleton* skeleton2, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* neutralPose2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonMapper_LockAllTranslations(JPH_SkeletonMapper* mapper, [NativeTypeName("const JPH_Skeleton *")] JPH_Skeleton* skeleton2, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* neutralPose2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonMapper_LockTranslations(JPH_SkeletonMapper* mapper, [NativeTypeName("const JPH_Skeleton *")] JPH_Skeleton* skeleton2, [NativeTypeName("const bool *")] bool* lockedTranslations, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* neutralPose2);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonMapper_Map([NativeTypeName("const JPH_SkeletonMapper *")] JPH_SkeletonMapper* mapper, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* pose1ModelSpace, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* pose2LocalSpace, JPH_Mat4* outPose2ModelSpace);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_SkeletonMapper_MapReverse([NativeTypeName("const JPH_SkeletonMapper *")] JPH_SkeletonMapper* mapper, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* pose2ModelSpace, JPH_Mat4* outPose1ModelSpace);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_SkeletonMapper_GetMappedJointIndex([NativeTypeName("const JPH_SkeletonMapper *")] JPH_SkeletonMapper* mapper, int joint1Index);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_SkeletonMapper_IsJointTranslationLocked([NativeTypeName("const JPH_SkeletonMapper *")] JPH_SkeletonMapper* mapper, int joint2Index);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_RagdollSettings* JPH_RagdollSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_Destroy(JPH_RagdollSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Skeleton *")]
        public static extern JPH_Skeleton* JPH_RagdollSettings_GetSkeleton([NativeTypeName("const JPH_RagdollSettings *")] JPH_RagdollSettings* character);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_SetSkeleton(JPH_RagdollSettings* character, JPH_Skeleton* skeleton);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_RagdollSettings_Stabilize(JPH_RagdollSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_DisableParentChildCollisions(JPH_RagdollSettings* settings, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* jointMatrices, float minSeparationDistance);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_CalculateBodyIndexToConstraintIndex(JPH_RagdollSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_RagdollSettings_GetConstraintIndexForBodyIndex(JPH_RagdollSettings* settings, int bodyIndex);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_CalculateConstraintIndexToBodyIdxPair(JPH_RagdollSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_ResizeParts(JPH_RagdollSettings* settings, int count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_RagdollSettings_GetPartCount([NativeTypeName("const JPH_RagdollSettings *")] JPH_RagdollSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_SetPartShape(JPH_RagdollSettings* settings, int partIndex, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_SetPartPosition(JPH_RagdollSettings* settings, int partIndex, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_SetPartRotation(JPH_RagdollSettings* settings, int partIndex, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_SetPartMotionType(JPH_RagdollSettings* settings, int partIndex, JPH_MotionType motionType);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_SetPartObjectLayer(JPH_RagdollSettings* settings, int partIndex, [NativeTypeName("JPH_ObjectLayer")] uint layer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_SetPartMassProperties(JPH_RagdollSettings* settings, int partIndex, float mass);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_RagdollSettings_SetPartToParent(JPH_RagdollSettings* settings, int partIndex, [NativeTypeName("const JPH_SwingTwistConstraintSettings *")] JPH_SwingTwistConstraintSettings* constraintSettings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Ragdoll* JPH_RagdollSettings_CreateRagdoll(JPH_RagdollSettings* settings, JPH_PhysicsSystem* system, [NativeTypeName("JPH_CollisionGroupID")] uint collisionGroup, [NativeTypeName("uint64_t")] ulong userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_Destroy(JPH_Ragdoll* ragdoll);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_AddToPhysicsSystem(JPH_Ragdoll* ragdoll, JPH_Activation activationMode, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_RemoveFromPhysicsSystem(JPH_Ragdoll* ragdoll, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_Activate(JPH_Ragdoll* ragdoll, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Ragdoll_IsActive([NativeTypeName("const JPH_Ragdoll *")] JPH_Ragdoll* ragdoll, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_ResetWarmStart(JPH_Ragdoll* ragdoll);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_SetPose(JPH_Ragdoll* ragdoll, [NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_SetPose2(JPH_Ragdoll* ragdoll, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* rootOffset, [NativeTypeName("const JPH_Mat4 *")] JPH_Mat4* jointMatrices, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_GetPose([NativeTypeName("const JPH_Ragdoll *")] JPH_Ragdoll* ragdoll, JPH_SkeletonPose* outPose, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_GetPose2([NativeTypeName("const JPH_Ragdoll *")] JPH_Ragdoll* ragdoll, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* outRootOffset, JPH_Mat4* outJointMatrices, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_DriveToPoseUsingMotors(JPH_Ragdoll* ragdoll, [NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_DriveToPoseUsingKinematics(JPH_Ragdoll* ragdoll, [NativeTypeName("const JPH_SkeletonPose *")] JPH_SkeletonPose* pose, float deltaTime, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_Ragdoll_GetBodyCount([NativeTypeName("const JPH_Ragdoll *")] JPH_Ragdoll* ragdoll);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BodyID")]
        public static extern uint JPH_Ragdoll_GetBodyID([NativeTypeName("const JPH_Ragdoll *")] JPH_Ragdoll* ragdoll, int bodyIndex);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_Ragdoll_GetConstraintCount([NativeTypeName("const JPH_Ragdoll *")] JPH_Ragdoll* ragdoll);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TwoBodyConstraint* JPH_Ragdoll_GetConstraint(JPH_Ragdoll* ragdoll, int constraintIndex);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Ragdoll_GetRootTransform([NativeTypeName("const JPH_Ragdoll *")] JPH_Ragdoll* ragdoll, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* outPosition, JPH_Quat* outRotation, [NativeTypeName("bool")] byte lockBodies);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_RagdollSettings *")]
        public static extern JPH_RagdollSettings* JPH_Ragdoll_GetRagdollSettings([NativeTypeName("const JPH_Ragdoll *")] JPH_Ragdoll* ragdoll);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_EstimateCollisionResponse([NativeTypeName("const JPH_Body *")] JPH_Body* body1, [NativeTypeName("const JPH_Body *")] JPH_Body* body2, [NativeTypeName("const JPH_ContactManifold *")] JPH_ContactManifold* manifold, float combinedFriction, float combinedRestitution, float minVelocityForRestitution, [NativeTypeName("uint32_t")] uint numIterations, JPH_CollisionEstimationResult* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraintSettings_Init(JPH_VehicleConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_VehicleConstraint* JPH_VehicleConstraint_Create(JPH_Body* body, [NativeTypeName("const JPH_VehicleConstraintSettings *")] JPH_VehicleConstraintSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PhysicsStepListener* JPH_VehicleConstraint_AsPhysicsStepListener(JPH_VehicleConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_SetMaxPitchRollAngle(JPH_VehicleConstraint* constraint, float maxPitchRollAngle);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_SetVehicleCollisionTester(JPH_VehicleConstraint* constraint, [NativeTypeName("const JPH_VehicleCollisionTester *")] JPH_VehicleCollisionTester* tester);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_OverrideGravity(JPH_VehicleConstraint* constraint, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_VehicleConstraint_IsGravityOverridden([NativeTypeName("const JPH_VehicleConstraint *")] JPH_VehicleConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_GetGravityOverride([NativeTypeName("const JPH_VehicleConstraint *")] JPH_VehicleConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_ResetGravityOverride(JPH_VehicleConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_GetLocalForward([NativeTypeName("const JPH_VehicleConstraint *")] JPH_VehicleConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_GetLocalUp([NativeTypeName("const JPH_VehicleConstraint *")] JPH_VehicleConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_GetWorldUp([NativeTypeName("const JPH_VehicleConstraint *")] JPH_VehicleConstraint* constraint, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_Body *")]
        public static extern JPH_Body* JPH_VehicleConstraint_GetVehicleBody([NativeTypeName("const JPH_VehicleConstraint *")] JPH_VehicleConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_VehicleController* JPH_VehicleConstraint_GetController(JPH_VehicleConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_VehicleConstraint_GetWheelsCount(JPH_VehicleConstraint* constraint);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Wheel* JPH_VehicleConstraint_GetWheel(JPH_VehicleConstraint* constraint, [NativeTypeName("uint32_t")] uint index);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_GetWheelLocalBasis(JPH_VehicleConstraint* constraint, [NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel, JPH_Vec3* outForward, JPH_Vec3* outUp, JPH_Vec3* outRight);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_GetWheelLocalTransform(JPH_VehicleConstraint* constraint, [NativeTypeName("uint32_t")] uint wheelIndex, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* wheelRight, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* wheelUp, JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleConstraint_GetWheelWorldTransform(JPH_VehicleConstraint* constraint, [NativeTypeName("uint32_t")] uint wheelIndex, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* wheelRight, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* wheelUp, [NativeTypeName("JPH_RMat4 *")] JPH_Mat4* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_WheelSettings* JPH_WheelSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_Destroy(JPH_WheelSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_GetPosition([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetPosition(JPH_WheelSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_GetSuspensionForcePoint([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetSuspensionForcePoint(JPH_WheelSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_GetSuspensionDirection([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetSuspensionDirection(JPH_WheelSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_GetSteeringAxis([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetSteeringAxis(JPH_WheelSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_GetWheelUp([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetWheelUp(JPH_WheelSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_GetWheelForward([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetWheelForward(JPH_WheelSettings* settings, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettings_GetSuspensionMinLength([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetSuspensionMinLength(JPH_WheelSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettings_GetSuspensionMaxLength([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetSuspensionMaxLength(JPH_WheelSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettings_GetSuspensionPreloadLength([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetSuspensionPreloadLength(JPH_WheelSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_GetSuspensionSpring([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings, JPH_SpringSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetSuspensionSpring(JPH_WheelSettings* settings, JPH_SpringSettings* springSettings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettings_GetRadius([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetRadius(JPH_WheelSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettings_GetWidth([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetWidth(JPH_WheelSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_WheelSettings_GetEnableSuspensionForcePoint([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettings_SetEnableSuspensionForcePoint(JPH_WheelSettings* settings, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Wheel* JPH_Wheel_Create([NativeTypeName("const JPH_WheelSettings *")] JPH_WheelSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Wheel_Destroy(JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_WheelSettings *")]
        public static extern JPH_WheelSettings* JPH_Wheel_GetSettings([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Wheel_GetAngularVelocity([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Wheel_SetAngularVelocity(JPH_Wheel* wheel, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Wheel_GetRotationAngle([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Wheel_SetRotationAngle(JPH_Wheel* wheel, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Wheel_GetSteerAngle([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Wheel_SetSteerAngle(JPH_Wheel* wheel, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Wheel_HasContact([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_BodyID")]
        public static extern uint JPH_Wheel_GetContactBodyID([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_SubShapeID")]
        public static extern uint JPH_Wheel_GetContactSubShapeID([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Wheel_GetContactPosition([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel, [NativeTypeName("JPH_RVec3 *")] JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Wheel_GetContactPointVelocity([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Wheel_GetContactNormal([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Wheel_GetContactLongitudinal([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Wheel_GetContactLateral([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel, JPH_Vec3* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Wheel_GetSuspensionLength([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Wheel_GetSuspensionLambda([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Wheel_GetLongitudinalLambda([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_Wheel_GetLateralLambda([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Wheel_HasHitHardPoint([NativeTypeName("const JPH_Wheel *")] JPH_Wheel* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleAntiRollBar_Init(JPH_VehicleAntiRollBar* antiRollBar);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleEngineSettings_Init(JPH_VehicleEngineSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleEngine_ClampRPM(JPH_VehicleEngine* engine);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleEngine_GetCurrentRPM([NativeTypeName("const JPH_VehicleEngine *")] JPH_VehicleEngine* engine);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleEngine_SetCurrentRPM(JPH_VehicleEngine* engine, float rpm);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleEngine_GetAngularVelocity([NativeTypeName("const JPH_VehicleEngine *")] JPH_VehicleEngine* engine);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleEngine_GetTorque([NativeTypeName("const JPH_VehicleEngine *")] JPH_VehicleEngine* engine, float acceleration);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleEngine_ApplyTorque(JPH_VehicleEngine* engine, float torque, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleEngine_ApplyDamping(JPH_VehicleEngine* engine, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_VehicleEngine_AllowSleep([NativeTypeName("const JPH_VehicleEngine *")] JPH_VehicleEngine* engine);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleDifferentialSettings_Init(JPH_VehicleDifferentialSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_VehicleTransmissionSettings* JPH_VehicleTransmissionSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_Destroy(JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TransmissionMode JPH_VehicleTransmissionSettings_GetMode([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetMode(JPH_VehicleTransmissionSettings* settings, JPH_TransmissionMode value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_VehicleTransmissionSettings_GetGearRatioCount([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmissionSettings_GetGearRatio([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings, [NativeTypeName("uint32_t")] uint index);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetGearRatio(JPH_VehicleTransmissionSettings* settings, [NativeTypeName("uint32_t")] uint index, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const float *")]
        public static extern float* JPH_VehicleTransmissionSettings_GetGearRatios([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetGearRatios(JPH_VehicleTransmissionSettings* settings, [NativeTypeName("const float *")] float* values, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_VehicleTransmissionSettings_GetReverseGearRatioCount([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmissionSettings_GetReverseGearRatio([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings, [NativeTypeName("uint32_t")] uint index);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetReverseGearRatio(JPH_VehicleTransmissionSettings* settings, [NativeTypeName("uint32_t")] uint index, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const float *")]
        public static extern float* JPH_VehicleTransmissionSettings_GetReverseGearRatios([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetReverseGearRatios(JPH_VehicleTransmissionSettings* settings, [NativeTypeName("const float *")] float* values, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmissionSettings_GetSwitchTime([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetSwitchTime(JPH_VehicleTransmissionSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmissionSettings_GetClutchReleaseTime([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetClutchReleaseTime(JPH_VehicleTransmissionSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmissionSettings_GetSwitchLatency([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetSwitchLatency(JPH_VehicleTransmissionSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmissionSettings_GetShiftUpRPM([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetShiftUpRPM(JPH_VehicleTransmissionSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmissionSettings_GetShiftDownRPM([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetShiftDownRPM(JPH_VehicleTransmissionSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmissionSettings_GetClutchStrength([NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmissionSettings_SetClutchStrength(JPH_VehicleTransmissionSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmission_SetMode(JPH_VehicleTransmission* transmission, JPH_TransmissionMode mode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmission_Set(JPH_VehicleTransmission* transmission, int currentGear, float clutchFriction);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTransmission_Update(JPH_VehicleTransmission* transmission, float deltaTime, float currentRPM, float forwardInput, [NativeTypeName("bool")] byte canShiftUp);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int JPH_VehicleTransmission_GetCurrentGear([NativeTypeName("const JPH_VehicleTransmission *")] JPH_VehicleTransmission* transmission);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmission_GetClutchFriction([NativeTypeName("const JPH_VehicleTransmission *")] JPH_VehicleTransmission* transmission);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_VehicleTransmission_IsSwitchingGear([NativeTypeName("const JPH_VehicleTransmission *")] JPH_VehicleTransmission* transmission);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTransmission_GetCurrentRatio([NativeTypeName("const JPH_VehicleTransmission *")] JPH_VehicleTransmission* transmission);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_VehicleTransmission_AllowSleep([NativeTypeName("const JPH_VehicleTransmission *")] JPH_VehicleTransmission* transmission);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleCollisionTester_Destroy(JPH_VehicleCollisionTester* tester);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("JPH_ObjectLayer")]
        public static extern uint JPH_VehicleCollisionTester_GetObjectLayer([NativeTypeName("const JPH_VehicleCollisionTester *")] JPH_VehicleCollisionTester* tester);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleCollisionTester_SetObjectLayer(JPH_VehicleCollisionTester* tester, [NativeTypeName("JPH_ObjectLayer")] uint value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_VehicleCollisionTesterRay* JPH_VehicleCollisionTesterRay_Create([NativeTypeName("JPH_ObjectLayer")] uint layer, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* up, float maxSlopeAngle);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_VehicleCollisionTesterCastSphere* JPH_VehicleCollisionTesterCastSphere_Create([NativeTypeName("JPH_ObjectLayer")] uint layer, float radius, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* up, float maxSlopeAngle);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_VehicleCollisionTesterCastCylinder* JPH_VehicleCollisionTesterCastCylinder_Create([NativeTypeName("JPH_ObjectLayer")] uint layer, float convexRadiusFraction);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleControllerSettings_Destroy(JPH_VehicleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_VehicleConstraint *")]
        public static extern JPH_VehicleConstraint* JPH_VehicleController_GetConstraint(JPH_VehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_WheelSettingsWV* JPH_WheelSettingsWV_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettingsWV_GetInertia([NativeTypeName("const JPH_WheelSettingsWV *")] JPH_WheelSettingsWV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettingsWV_SetInertia(JPH_WheelSettingsWV* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettingsWV_GetAngularDamping([NativeTypeName("const JPH_WheelSettingsWV *")] JPH_WheelSettingsWV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettingsWV_SetAngularDamping(JPH_WheelSettingsWV* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettingsWV_GetMaxSteerAngle([NativeTypeName("const JPH_WheelSettingsWV *")] JPH_WheelSettingsWV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettingsWV_SetMaxSteerAngle(JPH_WheelSettingsWV* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_LinearCurve *")]
        public static extern JPH_LinearCurve* JPH_WheelSettingsWV_GetLongitudinalFriction([NativeTypeName("const JPH_WheelSettingsWV *")] JPH_WheelSettingsWV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettingsWV_SetLongitudinalFriction(JPH_WheelSettingsWV* settings, [NativeTypeName("const JPH_LinearCurve *")] JPH_LinearCurve* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_LinearCurve *")]
        public static extern JPH_LinearCurve* JPH_WheelSettingsWV_GetLateralFriction([NativeTypeName("const JPH_WheelSettingsWV *")] JPH_WheelSettingsWV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettingsWV_SetLateralFriction(JPH_WheelSettingsWV* settings, [NativeTypeName("const JPH_LinearCurve *")] JPH_LinearCurve* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettingsWV_GetMaxBrakeTorque([NativeTypeName("const JPH_WheelSettingsWV *")] JPH_WheelSettingsWV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettingsWV_SetMaxBrakeTorque(JPH_WheelSettingsWV* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettingsWV_GetMaxHandBrakeTorque([NativeTypeName("const JPH_WheelSettingsWV *")] JPH_WheelSettingsWV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettingsWV_SetMaxHandBrakeTorque(JPH_WheelSettingsWV* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_WheelWV* JPH_WheelWV_Create([NativeTypeName("const JPH_WheelSettingsWV *")] JPH_WheelSettingsWV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_WheelSettingsWV *")]
        public static extern JPH_WheelSettingsWV* JPH_WheelWV_GetSettings([NativeTypeName("const JPH_WheelWV *")] JPH_WheelWV* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelWV_ApplyTorque(JPH_WheelWV* wheel, float torque, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_WheeledVehicleControllerSettings* JPH_WheeledVehicleControllerSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleControllerSettings_GetEngine([NativeTypeName("const JPH_WheeledVehicleControllerSettings *")] JPH_WheeledVehicleControllerSettings* settings, JPH_VehicleEngineSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleControllerSettings_SetEngine(JPH_WheeledVehicleControllerSettings* settings, [NativeTypeName("const JPH_VehicleEngineSettings *")] JPH_VehicleEngineSettings* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_VehicleTransmissionSettings *")]
        public static extern JPH_VehicleTransmissionSettings* JPH_WheeledVehicleControllerSettings_GetTransmission([NativeTypeName("const JPH_WheeledVehicleControllerSettings *")] JPH_WheeledVehicleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleControllerSettings_SetTransmission(JPH_WheeledVehicleControllerSettings* settings, [NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_WheeledVehicleControllerSettings_GetDifferentialsCount([NativeTypeName("const JPH_WheeledVehicleControllerSettings *")] JPH_WheeledVehicleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleControllerSettings_SetDifferentialsCount(JPH_WheeledVehicleControllerSettings* settings, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleControllerSettings_GetDifferential([NativeTypeName("const JPH_WheeledVehicleControllerSettings *")] JPH_WheeledVehicleControllerSettings* settings, [NativeTypeName("uint32_t")] uint index, JPH_VehicleDifferentialSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleControllerSettings_SetDifferential(JPH_WheeledVehicleControllerSettings* settings, [NativeTypeName("uint32_t")] uint index, [NativeTypeName("const JPH_VehicleDifferentialSettings *")] JPH_VehicleDifferentialSettings* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleControllerSettings_SetDifferentials(JPH_WheeledVehicleControllerSettings* settings, [NativeTypeName("const JPH_VehicleDifferentialSettings *")] JPH_VehicleDifferentialSettings* values, [NativeTypeName("uint32_t")] uint count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleControllerSettings_AddDifferential(JPH_WheeledVehicleControllerSettings* settings, int leftWheel, int rightWheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheeledVehicleControllerSettings_GetDifferentialLimitedSlipRatio([NativeTypeName("const JPH_WheeledVehicleControllerSettings *")] JPH_WheeledVehicleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleControllerSettings_SetDifferentialLimitedSlipRatio(JPH_WheeledVehicleControllerSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleController_SetDriverInput(JPH_WheeledVehicleController* controller, float forward, float right, float brake, float handBrake);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleController_SetForwardInput(JPH_WheeledVehicleController* controller, float forward);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheeledVehicleController_GetForwardInput([NativeTypeName("const JPH_WheeledVehicleController *")] JPH_WheeledVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleController_SetRightInput(JPH_WheeledVehicleController* controller, float rightRatio);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheeledVehicleController_GetRightInput([NativeTypeName("const JPH_WheeledVehicleController *")] JPH_WheeledVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleController_SetBrakeInput(JPH_WheeledVehicleController* controller, float brakeInput);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheeledVehicleController_GetBrakeInput([NativeTypeName("const JPH_WheeledVehicleController *")] JPH_WheeledVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleController_SetHandBrakeInput(JPH_WheeledVehicleController* controller, float handBrakeInput);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheeledVehicleController_GetHandBrakeInput([NativeTypeName("const JPH_WheeledVehicleController *")] JPH_WheeledVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheeledVehicleController_GetWheelSpeedAtClutch([NativeTypeName("const JPH_WheeledVehicleController *")] JPH_WheeledVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheeledVehicleController_SetTireMaxImpulseCallback(JPH_WheeledVehicleController* controller, [NativeTypeName("JPH_TireMaxImpulseCallback")] delegate* unmanaged[Cdecl]<void*, uint, float*, float*, float, float, float, float, float, float, void> tireMaxImpulseCallback, void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_VehicleEngine *")]
        public static extern JPH_VehicleEngine* JPH_WheeledVehicleController_GetEngine([NativeTypeName("const JPH_WheeledVehicleController *")] JPH_WheeledVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_VehicleTransmission *")]
        public static extern JPH_VehicleTransmission* JPH_WheeledVehicleController_GetTransmission([NativeTypeName("const JPH_WheeledVehicleController *")] JPH_WheeledVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTrackSettings_Init(JPH_VehicleTrackSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTrack_GetAngularVelocity([NativeTypeName("const JPH_VehicleTrack *")] JPH_VehicleTrack* track);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_VehicleTrack_SetAngularVelocity(JPH_VehicleTrack* track, float velocity);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_VehicleTrack_GetDrivenWheel([NativeTypeName("const JPH_VehicleTrack *")] JPH_VehicleTrack* track);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTrack_GetInertia([NativeTypeName("const JPH_VehicleTrack *")] JPH_VehicleTrack* track);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTrack_GetAngularDamping([NativeTypeName("const JPH_VehicleTrack *")] JPH_VehicleTrack* track);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTrack_GetMaxBrakeTorque([NativeTypeName("const JPH_VehicleTrack *")] JPH_VehicleTrack* track);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_VehicleTrack_GetDifferentialRatio([NativeTypeName("const JPH_VehicleTrack *")] JPH_VehicleTrack* track);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_VehicleTrack *")]
        public static extern JPH_VehicleTrack* JPH_TrackedVehicleController_GetTrack([NativeTypeName("const JPH_TrackedVehicleController *")] JPH_TrackedVehicleController* controller, JPH_TrackSide side);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_WheelSettingsTV* JPH_WheelSettingsTV_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettingsTV_GetLongitudinalFriction([NativeTypeName("const JPH_WheelSettingsTV *")] JPH_WheelSettingsTV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettingsTV_SetLongitudinalFriction(JPH_WheelSettingsTV* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_WheelSettingsTV_GetLateralFriction([NativeTypeName("const JPH_WheelSettingsTV *")] JPH_WheelSettingsTV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_WheelSettingsTV_SetLateralFriction(JPH_WheelSettingsTV* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_WheelTV* JPH_WheelTV_Create([NativeTypeName("const JPH_WheelSettingsTV *")] JPH_WheelSettingsTV* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_WheelSettingsTV *")]
        public static extern JPH_WheelSettingsTV* JPH_WheelTV_GetSettings([NativeTypeName("const JPH_WheelTV *")] JPH_WheelTV* wheel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_TrackedVehicleControllerSettings* JPH_TrackedVehicleControllerSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TrackedVehicleControllerSettings_GetEngine([NativeTypeName("const JPH_TrackedVehicleControllerSettings *")] JPH_TrackedVehicleControllerSettings* settings, JPH_VehicleEngineSettings* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TrackedVehicleControllerSettings_SetEngine(JPH_TrackedVehicleControllerSettings* settings, [NativeTypeName("const JPH_VehicleEngineSettings *")] JPH_VehicleEngineSettings* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_VehicleTransmissionSettings *")]
        public static extern JPH_VehicleTransmissionSettings* JPH_TrackedVehicleControllerSettings_GetTransmission([NativeTypeName("const JPH_TrackedVehicleControllerSettings *")] JPH_TrackedVehicleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TrackedVehicleControllerSettings_SetTransmission(JPH_TrackedVehicleControllerSettings* settings, [NativeTypeName("const JPH_VehicleTransmissionSettings *")] JPH_VehicleTransmissionSettings* value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TrackedVehicleControllerSettings_SetTrack(JPH_TrackedVehicleControllerSettings* settings, [NativeTypeName("uint32_t")] uint index, [NativeTypeName("const JPH_VehicleTrackSettings *")] JPH_VehicleTrackSettings* track);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TrackedVehicleController_SetDriverInput(JPH_TrackedVehicleController* controller, float forward, float leftRatio, float rightRatio, float brake);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TrackedVehicleController_GetForwardInput([NativeTypeName("const JPH_TrackedVehicleController *")] JPH_TrackedVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TrackedVehicleController_SetForwardInput(JPH_TrackedVehicleController* controller, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TrackedVehicleController_GetLeftRatio([NativeTypeName("const JPH_TrackedVehicleController *")] JPH_TrackedVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TrackedVehicleController_SetLeftRatio(JPH_TrackedVehicleController* controller, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TrackedVehicleController_GetRightRatio([NativeTypeName("const JPH_TrackedVehicleController *")] JPH_TrackedVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TrackedVehicleController_SetRightRatio(JPH_TrackedVehicleController* controller, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_TrackedVehicleController_GetBrakeInput([NativeTypeName("const JPH_TrackedVehicleController *")] JPH_TrackedVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_TrackedVehicleController_SetBrakeInput(JPH_TrackedVehicleController* controller, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_VehicleEngine *")]
        public static extern JPH_VehicleEngine* JPH_TrackedVehicleController_GetEngine([NativeTypeName("const JPH_TrackedVehicleController *")] JPH_TrackedVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const JPH_VehicleTransmission *")]
        public static extern JPH_VehicleTransmission* JPH_TrackedVehicleController_GetTransmission([NativeTypeName("const JPH_TrackedVehicleController *")] JPH_TrackedVehicleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_MotorcycleControllerSettings* JPH_MotorcycleControllerSettings_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleControllerSettings_GetMaxLeanAngle([NativeTypeName("const JPH_MotorcycleControllerSettings *")] JPH_MotorcycleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleControllerSettings_SetMaxLeanAngle(JPH_MotorcycleControllerSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleControllerSettings_GetLeanSpringConstant([NativeTypeName("const JPH_MotorcycleControllerSettings *")] JPH_MotorcycleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleControllerSettings_SetLeanSpringConstant(JPH_MotorcycleControllerSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleControllerSettings_GetLeanSpringDamping([NativeTypeName("const JPH_MotorcycleControllerSettings *")] JPH_MotorcycleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleControllerSettings_SetLeanSpringDamping(JPH_MotorcycleControllerSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleControllerSettings_GetLeanSpringIntegrationCoefficient([NativeTypeName("const JPH_MotorcycleControllerSettings *")] JPH_MotorcycleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleControllerSettings_SetLeanSpringIntegrationCoefficient(JPH_MotorcycleControllerSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleControllerSettings_GetLeanSpringIntegrationCoefficientDecay([NativeTypeName("const JPH_MotorcycleControllerSettings *")] JPH_MotorcycleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleControllerSettings_SetLeanSpringIntegrationCoefficientDecay(JPH_MotorcycleControllerSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleControllerSettings_GetLeanSmoothingFactor([NativeTypeName("const JPH_MotorcycleControllerSettings *")] JPH_MotorcycleControllerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleControllerSettings_SetLeanSmoothingFactor(JPH_MotorcycleControllerSettings* settings, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleController_GetWheelBase([NativeTypeName("const JPH_MotorcycleController *")] JPH_MotorcycleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_MotorcycleController_IsLeanControllerEnabled([NativeTypeName("const JPH_MotorcycleController *")] JPH_MotorcycleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleController_EnableLeanController(JPH_MotorcycleController* controller, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_MotorcycleController_IsLeanSteeringLimitEnabled([NativeTypeName("const JPH_MotorcycleController *")] JPH_MotorcycleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleController_EnableLeanSteeringLimit(JPH_MotorcycleController* controller, [NativeTypeName("bool")] byte value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleController_GetLeanSpringConstant([NativeTypeName("const JPH_MotorcycleController *")] JPH_MotorcycleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleController_SetLeanSpringConstant(JPH_MotorcycleController* controller, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleController_GetLeanSpringDamping([NativeTypeName("const JPH_MotorcycleController *")] JPH_MotorcycleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleController_SetLeanSpringDamping(JPH_MotorcycleController* controller, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleController_GetLeanSpringIntegrationCoefficient([NativeTypeName("const JPH_MotorcycleController *")] JPH_MotorcycleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleController_SetLeanSpringIntegrationCoefficient(JPH_MotorcycleController* controller, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleController_GetLeanSpringIntegrationCoefficientDecay([NativeTypeName("const JPH_MotorcycleController *")] JPH_MotorcycleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleController_SetLeanSpringIntegrationCoefficientDecay(JPH_MotorcycleController* controller, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_MotorcycleController_GetLeanSmoothingFactor([NativeTypeName("const JPH_MotorcycleController *")] JPH_MotorcycleController* controller);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_MotorcycleController_SetLeanSmoothingFactor(JPH_MotorcycleController* controller, float value);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_LinearCurve* JPH_LinearCurve_Create();

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_LinearCurve_Destroy(JPH_LinearCurve* curve);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_LinearCurve_Clear(JPH_LinearCurve* curve);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_LinearCurve_Reserve(JPH_LinearCurve* curve, [NativeTypeName("uint32_t")] uint numPoints);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_LinearCurve_AddPoint(JPH_LinearCurve* curve, float x, float y);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_LinearCurve_Sort(JPH_LinearCurve* curve);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_LinearCurve_GetMinX([NativeTypeName("const JPH_LinearCurve *")] JPH_LinearCurve* curve);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_LinearCurve_GetMaxX([NativeTypeName("const JPH_LinearCurve *")] JPH_LinearCurve* curve);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float JPH_LinearCurve_GetValue([NativeTypeName("const JPH_LinearCurve *")] JPH_LinearCurve* curve, float x);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint JPH_LinearCurve_GetPointCount([NativeTypeName("const JPH_LinearCurve *")] JPH_LinearCurve* curve);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_LinearCurve_GetPoint([NativeTypeName("const JPH_LinearCurve *")] JPH_LinearCurve* curve, [NativeTypeName("uint32_t")] uint index, JPH_Point* result);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_LinearCurve_GetPoints([NativeTypeName("const JPH_LinearCurve *")] JPH_LinearCurve* curve, JPH_Point* points, [NativeTypeName("uint32_t *")] uint* count);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_PhysicsUpdateError JPH_PhysicsSystem_Update2(JPH_PhysicsSystem* system, float deltaTime, int collisionSteps, JPH_TempAllocator* tempAllocator, JPH_JobSystem* jobSystem);
    }
}
