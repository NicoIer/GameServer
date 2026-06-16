// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.InteropServices;

namespace JoltPhysics
{
    /// <summary>
    /// Managed mirror of JPH_PhysicsSettings — the Jolt simulation tuning parameters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct JoltPhysicsSettings
    {
        public int MaxInFlightBodyPairs;
        public int StepListenersBatchSize;
        public int StepListenerBatchesPerJob;
        public float Baumgarte;
        public float SpeculativeContactDistance;
        public float PenetrationSlop;
        public float LinearCastThreshold;
        public float LinearCastMaxPenetration;
        public float ManifoldTolerance;
        public float MaxPenetrationDistance;
        public float BodyPairCacheMaxDeltaPositionSq;
        public float BodyPairCacheCosMaxDeltaRotationDiv2;
        public float ContactNormalCosMaxDeltaRotation;
        public float ContactPointPreserveLambdaMaxDistSq;
        public uint NumVelocitySteps;
        public uint NumPositionSteps;
        public float MinVelocityForRestitution;
        public float TimeBeforeSleep;
        public float PointVelocitySleepThreshold;
        public bool DeterministicSimulation;
        public bool ConstraintWarmStart;
        public bool UseBodyPairContactCache;
        public bool UseManifoldReduction;
        public bool UseLargeIslandSplitter;
        public bool AllowSleeping;
        public bool CheckActiveEdges;

        internal unsafe JPH_PhysicsSettings ToNative()
        {
            return new JPH_PhysicsSettings
            {
                maxInFlightBodyPairs = MaxInFlightBodyPairs,
                stepListenersBatchSize = StepListenersBatchSize,
                stepListenerBatchesPerJob = StepListenerBatchesPerJob,
                baumgarte = Baumgarte,
                speculativeContactDistance = SpeculativeContactDistance,
                penetrationSlop = PenetrationSlop,
                linearCastThreshold = LinearCastThreshold,
                linearCastMaxPenetration = LinearCastMaxPenetration,
                manifoldTolerance = ManifoldTolerance,
                maxPenetrationDistance = MaxPenetrationDistance,
                bodyPairCacheMaxDeltaPositionSq = BodyPairCacheMaxDeltaPositionSq,
                bodyPairCacheCosMaxDeltaRotationDiv2 = BodyPairCacheCosMaxDeltaRotationDiv2,
                contactNormalCosMaxDeltaRotation = ContactNormalCosMaxDeltaRotation,
                contactPointPreserveLambdaMaxDistSq = ContactPointPreserveLambdaMaxDistSq,
                numVelocitySteps = NumVelocitySteps,
                numPositionSteps = NumPositionSteps,
                minVelocityForRestitution = MinVelocityForRestitution,
                timeBeforeSleep = TimeBeforeSleep,
                pointVelocitySleepThreshold = PointVelocitySleepThreshold,
                deterministicSimulation = (byte)(DeterministicSimulation ? 1 : 0),
                constraintWarmStart = (byte)(ConstraintWarmStart ? 1 : 0),
                useBodyPairContactCache = (byte)(UseBodyPairContactCache ? 1 : 0),
                useManifoldReduction = (byte)(UseManifoldReduction ? 1 : 0),
                useLargeIslandSplitter = (byte)(UseLargeIslandSplitter ? 1 : 0),
                allowSleeping = (byte)(AllowSleeping ? 1 : 0),
                checkActiveEdges = (byte)(CheckActiveEdges ? 1 : 0),
            };
        }

        internal static JoltPhysicsSettings FromNative(JPH_PhysicsSettings native)
        {
            return new JoltPhysicsSettings
            {
                MaxInFlightBodyPairs = native.maxInFlightBodyPairs,
                StepListenersBatchSize = native.stepListenersBatchSize,
                StepListenerBatchesPerJob = native.stepListenerBatchesPerJob,
                Baumgarte = native.baumgarte,
                SpeculativeContactDistance = native.speculativeContactDistance,
                PenetrationSlop = native.penetrationSlop,
                LinearCastThreshold = native.linearCastThreshold,
                LinearCastMaxPenetration = native.linearCastMaxPenetration,
                ManifoldTolerance = native.manifoldTolerance,
                MaxPenetrationDistance = native.maxPenetrationDistance,
                BodyPairCacheMaxDeltaPositionSq = native.bodyPairCacheMaxDeltaPositionSq,
                BodyPairCacheCosMaxDeltaRotationDiv2 = native.bodyPairCacheCosMaxDeltaRotationDiv2,
                ContactNormalCosMaxDeltaRotation = native.contactNormalCosMaxDeltaRotation,
                ContactPointPreserveLambdaMaxDistSq = native.contactPointPreserveLambdaMaxDistSq,
                NumVelocitySteps = native.numVelocitySteps,
                NumPositionSteps = native.numPositionSteps,
                MinVelocityForRestitution = native.minVelocityForRestitution,
                TimeBeforeSleep = native.timeBeforeSleep,
                PointVelocitySleepThreshold = native.pointVelocitySleepThreshold,
                DeterministicSimulation = native.deterministicSimulation != 0,
                ConstraintWarmStart = native.constraintWarmStart != 0,
                UseBodyPairContactCache = native.useBodyPairContactCache != 0,
                UseManifoldReduction = native.useManifoldReduction != 0,
                UseLargeIslandSplitter = native.useLargeIslandSplitter != 0,
                AllowSleeping = native.allowSleeping != 0,
                CheckActiveEdges = native.checkActiveEdges != 0,
            };
        }
    }

    public struct PhysicsSystemSettings
    {
        public uint MaxBodies;
        public uint NumBodyMutexes;
        public uint MaxBodyPairs;
        public uint MaxContactConstraints;

        public static PhysicsSystemSettings Default => new()
        {
            MaxBodies = 10240, NumBodyMutexes = 0, MaxBodyPairs = 65536, MaxContactConstraints = 10240,
        };
    }

    public sealed class PhysicsSystem: NativeHandle
    {
        // Keep GC references to filters to prevent premature collection
        private readonly BroadPhaseLayerInterfaceTable _broadPhaseLayerInterface;
        private readonly ObjectLayerPairFilterTable _objectLayerPairFilter;
        private readonly ObjectVsBroadPhaseLayerFilterTable _objectVsBroadPhaseLayerFilter;

        public PhysicsSystem(
        PhysicsSystemSettings settings,
        BroadPhaseLayerInterfaceTable broadPhaseLayerInterface,
        ObjectLayerPairFilterTable objectLayerPairFilter,
        ObjectVsBroadPhaseLayerFilterTable objectVsBroadPhaseLayerFilter)
                : base(IntPtr.Zero, true)
        {
            _broadPhaseLayerInterface = broadPhaseLayerInterface;
            _objectLayerPairFilter = objectLayerPairFilter;
            _objectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter;

            unsafe
            {
                var nativeSettings = new JPH_PhysicsSystemSettings
                {
                    maxBodies = settings.MaxBodies,
                    numBodyMutexes = settings.NumBodyMutexes,
                    maxBodyPairs = settings.MaxBodyPairs,
                    maxContactConstraints = settings.MaxContactConstraints,
                    broadPhaseLayerInterface = (JPH_BroadPhaseLayerInterface*)broadPhaseLayerInterface.Handle,
                    objectLayerPairFilter = (JPH_ObjectLayerPairFilter*)objectLayerPairFilter.Handle,
                    objectVsBroadPhaseLayerFilter =
                            (JPH_ObjectVsBroadPhaseLayerFilter*)objectVsBroadPhaseLayerFilter.Handle,
                };
                Handle = (IntPtr)Methods.JPH_PhysicsSystem_Create(&nativeSettings);
            }
        }

        public BodyInterface GetBodyInterface()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = (IntPtr)Methods.JPH_PhysicsSystem_GetBodyInterface((JPH_PhysicsSystem*)Handle);
                return new BodyInterface(ptr);
            }
        }

        public Float3 Gravity
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_Vec3 result;
                    Methods.JPH_PhysicsSystem_GetGravity((JPH_PhysicsSystem*)Handle, &result);
                    return new Float3(result.x, result.y, result.z);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    var native = new JPH_Vec3 { x = value.x, y = value.y, z = value.z };
                    Methods.JPH_PhysicsSystem_SetGravity((JPH_PhysicsSystem*)Handle, &native);
                }
            }
        }

        public void Update(float deltaTime, int collisionSteps, TempAllocator tempAllocator, JobSystemThreadPool jobSystem)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_PhysicsSystem_Update((JPH_PhysicsSystem*)Handle,
                    deltaTime,
                    collisionSteps,
                    (JPH_TempAllocator*)tempAllocator.Handle,
                    (JPH_JobSystem*)jobSystem.Handle);
            }
        }

        public void SetContactListener(ContactListener listener)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_PhysicsSystem_SetContactListener((JPH_PhysicsSystem*)Handle,
                    (JPH_ContactListener*)listener.Handle);
            }
        }

        public BodyLockInterface GetBodyLockInterface()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = (IntPtr)Methods.JPH_PhysicsSystem_GetBodyLockInterface((JPH_PhysicsSystem*)Handle);
                return new BodyLockInterface(ptr);
            }
        }

        public BodyLockInterface GetBodyLockInterfaceNoLock()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = (IntPtr)Methods.JPH_PhysicsSystem_GetBodyLockInterfaceNoLock((JPH_PhysicsSystem*)Handle);
                return new BodyLockInterface(ptr);
            }
        }

        public BroadPhaseQuery GetBroadPhaseQuery()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = (IntPtr)Methods.JPH_PhysicsSystem_GetBroadPhaseQuery((JPH_PhysicsSystem*)Handle);
                return new BroadPhaseQuery(ptr);
            }
        }

        public NarrowPhaseQuery GetNarrowPhaseQuery()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = (IntPtr)Methods.JPH_PhysicsSystem_GetNarrowPhaseQuery((JPH_PhysicsSystem*)Handle);
                return new NarrowPhaseQuery(ptr);
            }
        }

        public void AddConstraint(Constraint constraint)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_PhysicsSystem_AddConstraint((JPH_PhysicsSystem*)Handle, (JPH_Constraint*)constraint.Handle);
            }
        }

        public void RemoveConstraint(Constraint constraint)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_PhysicsSystem_RemoveConstraint((JPH_PhysicsSystem*)Handle, (JPH_Constraint*)constraint.Handle);
            }
        }

        public void OptimizeBroadPhase()
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_PhysicsSystem_OptimizeBroadPhase((JPH_PhysicsSystem*)Handle);
            }
        }

        public BodyInterface GetBodyInterfaceNoLock()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = (IntPtr)Methods.JPH_PhysicsSystem_GetBodyInterfaceNoLock((JPH_PhysicsSystem*)Handle);
                return new BodyInterface(ptr);
            }
        }

        public NarrowPhaseQuery GetNarrowPhaseQueryNoLock()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = (IntPtr)Methods.JPH_PhysicsSystem_GetNarrowPhaseQueryNoLock((JPH_PhysicsSystem*)Handle);
                return new NarrowPhaseQuery(ptr);
            }
        }

        public uint NumBodies
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_PhysicsSystem_GetNumBodies((JPH_PhysicsSystem*)Handle); }
            }
        }

        public uint MaxBodies
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_PhysicsSystem_GetMaxBodies((JPH_PhysicsSystem*)Handle); }
            }
        }

        public uint NumConstraints
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_PhysicsSystem_GetNumConstraints((JPH_PhysicsSystem*)Handle); }
            }
        }

        public uint GetNumActiveBodies(BodyType bodyType)
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_PhysicsSystem_GetNumActiveBodies((JPH_PhysicsSystem*)Handle, (JPH_BodyType)bodyType); }
        }

        public bool WereBodiesInContact(BodyID body1, BodyID body2)
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_PhysicsSystem_WereBodiesInContact((JPH_PhysicsSystem*)Handle, body1.Value, body2.Value) != 0; }
        }

        public void GetActiveBodies(BodyType type, BodyID[] ids)
        {
            ThrowIfDisposed();
            unsafe
            {
                fixed (BodyID* pIds = ids)
                {
                    Methods.JPH_PhysicsSystem_GetActiveBodies(
                        (JPH_PhysicsSystem*)Handle, (JPH_BodyType)type,
                        (uint*)pIds, (uint)ids.Length);
                }
            }
        }

        public Body GetBodyPtr(BodyID bodyID)
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = Methods.JPH_PhysicsSystem_GetBodyPtr((JPH_PhysicsSystem*)Handle, bodyID.Value);
                return new Body(ptr);
            }
        }

        public JoltPhysicsSettings GetPhysicsSettings()
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_PhysicsSettings native;
                Methods.JPH_PhysicsSystem_GetPhysicsSettings((JPH_PhysicsSystem*)Handle, &native);
                return JoltPhysicsSettings.FromNative(native);
            }
        }

        public void SetPhysicsSettings(JoltPhysicsSettings settings)
        {
            ThrowIfDisposed();
            unsafe
            {
                var native = settings.ToNative();
                Methods.JPH_PhysicsSystem_SetPhysicsSettings((JPH_PhysicsSystem*)Handle, &native);
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_PhysicsSystem_Destroy((JPH_PhysicsSystem*)Handle);
            }
        }
    }
}