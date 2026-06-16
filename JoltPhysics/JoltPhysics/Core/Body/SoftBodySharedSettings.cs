// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.InteropServices;

namespace JoltPhysics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SoftVertex
    {
        public Float3 Position;
        public Float3 Velocity;
        public float InvMass;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SoftFace
    {
        public uint Vertex1;
        public uint Vertex2;
        public uint Vertex3;
        public uint MaterialIndex;
    }

    public sealed class SoftBodySharedSettings : NativeHandle
    {
        public SoftBodySharedSettings() : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_SoftBodySharedSettings_Create();
            }
        }

        public void AddVertex(SoftVertex vertex)
        {
            ThrowIfDisposed();
            unsafe
            {
                var native = new JPH_SoftVertex
                {
                    position = new JPH_Vec3 { x = vertex.Position.x, y = vertex.Position.y, z = vertex.Position.z },
                    velocity = new JPH_Vec3 { x = vertex.Velocity.x, y = vertex.Velocity.y, z = vertex.Velocity.z },
                    invMass = vertex.InvMass,
                };
                Methods.JPH_SoftBodySharedSettings_AddVertex((JPH_SoftBodySharedSettings*)Handle, &native);
            }
        }

        public void AddVertices(SoftVertex[] vertices)
        {
            ThrowIfDisposed();
            unsafe
            {
                var natives = stackalloc JPH_SoftVertex[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    natives[i] = new JPH_SoftVertex
                    {
                        position = new JPH_Vec3
                        {
                            x = vertices[i].Position.x,
                            y = vertices[i].Position.y,
                            z = vertices[i].Position.z,
                        },
                        velocity = new JPH_Vec3
                        {
                            x = vertices[i].Velocity.x,
                            y = vertices[i].Velocity.y,
                            z = vertices[i].Velocity.z,
                        },
                        invMass = vertices[i].InvMass,
                    };
                }
                Methods.JPH_SoftBodySharedSettings_AddVertices(
                    (JPH_SoftBodySharedSettings*)Handle, natives, (uint)vertices.Length);
            }
        }

        public bool RemoveVertex(uint index)
        {
            ThrowIfDisposed();
            unsafe
            {
                return Methods.JPH_SoftBodySharedSettings_RemoveVertex(
                    (JPH_SoftBodySharedSettings*)Handle, index) != 0;
            }
        }

        public uint VertexCount
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SoftBodySharedSettings_GetVertexCount(
                        (JPH_SoftBodySharedSettings*)Handle);
                }
            }
        }

        public bool GetVertex(uint index, out SoftVertex vertex)
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_SoftVertex native;
                bool ok = Methods.JPH_SoftBodySharedSettings_GetVertex(
                    (JPH_SoftBodySharedSettings*)Handle, index, &native) != 0;
                vertex = new SoftVertex
                {
                    Position = new Float3(native.position.x, native.position.y, native.position.z),
                    Velocity = new Float3(native.velocity.x, native.velocity.y, native.velocity.z),
                    InvMass = native.invMass,
                };
                return ok;
            }
        }

        public void AddFace(SoftFace face)
        {
            ThrowIfDisposed();
            unsafe
            {
                var native = new JPH_SoftFace
                {
                    vertex1 = face.Vertex1,
                    vertex2 = face.Vertex2,
                    vertex3 = face.Vertex3,
                    materialIndex = face.MaterialIndex,
                };
                Methods.JPH_SoftBodySharedSettings_AddFace((JPH_SoftBodySharedSettings*)Handle, &native);
            }
        }

        public void AddFaces(SoftFace[] faces)
        {
            ThrowIfDisposed();
            unsafe
            {
                var natives = stackalloc JPH_SoftFace[faces.Length];
                for (int i = 0; i < faces.Length; i++)
                {
                    natives[i] = new JPH_SoftFace
                    {
                        vertex1 = faces[i].Vertex1,
                        vertex2 = faces[i].Vertex2,
                        vertex3 = faces[i].Vertex3,
                        materialIndex = faces[i].MaterialIndex,
                    };
                }
                Methods.JPH_SoftBodySharedSettings_AddFaces(
                    (JPH_SoftBodySharedSettings*)Handle, natives, (uint)faces.Length);
            }
        }

        public bool RemoveFace(uint index)
        {
            ThrowIfDisposed();
            unsafe
            {
                return Methods.JPH_SoftBodySharedSettings_RemoveFace(
                    (JPH_SoftBodySharedSettings*)Handle, index) != 0;
            }
        }

        public uint FaceCount
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SoftBodySharedSettings_GetFaceCount(
                        (JPH_SoftBodySharedSettings*)Handle);
                }
            }
        }

        public bool GetFace(uint index, out SoftFace face)
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_SoftFace native;
                bool ok = Methods.JPH_SoftBodySharedSettings_GetFace(
                    (JPH_SoftBodySharedSettings*)Handle, index, &native) != 0;
                face = new SoftFace
                {
                    Vertex1 = native.vertex1,
                    Vertex2 = native.vertex2,
                    Vertex3 = native.vertex3,
                    MaterialIndex = native.materialIndex,
                };
                return ok;
            }
        }

        public void CreateConstraints(float compliance, SoftBodyBendType bendType)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_SoftBodySharedSettings_CreateConstraints(
                    (JPH_SoftBodySharedSettings*)Handle, compliance, (JPH_SoftBodyBendType)bendType);
            }
        }

        public void Optimize()
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_SoftBodySharedSettings_Optimize((JPH_SoftBodySharedSettings*)Handle);
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_SoftBodySharedSettings_Destroy((JPH_SoftBodySharedSettings*)Handle);
            }
        }
    }
}
