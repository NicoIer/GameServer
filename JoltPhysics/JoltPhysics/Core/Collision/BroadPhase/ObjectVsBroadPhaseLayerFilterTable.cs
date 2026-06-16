// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class ObjectVsBroadPhaseLayerFilterTable : NativeHandle
    {
        public ObjectVsBroadPhaseLayerFilterTable(
            BroadPhaseLayerInterfaceTable broadPhaseLayerInterface,
            uint numBroadPhaseLayers,
            ObjectLayerPairFilterTable objectLayerPairFilter,
            uint numObjectLayers)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_ObjectVsBroadPhaseLayerFilterTable_Create(
                    (JPH_BroadPhaseLayerInterface*)broadPhaseLayerInterface.Handle,
                    numBroadPhaseLayers,
                    (JPH_ObjectLayerPairFilter*)objectLayerPairFilter.Handle,
                    numObjectLayers);
            }
        }

        protected override void DestroyNative()
        {
            // Destroyed by JPH_PhysicsSystem_Destroy — do not double-free.
        }
    }
}
