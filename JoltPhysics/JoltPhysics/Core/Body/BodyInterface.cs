// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public readonly struct BodyInterface
    {
        internal readonly IntPtr Handle;

        internal BodyInterface(IntPtr handle)
        {
            Handle = handle;
        }

        private unsafe JPH_BodyInterface* Ptr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (JPH_BodyInterface*)Handle;
        }

        #region Create / Destroy

        public BodyID CreateAndAddBody(BodyCreationSettings settings, Activation activation)
        {
            unsafe
            {
                uint id = Methods.JPH_BodyInterface_CreateAndAddBody(
                    Ptr, (JPH_BodyCreationSettings*)settings.Handle, (JPH_Activation)activation);
                return new BodyID(id);
            }
        }

        /// <summary>
        /// 仅创建 body（分配 BodyID），不加入物理世界。需后续调用 AddBody 才参与模拟。
        /// </summary>
        public BodyID CreateBody(BodyCreationSettings settings)
        {
            unsafe
            {
                var body = Methods.JPH_BodyInterface_CreateBody(
                    Ptr, (JPH_BodyCreationSettings*)settings.Handle);
                if (body == null)
                    return BodyID.Invalid;
                return new BodyID(Methods.JPH_Body_GetID(body));
            }
        }

        public BodyID CreateBodyWithID(uint bodyID, BodyCreationSettings settings)
        {
            unsafe
            {
                var body = Methods.JPH_BodyInterface_CreateBodyWithID(
                    Ptr, bodyID, (JPH_BodyCreationSettings*)settings.Handle);
                return new BodyID(Methods.JPH_Body_GetID(body));
            }
        }

        /// <summary>
        /// 销毁一个不在物理世界中的 body（已 RemoveBody 或从未 AddBody）。
        /// </summary>
        public void DestroyBody(BodyID bodyID)
        {
            unsafe { Methods.JPH_BodyInterface_DestroyBody(Ptr, bodyID.Value); }
        }

        public void AddBody(BodyID bodyID, Activation activation)
        {
            unsafe { Methods.JPH_BodyInterface_AddBody(Ptr, bodyID.Value, (JPH_Activation)activation); }
        }

        public void RemoveBody(BodyID bodyID)
        {
            unsafe { Methods.JPH_BodyInterface_RemoveBody(Ptr, bodyID.Value); }
        }

        public void RemoveAndDestroyBody(BodyID bodyID)
        {
            unsafe { Methods.JPH_BodyInterface_RemoveAndDestroyBody(Ptr, bodyID.Value); }
        }

        public bool IsAdded(BodyID bodyID)
        {
            unsafe { return Methods.JPH_BodyInterface_IsAdded(Ptr, bodyID.Value) != 0; }
        }

        public BodyID CreateAndAddSoftBody(SoftBodyCreationSettings settings, Activation activation)
        {
            unsafe
            {
                uint id = Methods.JPH_BodyInterface_CreateAndAddSoftBody(
                    Ptr, (JPH_SoftBodyCreationSettings*)settings.Handle, (JPH_Activation)activation);
                return new BodyID(id);
            }
        }

        public BodyID CreateSoftBody(SoftBodyCreationSettings settings)
        {
            unsafe
            {
                var body = Methods.JPH_BodyInterface_CreateSoftBody(
                    Ptr, (JPH_SoftBodyCreationSettings*)settings.Handle);
                if (body == null)
                    return BodyID.Invalid;
                return new BodyID(Methods.JPH_Body_GetID(body));
            }
        }

        public BodyID CreateSoftBodyWithID(uint bodyID, SoftBodyCreationSettings settings)
        {
            unsafe
            {
                var body = Methods.JPH_BodyInterface_CreateSoftBodyWithID(
                    Ptr, bodyID, (JPH_SoftBodyCreationSettings*)settings.Handle);
                if (body == null)
                    return BodyID.Invalid;
                return new BodyID(Methods.JPH_Body_GetID(body));
            }
        }

        #endregion

        #region Activation

        public void ActivateBody(BodyID bodyID)
        {
            unsafe { Methods.JPH_BodyInterface_ActivateBody(Ptr, bodyID.Value); }
        }

        public void DeactivateBody(BodyID bodyID)
        {
            unsafe { Methods.JPH_BodyInterface_DeactivateBody(Ptr, bodyID.Value); }
        }

        public bool IsActive(BodyID bodyID)
        {
            unsafe { return Methods.JPH_BodyInterface_IsActive(Ptr, bodyID.Value) != 0; }
        }

        public void ResetSleepTimer(BodyID bodyID)
        {
            unsafe { Methods.JPH_BodyInterface_ResetSleepTimer(Ptr, bodyID.Value); }
        }

        #endregion

        #region Position / Rotation

        public Float3 GetPosition(BodyID bodyID)
        {
            unsafe
            {
                Float3 result;
                Methods.JPH_BodyInterface_GetPosition(Ptr, bodyID.Value, (JPH_Vec3*)&result);
                return result;
            }
        }

        public void SetPosition(BodyID bodyID, Float3 position, Activation activation)
        {
            unsafe { Methods.JPH_BodyInterface_SetPosition(Ptr, bodyID.Value, (JPH_Vec3*)&position, (JPH_Activation)activation); }
        }

        public Quat GetRotation(BodyID bodyID)
        {
            unsafe
            {
                Quat result;
                Methods.JPH_BodyInterface_GetRotation(Ptr, bodyID.Value, (JPH_Quat*)&result);
                return result;
            }
        }

        public void SetRotation(BodyID bodyID, Quat rotation, Activation activation)
        {
            unsafe { Methods.JPH_BodyInterface_SetRotation(Ptr, bodyID.Value, (JPH_Quat*)&rotation, (JPH_Activation)activation); }
        }

        public void GetPositionAndRotation(BodyID bodyID, out Float3 position, out Quat rotation)
        {
            unsafe
            {
                fixed (Float3* pPos = &position)
                fixed (Quat* pRot = &rotation)
                {
                    Methods.JPH_BodyInterface_GetPositionAndRotation(
                        Ptr, bodyID.Value, (JPH_Vec3*)pPos, (JPH_Quat*)pRot);
                }
            }
        }

        public void SetPositionAndRotation(BodyID bodyID, Float3 position, Quat rotation, Activation activation)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetPositionAndRotation(
                    Ptr, bodyID.Value, (JPH_Vec3*)&position, (JPH_Quat*)&rotation, (JPH_Activation)activation);
            }
        }

        /// <summary>
        /// 仅在 position/rotation 实际变化时才设置（Jolt 内部比较）。
        /// </summary>
        public void SetPositionAndRotationWhenChanged(BodyID bodyID, Float3 position, Quat rotation, Activation activation)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetPositionAndRotationWhenChanged(
                    Ptr, bodyID.Value, (JPH_Vec3*)&position, (JPH_Quat*)&rotation, (JPH_Activation)activation);
            }
        }

        public void SetPositionRotationAndVelocity(BodyID bodyID, Float3 position, Quat rotation, Float3 linearVelocity, Float3 angularVelocity)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetPositionRotationAndVelocity(
                    Ptr, bodyID.Value, (JPH_Vec3*)&position, (JPH_Quat*)&rotation,
                    (JPH_Vec3*)&linearVelocity, (JPH_Vec3*)&angularVelocity);
            }
        }

        public Float3 GetCenterOfMassPosition(BodyID bodyID)
        {
            unsafe
            {
                Float3 result;
                Methods.JPH_BodyInterface_GetCenterOfMassPosition(Ptr, bodyID.Value, (JPH_Vec3*)&result);
                return result;
            }
        }

        public void MoveKinematic(BodyID bodyID, Float3 targetPosition, Quat targetRotation, float deltaTime)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_MoveKinematic(
                    Ptr, bodyID.Value, (JPH_Vec3*)&targetPosition, (JPH_Quat*)&targetRotation, deltaTime);
            }
        }

        #endregion

        #region Velocity

        public Float3 GetLinearVelocity(BodyID bodyID)
        {
            unsafe
            {
                Float3 result;
                Methods.JPH_BodyInterface_GetLinearVelocity(Ptr, bodyID.Value, (JPH_Vec3*)&result);
                return result;
            }
        }

        public void SetLinearVelocity(BodyID bodyID, Float3 velocity)
        {
            unsafe { Methods.JPH_BodyInterface_SetLinearVelocity(Ptr, bodyID.Value, (JPH_Vec3*)&velocity); }
        }

        public Float3 GetAngularVelocity(BodyID bodyID)
        {
            unsafe
            {
                Float3 result;
                Methods.JPH_BodyInterface_GetAngularVelocity(Ptr, bodyID.Value, (JPH_Vec3*)&result);
                return result;
            }
        }

        public void SetAngularVelocity(BodyID bodyID, Float3 velocity)
        {
            unsafe { Methods.JPH_BodyInterface_SetAngularVelocity(Ptr, bodyID.Value, (JPH_Vec3*)&velocity); }
        }

        public void GetLinearAndAngularVelocity(BodyID bodyID, out Float3 linearVelocity, out Float3 angularVelocity)
        {
            unsafe
            {
                fixed (Float3* pLin = &linearVelocity)
                fixed (Float3* pAng = &angularVelocity)
                {
                    Methods.JPH_BodyInterface_GetLinearAndAngularVelocity(
                        Ptr, bodyID.Value, (JPH_Vec3*)pLin, (JPH_Vec3*)pAng);
                }
            }
        }

        public void SetLinearAndAngularVelocity(BodyID bodyID, Float3 linearVelocity, Float3 angularVelocity)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetLinearAndAngularVelocity(
                    Ptr, bodyID.Value, (JPH_Vec3*)&linearVelocity, (JPH_Vec3*)&angularVelocity);
            }
        }

        public void AddLinearVelocity(BodyID bodyID, Float3 velocity)
        {
            unsafe { Methods.JPH_BodyInterface_AddLinearVelocity(Ptr, bodyID.Value, (JPH_Vec3*)&velocity); }
        }

        public void AddLinearAndAngularVelocity(BodyID bodyID, Float3 linearVelocity, Float3 angularVelocity)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_AddLinearAndAngularVelocity(
                    Ptr, bodyID.Value, (JPH_Vec3*)&linearVelocity, (JPH_Vec3*)&angularVelocity);
            }
        }

        public Float3 GetPointVelocity(BodyID bodyID, Float3 point)
        {
            unsafe
            {
                Float3 result;
                Methods.JPH_BodyInterface_GetPointVelocity(Ptr, bodyID.Value, (JPH_Vec3*)&point, (JPH_Vec3*)&result);
                return result;
            }
        }

        #endregion

        #region Force / Torque / Impulse

        public void AddForce(BodyID bodyID, Float3 force)
        {
            unsafe { Methods.JPH_BodyInterface_AddForce(Ptr, bodyID.Value, (JPH_Vec3*)&force); }
        }

        public void AddForce(BodyID bodyID, Float3 force, Float3 point)
        {
            unsafe { Methods.JPH_BodyInterface_AddForce2(Ptr, bodyID.Value, (JPH_Vec3*)&force, (JPH_Vec3*)&point); }
        }

        public void AddTorque(BodyID bodyID, Float3 torque)
        {
            unsafe { Methods.JPH_BodyInterface_AddTorque(Ptr, bodyID.Value, (JPH_Vec3*)&torque); }
        }

        public void AddForceAndTorque(BodyID bodyID, Float3 force, Float3 torque)
        {
            unsafe { Methods.JPH_BodyInterface_AddForceAndTorque(Ptr, bodyID.Value, (JPH_Vec3*)&force, (JPH_Vec3*)&torque); }
        }

        public void AddImpulse(BodyID bodyID, Float3 impulse)
        {
            unsafe { Methods.JPH_BodyInterface_AddImpulse(Ptr, bodyID.Value, (JPH_Vec3*)&impulse); }
        }

        public void AddImpulse(BodyID bodyID, Float3 impulse, Float3 point)
        {
            unsafe { Methods.JPH_BodyInterface_AddImpulse2(Ptr, bodyID.Value, (JPH_Vec3*)&impulse, (JPH_Vec3*)&point); }
        }

        public void AddAngularImpulse(BodyID bodyID, Float3 impulse)
        {
            unsafe { Methods.JPH_BodyInterface_AddAngularImpulse(Ptr, bodyID.Value, (JPH_Vec3*)&impulse); }
        }

        #endregion

        #region Properties

        public MotionType GetMotionType(BodyID bodyID)
        {
            unsafe { return (MotionType)Methods.JPH_BodyInterface_GetMotionType(Ptr, bodyID.Value); }
        }

        public void SetMotionType(BodyID bodyID, MotionType motionType, Activation activation)
        {
            unsafe { Methods.JPH_BodyInterface_SetMotionType(Ptr, bodyID.Value, (JPH_MotionType)motionType, (JPH_Activation)activation); }
        }

        public float GetRestitution(BodyID bodyID)
        {
            unsafe { return Methods.JPH_BodyInterface_GetRestitution(Ptr, bodyID.Value); }
        }

        public void SetRestitution(BodyID bodyID, float restitution)
        {
            unsafe { Methods.JPH_BodyInterface_SetRestitution(Ptr, bodyID.Value, restitution); }
        }

        public float GetFriction(BodyID bodyID)
        {
            unsafe { return Methods.JPH_BodyInterface_GetFriction(Ptr, bodyID.Value); }
        }

        public void SetFriction(BodyID bodyID, float friction)
        {
            unsafe { Methods.JPH_BodyInterface_SetFriction(Ptr, bodyID.Value, friction); }
        }

        public float GetGravityFactor(BodyID bodyID)
        {
            unsafe { return Methods.JPH_BodyInterface_GetGravityFactor(Ptr, bodyID.Value); }
        }

        public void SetGravityFactor(BodyID bodyID, float gravityFactor)
        {
            unsafe { Methods.JPH_BodyInterface_SetGravityFactor(Ptr, bodyID.Value, gravityFactor); }
        }

        public ObjectLayer GetObjectLayer(BodyID bodyID)
        {
            unsafe { return new ObjectLayer(Methods.JPH_BodyInterface_GetObjectLayer(Ptr, bodyID.Value)); }
        }

        public void SetObjectLayer(BodyID bodyID, ObjectLayer layer)
        {
            unsafe { Methods.JPH_BodyInterface_SetObjectLayer(Ptr, bodyID.Value, layer.Value); }
        }

        public bool IsSensor(BodyID bodyID)
        {
            unsafe { return Methods.JPH_BodyInterface_IsSensor(Ptr, bodyID.Value) != 0; }
        }

        public void SetIsSensor(BodyID bodyID, bool isSensor)
        {
            unsafe { Methods.JPH_BodyInterface_SetIsSensor(Ptr, bodyID.Value, (byte)(isSensor ? 1 : 0)); }
        }

        public void SetMotionQuality(BodyID bodyID, MotionQuality quality)
        {
            unsafe { Methods.JPH_BodyInterface_SetMotionQuality(Ptr, bodyID.Value, (JPH_MotionQuality)quality); }
        }

        public MotionQuality GetMotionQuality(BodyID bodyID)
        {
            unsafe { return (MotionQuality)Methods.JPH_BodyInterface_GetMotionQuality(Ptr, bodyID.Value); }
        }

        public CollisionGroup GetCollisionGroup(BodyID bodyID)
        {
            unsafe
            {
                JPH_CollisionGroup native;
                Methods.JPH_BodyInterface_GetCollisionGroup(Ptr, bodyID.Value, &native);
                return CollisionGroup.FromNative(native);
            }
        }

        public void SetCollisionGroup(BodyID bodyID, CollisionGroup group)
        {
            unsafe
            {
                group.ToNative(out var native);
                Methods.JPH_BodyInterface_SetCollisionGroup(Ptr, bodyID.Value, &native);
            }
        }

        #endregion

        #region Shape

        public void SetShape(BodyID bodyID, Shape shape, bool updateMassProperties, Activation activation)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetShape(
                    Ptr, bodyID.Value, (JPH_Shape*)shape.Handle,
                    (byte)(updateMassProperties ? 1 : 0), (JPH_Activation)activation);
            }
        }

        public void NotifyShapeChanged(BodyID bodyID, Float3 previousCenterOfMass, bool updateMassProperties, Activation activation)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_NotifyShapeChanged(
                    Ptr, bodyID.Value, (JPH_Vec3*)&previousCenterOfMass,
                    (byte)(updateMassProperties ? 1 : 0), (JPH_Activation)activation);
            }
        }

        #endregion

        #region UserData

        public void SetUserData(BodyID bodyID, ulong userData)
        {
            unsafe { Methods.JPH_BodyInterface_SetUserData(Ptr, bodyID.Value, userData); }
        }

        public ulong GetUserData(BodyID bodyID)
        {
            unsafe { return Methods.JPH_BodyInterface_GetUserData(Ptr, bodyID.Value); }
        }

        #endregion

        #region Misc

        public void InvalidateContactCache(BodyID bodyID)
        {
            unsafe { Methods.JPH_BodyInterface_InvalidateContactCache(Ptr, bodyID.Value); }
        }

        public bool ApplyBuoyancyImpulse(BodyID bodyID, Float3 surfacePosition, Float3 surfaceNormal,
            float buoyancy, float linearDrag, float angularDrag, Float3 fluidVelocity, Float3 gravity, float deltaTime)
        {
            unsafe
            {
                return Methods.JPH_BodyInterface_ApplyBuoyancyImpulse(
                    Ptr, bodyID.Value,
                    (JPH_Vec3*)&surfacePosition, (JPH_Vec3*)&surfaceNormal,
                    buoyancy, linearDrag, angularDrag,
                    (JPH_Vec3*)&fluidVelocity, (JPH_Vec3*)&gravity, deltaTime) != 0;
            }
        }

        public Mat4 GetWorldTransform(BodyID bodyID)
        {
            unsafe
            {
                Mat4 result;
                Methods.JPH_BodyInterface_GetWorldTransform(Ptr, bodyID.Value, (JPH_Mat4*)&result);
                return result;
            }
        }

        public Mat4 GetCenterOfMassTransform(BodyID bodyID)
        {
            unsafe
            {
                Mat4 result;
                Methods.JPH_BodyInterface_GetCenterOfMassTransform(Ptr, bodyID.Value, (JPH_Mat4*)&result);
                return result;
            }
        }

        public Mat4 GetInverseInertia(BodyID bodyID)
        {
            unsafe
            {
                Mat4 result;
                Methods.JPH_BodyInterface_GetInverseInertia(Ptr, bodyID.Value, (JPH_Mat4*)&result);
                return result;
            }
        }

        public BodyType GetBodyType(BodyID bodyID)
        {
            unsafe { return (BodyType)Methods.JPH_BodyInterface_GetBodyType(Ptr, bodyID.Value); }
        }

        public bool GetUseManifoldReduction(BodyID bodyID)
        {
            unsafe { return Methods.JPH_BodyInterface_GetUseManifoldReduction(Ptr, bodyID.Value) != 0; }
        }

        public void SetUseManifoldReduction(BodyID bodyID, bool value)
        {
            unsafe { Methods.JPH_BodyInterface_SetUseManifoldReduction(Ptr, bodyID.Value, (byte)(value ? 1 : 0)); }
        }

        #endregion

        #region ShapeData / Raycast (custom helpers)

        /// <summary>
        /// 根据 Body 当前 Shape 的实际类型生成对应的 ShapeData 快照。
        /// </summary>
        public ShapeData GetShapeData(BodyID bodyID)
        {
            unsafe
            {
                var shape = Methods.JPH_BodyInterface_GetShape(Ptr, bodyID.Value);
                if (shape == null)
                    return ShapeData.UnitSphere;

                var subType = Methods.JPH_Shape_GetSubType(shape);
                switch (subType)
                {
                    case JPH_ShapeSubType.JPH_ShapeSubType_Box:
                    {
                        Float3 halfExt;
                        Methods.JPH_BoxShape_GetHalfExtent((JPH_BoxShape*)shape, (JPH_Vec3*)&halfExt);
                        float convexRadius = Methods.JPH_BoxShape_GetConvexRadius((JPH_BoxShape*)shape);
                        return ShapeData.CreateBox(Float3.Zero, halfExt, convexRadius);
                    }
                    case JPH_ShapeSubType.JPH_ShapeSubType_Sphere:
                    {
                        float radius = Methods.JPH_SphereShape_GetRadius((JPH_SphereShape*)shape);
                        return ShapeData.CreateSphere(radius);
                    }
                    case JPH_ShapeSubType.JPH_ShapeSubType_Capsule:
                    {
                        float capsuleRadius = Methods.JPH_CapsuleShape_GetRadius((JPH_CapsuleShape*)shape);
                        float halfHeight = Methods.JPH_CapsuleShape_GetHalfHeightOfCylinder((JPH_CapsuleShape*)shape);
                        return ShapeData.CreateCapsule(halfHeight, capsuleRadius);
                    }
                    case JPH_ShapeSubType.JPH_ShapeSubType_ConvexHull:
                    {
                        var hullShape = (JPH_ConvexHullShape*)shape;

                        uint numPoints = Methods.JPH_ConvexHullShape_GetNumPoints(hullShape);
                        var points = new Float3[numPoints];
                        for (uint i = 0; i < numPoints; i++)
                        {
                            Float3 pt;
                            Methods.JPH_ConvexHullShape_GetPoint(hullShape, i, (JPH_Vec3*)&pt);
                            points[i] = pt;
                        }

                        uint numFaces = Methods.JPH_ConvexHullShape_GetNumFaces(hullShape);
                        // 预估三角形数：每个面平均按 4 顶点算（= 2 个三角形），每三角形 3 索引
                        var triList = new System.Collections.Generic.List<int>((int)numFaces * 6);
                        uint* faceBufPtr = stackalloc uint[64];
                        for (uint f = 0; f < numFaces; f++)
                        {
                            uint numVerts = Methods.JPH_ConvexHullShape_GetNumVerticesInFace(hullShape, f);
                            if (numVerts < 3) continue;
                            if (numVerts > 64) numVerts = 64;
                            Methods.JPH_ConvexHullShape_GetFaceVertices(hullShape, f, numVerts, faceBufPtr);
                            for (uint t = 1; t + 1 < numVerts; t++)
                            {
                                triList.Add((int)faceBufPtr[0]);
                                triList.Add((int)faceBufPtr[t]);
                                triList.Add((int)faceBufPtr[t + 1]);
                            }
                        }

                        return ShapeData.CreateConvexHull(points, triList.ToArray());
                    }
                    default:
                    {
                        JPH_AABox bounds;
                        Methods.JPH_Shape_GetLocalBounds(shape, &bounds);
                        float hx = (bounds.max.x - bounds.min.x) * 0.5f;
                        float hy = (bounds.max.y - bounds.min.y) * 0.5f;
                        float hz = (bounds.max.z - bounds.min.z) * 0.5f;
                        float cx = (bounds.max.x + bounds.min.x) * 0.5f;
                        float cy = (bounds.max.y + bounds.min.y) * 0.5f;
                        float cz = (bounds.max.z + bounds.min.z) * 0.5f;
                        return ShapeData.CreateBox(
                            new Float3(cx, cy, cz),
                            new Float3(hx, hy, hz));
                    }
                }
            }
        }

        /// <summary>
        /// 对指定 Body 做射线检测（世界空间）。
        /// worldDirection 的长度即为射线最大距离。
        /// 返回 true 时 fraction ∈ [0,1]，实际命中距离 = fraction * |worldDirection|。
        /// </summary>
        public bool RaycastBody(BodyID bodyID, Float3 worldOrigin, Float3 worldDirection, out float fraction)
        {
            fraction = float.MaxValue;
            unsafe
            {
                Float3 bodyPos;
                Quat bodyRot;
                Methods.JPH_BodyInterface_GetPositionAndRotation(
                    Ptr, bodyID.Value, (JPH_Vec3*)&bodyPos, (JPH_Quat*)&bodyRot);

                var shape = Methods.JPH_BodyInterface_GetShape(Ptr, bodyID.Value);
                if (shape == null) return false;

                float qx = -bodyRot.x, qy = -bodyRot.y, qz = -bodyRot.z, qw = bodyRot.w;

                float ox = worldOrigin.x - bodyPos.x;
                float oy = worldOrigin.y - bodyPos.y;
                float oz = worldOrigin.z - bodyPos.z;
                RotateByQuat(qx, qy, qz, qw, ox, oy, oz, out var localOrigin);

                RotateByQuat(qx, qy, qz, qw, worldDirection.x, worldDirection.y, worldDirection.z, out var localDir);

                JPH_RayCastResult hit;
                if (Methods.JPH_Shape_CastRay(shape, &localOrigin, &localDir, &hit) != 0)
                {
                    fraction = hit.fraction;
                    return true;
                }
                return false;
            }
        }

        private static void RotateByQuat(float qx, float qy, float qz, float qw,
            float vx, float vy, float vz, out JPH_Vec3 result)
        {
            float tx = 2f * (qy * vz - qz * vy);
            float ty = 2f * (qz * vx - qx * vz);
            float tz = 2f * (qx * vy - qy * vx);
            result.x = vx + qw * tx + (qy * tz - qz * ty);
            result.y = vy + qw * ty + (qz * tx - qx * tz);
            result.z = vz + qw * tz + (qx * ty - qy * tx);
        }

        #endregion
    }
}
