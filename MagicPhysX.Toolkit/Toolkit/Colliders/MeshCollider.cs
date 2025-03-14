// using System.Runtime.CompilerServices;
//
// namespace MagicPhysX.Toolkit.Colliders
// {
//     public unsafe class MeshCollider : Collider
//     {
//         ref PxConvexMeshGeometry GetGeometry() => ref Unsafe.AsRef<PxConvexMeshGeometry>(shape->GetGeometry());
//         
//         public MeshCollider(PxShape* shape, ColliderType type) : base(shape, type)
//         {
//         }
//     }
// }