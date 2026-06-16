// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    /// <summary>
    /// 非拥有的 Body 包装。仅在持有 BodyLock 期间有效。
    /// 通过 BodyLockRead/BodyLockWrite 获取。
    /// </summary>
    public readonly unsafe struct Body
    {
        internal readonly JPH_Body* Ptr;

        internal Body(JPH_Body* ptr)
        {
            Ptr = ptr;
        }

        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr != null;
        }

        #region Identity

        public BodyID ID => new BodyID(Methods.JPH_Body_GetID(Ptr));

        public BodyType BodyType => (BodyType)Methods.JPH_Body_GetBodyType(Ptr);

        public bool IsRigidBody => Methods.JPH_Body_IsRigidBody(Ptr) != 0;

        public bool IsSoftBody => Methods.JPH_Body_IsSoftBody(Ptr) != 0;

        #endregion

        #region State

        public bool IsActive => Methods.JPH_Body_IsActive(Ptr) != 0;

        public bool IsStatic => Methods.JPH_Body_IsStatic(Ptr) != 0;

        public bool IsKinematic => Methods.JPH_Body_IsKinematic(Ptr) != 0;

        public bool IsDynamic => Methods.JPH_Body_IsDynamic(Ptr) != 0;

        public bool CanBeKinematicOrDynamic => Methods.JPH_Body_CanBeKinematicOrDynamic(Ptr) != 0;

        public bool IsInBroadPhase => Methods.JPH_Body_IsInBroadPhase(Ptr) != 0;

        #endregion

        #region Properties

        public MotionType MotionType
        {
            get => (MotionType)Methods.JPH_Body_GetMotionType(Ptr);
            set => Methods.JPH_Body_SetMotionType(Ptr, (JPH_MotionType)value);
        }

        public bool IsSensor
        {
            get => Methods.JPH_Body_IsSensor(Ptr) != 0;
            set => Methods.JPH_Body_SetIsSensor(Ptr, (byte)(value ? 1 : 0));
        }

        public bool AllowSleeping
        {
            get => Methods.JPH_Body_GetAllowSleeping(Ptr) != 0;
            set => Methods.JPH_Body_SetAllowSleeping(Ptr, (byte)(value ? 1 : 0));
        }

        public float Friction
        {
            get => Methods.JPH_Body_GetFriction(Ptr);
            set => Methods.JPH_Body_SetFriction(Ptr, value);
        }

        public float Restitution
        {
            get => Methods.JPH_Body_GetRestitution(Ptr);
            set => Methods.JPH_Body_SetRestitution(Ptr, value);
        }

        public BroadPhaseLayer BroadPhaseLayer => new BroadPhaseLayer(Methods.JPH_Body_GetBroadPhaseLayer(Ptr));

        public ObjectLayer ObjectLayer => new ObjectLayer(Methods.JPH_Body_GetObjectLayer(Ptr));

        public ulong UserData
        {
            get => Methods.JPH_Body_GetUserData(Ptr);
            set => Methods.JPH_Body_SetUserData(Ptr, value);
        }

        public bool CollideKinematicVsNonDynamic
        {
            get => Methods.JPH_Body_GetCollideKinematicVsNonDynamic(Ptr) != 0;
            set => Methods.JPH_Body_SetCollideKinematicVsNonDynamic(Ptr, (byte)(value ? 1 : 0));
        }

        public bool UseManifoldReduction
        {
            get => Methods.JPH_Body_GetUseManifoldReduction(Ptr) != 0;
            set => Methods.JPH_Body_SetUseManifoldReduction(Ptr, (byte)(value ? 1 : 0));
        }

        public bool ApplyGyroscopicForce
        {
            get => Methods.JPH_Body_GetApplyGyroscopicForce(Ptr) != 0;
            set => Methods.JPH_Body_SetApplyGyroscopicForce(Ptr, (byte)(value ? 1 : 0));
        }

        public bool EnhancedInternalEdgeRemoval
        {
            get => Methods.JPH_Body_GetEnhancedInternalEdgeRemoval(Ptr) != 0;
            set => Methods.JPH_Body_SetEnhancedInternalEdgeRemoval(Ptr, (byte)(value ? 1 : 0));
        }

        #endregion

        #region Transform

        public Float3 Position
        {
            get
            {
                Float3 result;
                Methods.JPH_Body_GetPosition(Ptr, (JPH_Vec3*)&result);
                return result;
            }
        }

        public Quat Rotation
        {
            get
            {
                Quat result;
                Methods.JPH_Body_GetRotation(Ptr, (JPH_Quat*)&result);
                return result;
            }
        }

        public Float3 CenterOfMassPosition
        {
            get
            {
                Float3 result;
                Methods.JPH_Body_GetCenterOfMassPosition(Ptr, (JPH_Vec3*)&result);
                return result;
            }
        }

        public BoundingBox WorldSpaceBounds
        {
            get
            {
                JPH_AABox box;
                Methods.JPH_Body_GetWorldSpaceBounds(Ptr, &box);
                return new BoundingBox(
                    new Float3(box.min.x, box.min.y, box.min.z),
                    new Float3(box.max.x, box.max.y, box.max.z));
            }
        }

        #endregion

        #region Transform (Matrix)

        public Mat4 WorldTransform
        {
            get
            {
                Mat4 result;
                Methods.JPH_Body_GetWorldTransform(Ptr, (JPH_Mat4*)&result);
                return result;
            }
        }

        public Mat4 CenterOfMassTransform
        {
            get
            {
                Mat4 result;
                Methods.JPH_Body_GetCenterOfMassTransform(Ptr, (JPH_Mat4*)&result);
                return result;
            }
        }

        public Mat4 InverseCenterOfMassTransform
        {
            get
            {
                Mat4 result;
                Methods.JPH_Body_GetInverseCenterOfMassTransform(Ptr, (JPH_Mat4*)&result);
                return result;
            }
        }

        public Mat4 InverseInertia
        {
            get
            {
                Mat4 result;
                Methods.JPH_Body_GetInverseInertia(Ptr, (JPH_Mat4*)&result);
                return result;
            }
        }

        #endregion

        #region Velocity

        public Float3 LinearVelocity
        {
            get
            {
                Float3 result;
                Methods.JPH_Body_GetLinearVelocity(Ptr, (JPH_Vec3*)&result);
                return result;
            }
            set => Methods.JPH_Body_SetLinearVelocity(Ptr, (JPH_Vec3*)&value);
        }

        public Float3 AngularVelocity
        {
            get
            {
                Float3 result;
                Methods.JPH_Body_GetAngularVelocity(Ptr, (JPH_Vec3*)&result);
                return result;
            }
            set => Methods.JPH_Body_SetAngularVelocity(Ptr, (JPH_Vec3*)&value);
        }

        public Float3 GetPointVelocity(Float3 point)
        {
            Float3 result;
            Methods.JPH_Body_GetPointVelocity(Ptr, (JPH_Vec3*)&point, (JPH_Vec3*)&result);
            return result;
        }

        public Float3 GetPointVelocityCOM(Float3 pointRelativeToCOM)
        {
            Float3 result;
            Methods.JPH_Body_GetPointVelocityCOM(Ptr, (JPH_Vec3*)&pointRelativeToCOM, (JPH_Vec3*)&result);
            return result;
        }

        public void SetLinearVelocityClamped(Float3 velocity)
        {
            Methods.JPH_Body_SetLinearVelocityClamped(Ptr, (JPH_Vec3*)&velocity);
        }

        public void SetAngularVelocityClamped(Float3 velocity)
        {
            Methods.JPH_Body_SetAngularVelocityClamped(Ptr, (JPH_Vec3*)&velocity);
        }

        #endregion

        #region Force / Torque / Impulse

        public void AddForce(Float3 force)
        {
            Methods.JPH_Body_AddForce(Ptr, (JPH_Vec3*)&force);
        }

        public void AddForce(Float3 force, Float3 position)
        {
            Methods.JPH_Body_AddForceAtPosition(Ptr, (JPH_Vec3*)&force, (JPH_Vec3*)&position);
        }

        public void AddTorque(Float3 torque)
        {
            Methods.JPH_Body_AddTorque(Ptr, (JPH_Vec3*)&torque);
        }

        public Float3 AccumulatedForce
        {
            get
            {
                Float3 result;
                Methods.JPH_Body_GetAccumulatedForce(Ptr, (JPH_Vec3*)&result);
                return result;
            }
        }

        public Float3 AccumulatedTorque
        {
            get
            {
                Float3 result;
                Methods.JPH_Body_GetAccumulatedTorque(Ptr, (JPH_Vec3*)&result);
                return result;
            }
        }

        public void ResetForce() => Methods.JPH_Body_ResetForce(Ptr);
        public void ResetTorque() => Methods.JPH_Body_ResetTorque(Ptr);
        public void ResetMotion() => Methods.JPH_Body_ResetMotion(Ptr);

        public void AddImpulse(Float3 impulse)
        {
            Methods.JPH_Body_AddImpulse(Ptr, (JPH_Vec3*)&impulse);
        }

        public void AddImpulse(Float3 impulse, Float3 position)
        {
            Methods.JPH_Body_AddImpulseAtPosition(Ptr, (JPH_Vec3*)&impulse, (JPH_Vec3*)&position);
        }

        public void AddAngularImpulse(Float3 angularImpulse)
        {
            Methods.JPH_Body_AddAngularImpulse(Ptr, (JPH_Vec3*)&angularImpulse);
        }

        public void MoveKinematic(Float3 targetPosition, Quat targetRotation, float deltaTime)
        {
            Methods.JPH_Body_MoveKinematic(Ptr, (JPH_Vec3*)&targetPosition, (JPH_Quat*)&targetRotation, deltaTime);
        }

        #endregion

        #region Shape / MotionProperties

        public MotionProperties GetMotionProperties()
        {
            return new MotionProperties(Methods.JPH_Body_GetMotionProperties(Ptr));
        }

        public MotionProperties GetMotionPropertiesUnchecked()
        {
            return new MotionProperties(Methods.JPH_Body_GetMotionPropertiesUnchecked(Ptr));
        }

        public Shape GetShape()
        {
            var ptr = Methods.JPH_Body_GetShape(Ptr);
            if (ptr == null) return null;
            return new GenericShape((IntPtr)ptr, false);
        }

        public void ResetSleepTimer() => Methods.JPH_Body_ResetSleepTimer(Ptr);

        public bool IsCollisionCacheInvalid => Methods.JPH_Body_IsCollisionCacheInvalid(Ptr) != 0;

        public bool GetUseManifoldReductionWithBody(Body other)
        {
            return Methods.JPH_Body_GetUseManifoldReductionWithBody(Ptr, other.Ptr) != 0;
        }

        public bool GetEnhancedInternalEdgeRemovalWithBody(Body other)
        {
            return Methods.JPH_Body_GetEnhancedInternalEdgeRemovalWithBody(Ptr, other.Ptr) != 0;
        }

        public Float3 GetWorldSpaceSurfaceNormal(uint subShapeID, Float3 position)
        {
            Float3 result;
            Methods.JPH_Body_GetWorldSpaceSurfaceNormal(Ptr, subShapeID, (JPH_Vec3*)&position, (JPH_Vec3*)&result);
            return result;
        }

        public bool ApplyBuoyancyImpulse(Float3 surfacePosition, Float3 surfaceNormal,
            float buoyancy, float linearDrag, float angularDrag,
            Float3 fluidVelocity, Float3 gravity, float deltaTime)
        {
            return Methods.JPH_Body_ApplyBuoyancyImpulse(Ptr,
                (JPH_Vec3*)&surfacePosition, (JPH_Vec3*)&surfaceNormal,
                buoyancy, linearDrag, angularDrag,
                (JPH_Vec3*)&fluidVelocity, (JPH_Vec3*)&gravity, deltaTime) != 0;
        }

        #endregion

        #region Collision Group

        public CollisionGroup GetCollisionGroup()
        {
            JPH_CollisionGroup native;
            Methods.JPH_Body_GetCollisionGroup(Ptr, &native);
            return new CollisionGroup(native.groupID, native.subGroupID);
        }

        public void SetCollisionGroup(CollisionGroup group)
        {
            var native = new JPH_CollisionGroup
            {
                groupFilter = null,
                groupID = group.groupID,
                subGroupID = group.subGroupID,
            };
            Methods.JPH_Body_SetCollisionGroup(Ptr, &native);
        }

        #endregion

        #region Static helpers

        public static Body GetFixedToWorldBody()
        {
            return new Body(Methods.JPH_Body_GetFixedToWorldBody());
        }

        #endregion

        #region Soft Body

        public uint SoftBodyVertexCount => Methods.JPH_Body_GetSoftBodyVertexCount(Ptr);

        public Float3 GetSoftBodyVertexPosition(uint index)
        {
            Float3 result;
            Methods.JPH_Body_GetSoftBodyVertexPosition(Ptr, index, (JPH_Vec3*)&result);
            return result;
        }

        public uint GetSoftBodyVertexPositions(Float3[] outPositions)
        {
            uint count;
            fixed (Float3* pOut = outPositions)
            {
                Methods.JPH_Body_GetSoftBodyVertexPositions(Ptr, (JPH_Vec3*)pOut, (uint)outPositions.Length, &count);
            }
            return count;
        }

        #endregion
    }
}
