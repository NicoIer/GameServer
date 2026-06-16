// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class BroadPhaseLayerInterfaceTable : NativeHandle
    {
        public BroadPhaseLayerInterfaceTable(uint numObjectLayers, uint numBroadPhaseLayers)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_BroadPhaseLayerInterfaceTable_Create(
                    numObjectLayers, numBroadPhaseLayers);
            }
        }

        public void MapObjectToBroadPhaseLayer(ObjectLayer objectLayer, BroadPhaseLayer broadPhaseLayer)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_BroadPhaseLayerInterfaceTable_MapObjectToBroadPhaseLayer(
                    (JPH_BroadPhaseLayerInterface*)Handle, objectLayer.Value, broadPhaseLayer.Value);
            }
        }

        protected override void DestroyNative()
        {
            // Destroyed by JPH_PhysicsSystem_Destroy — do not double-free.
        }
    }
}
