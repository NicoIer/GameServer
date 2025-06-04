using GameCore.Jolt;
using Jolt;

namespace Game.Jolt
{
    public class JoltApplication
    {
        
        protected static void SetupCollisionFiltering(ref PhysicsSystemSettings settings)
        {
            // We use only 2 layers: one for non-moving objects and one for moving objects
            ObjectLayerPairFilterTable objectLayerPairFilter = new(2);
            objectLayerPairFilter.EnableCollision((ushort)ObjectLayers.NonMoving, (byte)ObjectLayers.Moving);
            objectLayerPairFilter.EnableCollision((ushort)ObjectLayers.Moving, (byte)ObjectLayers.Moving);

            // We use a 1-to-1 mapping between object layers and broadphase layers
            BroadPhaseLayerInterfaceTable broadPhaseLayerInterface = new(2, 2);
            broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((ushort)ObjectLayers.NonMoving,
                (byte)BroadPhaseLayers.NonMoving);
            broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((ushort)ObjectLayers.Moving,
                (byte)BroadPhaseLayers.Moving);

            ObjectVsBroadPhaseLayerFilterTable objectVsBroadPhaseLayerFilter =
                new(broadPhaseLayerInterface, 2, objectLayerPairFilter, 2);

            settings.ObjectLayerPairFilter = objectLayerPairFilter;
            settings.BroadPhaseLayerInterface = broadPhaseLayerInterface;
            settings.ObjectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter;
        }
    }
}