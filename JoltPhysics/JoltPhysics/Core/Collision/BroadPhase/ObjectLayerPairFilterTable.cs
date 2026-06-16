// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class ObjectLayerPairFilterTable : NativeHandle
    {
        public ObjectLayerPairFilterTable(uint numObjectLayers)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_ObjectLayerPairFilterTable_Create(numObjectLayers);
            }
        }

        public void EnableCollision(ObjectLayer layer1, ObjectLayer layer2)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_ObjectLayerPairFilterTable_EnableCollision(
                    (JPH_ObjectLayerPairFilter*)Handle, layer1.Value, layer2.Value);
            }
        }

        public void DisableCollision(ObjectLayer layer1, ObjectLayer layer2)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_ObjectLayerPairFilterTable_DisableCollision(
                    (JPH_ObjectLayerPairFilter*)Handle, layer1.Value, layer2.Value);
            }
        }

        protected override void DestroyNative()
        {
            // Destroyed by JPH_PhysicsSystem_Destroy — do not double-free.
        }
    }
}
