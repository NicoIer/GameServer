// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System.Numerics;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public static class JoltAPI
    {
        public static bool Init()
        {
            return Methods.JPH_Init() != 0;
        }

        public static void Shutdown()
        {
            Methods.JPH_Shutdown();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mat4 MakeTransform(Quaternion rot, Vector3 from)
        {
            unsafe
            {
                JPH_Mat4 mat;
                JPH_Quat q = *(JPH_Quat*)&rot;
                JPH_Vec3 p = *(JPH_Vec3*)&from;
                Methods.JPH_Mat4_RotationTranslation(&mat, &q, &p);
                return new Mat4
                {
                    Column0 = new Float4(mat.Column0.x, mat.Column0.y, mat.Column0.z, mat.Column0.w),
                    Column1 = new Float4(mat.Column1.x, mat.Column1.y, mat.Column1.z, mat.Column1.w),
                    Column2 = new Float4(mat.Column2.x, mat.Column2.y, mat.Column2.z, mat.Column2.w),
                    Column3 = new Float4(mat.Column3.x, mat.Column3.y, mat.Column3.z, mat.Column3.w),
                };
            }
        }
    }
}