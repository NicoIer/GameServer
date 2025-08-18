using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace JoltWrapper.Editor
{
    public static class JoltHandles
    {
        private static readonly Color HandleColor = new Color(0.7f, 1f, 0.5f);

        private static void StartHandle()
        {
            Handles.color = HandleColor;
            Handles.matrix = Matrix4x4.identity;
        }

        private static void StartHandle(float3 position, quaternion rotation)
        {
            Handles.color = HandleColor;
            Handles.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        }

        private static void ResetHandle()
        {
            Handles.color = default;
            Handles.matrix = Matrix4x4.identity;
        }

        public static void DrawBoxShape(float3 position, quaternion rotation, JoltBox shape)
        {
            StartHandle(position, rotation);

            var clampedConvexRadius = math.clamp(shape.ConvexRadius, 0f, math.cmin(shape.halfExtents));

            if (clampedConvexRadius == 0f)
            {
                Handles.DrawWireCube(float3.zero, shape.halfExtents * 2f);
            }
            else
            {
                DrawRoundedWireCube(float3.zero, shape.halfExtents * 2f, clampedConvexRadius);
            }

            ResetHandle();
        }

        public static void DrawSphereShape(Vector3 pos, Quaternion rot, JoltSphere shape)
        {
            StartHandle(pos, rot);

            DrawWireSphere(float3.zero, shape.radius);

            ResetHandle();
        }

        private static void DrawWireSphere(float3 position, float radius)
        {
            Handles.DrawWireDisc(position, math.up(), radius);
            Handles.DrawWireDisc(position, math.left(), radius);
            Handles.DrawWireDisc(position, math.forward(), radius);
        }

        
        public static void DrawCapsuleShape(Vector3 position, Quaternion rotation, JoltCapsule shape)
        {
            StartHandle(position, rotation);

            DrawWireCapsule(float3.zero, shape.halfHeight * 2f, shape.radius);

            ResetHandle();
        }

        private static void DrawWireCapsule(float3 position, float height, float radius)
        {
            var h = new float3(0, height * 0.5f, 0);

            Handles.DrawWireDisc(position + h, math.up(), radius);
            Handles.DrawWireDisc(position - h, math.up(), radius);

            var rx = new float3(radius, 0, 0);
            var rz = new float3(0, 0, radius);

            Handles.DrawLine(position + h + rx, position - h + rx);
            Handles.DrawLine(position + h - rx, position - h - rx);

            Handles.DrawLine(position + h + rz, position - h + rz);
            Handles.DrawLine(position + h - rz, position - h - rz);

            // xy plane wire arcs

            DrawWireArcXY(position + h,   0f, 180f, radius);
            DrawWireArcXY(position - h, 180f, 180f, radius);

            // zy plane wire arcs

            DrawWireArcZY(position + h,   0f, 180f, radius);
            DrawWireArcZY(position - h, 180f, 180f, radius);
        }

        private static void DrawRoundedWireCube(float3 position, float3 size, float bevel)
        {
            var fx = new float3(size.x * 0.5f, 0f, 0f); // isolated x face center
            var fy = new float3(0f, size.y * 0.5f, 0f); // isolated y face center
            var fz = new float3(0f, 0f, size.z * 0.5f); // isolated z face center

            var cx = new float3(size.x * 0.5f - bevel, 0f, 0f); // isolated x corner
            var cy = new float3(0f, size.y * 0.5f - bevel, 0f); // isolated y corner
            var cz = new float3(0f, 0f, size.z * 0.5f - bevel); // isolated z corner

            // faces

            DrawQuadXY(+fz, size.xy - bevel * 2f);
            DrawQuadXY(-fz, size.xy - bevel * 2f);

            DrawQuadXZ(+fy, size.xz - bevel * 2f);
            DrawQuadXZ(-fy, size.xz - bevel * 2f);

            DrawQuadYZ(+fx, size.yz - bevel * 2f);
            DrawQuadYZ(-fx, size.yz - bevel * 2f);

            // xy plane arcs

            DrawWireArcXY(position + cx + cy + cz, 0f, 90f, bevel);
            DrawWireArcXY(position - cx + cy + cz, 90f, 90f, bevel);
            DrawWireArcXY(position - cx - cy + cz, 180f, 90f, bevel);
            DrawWireArcXY(position + cx - cy + cz, 270f, 90f, bevel);

            DrawWireArcXY(position + cx + cy - cz, 0f, 90f, bevel);
            DrawWireArcXY(position - cx + cy - cz, 90f, 90f, bevel);
            DrawWireArcXY(position - cx - cy - cz, 180f, 90f, bevel);
            DrawWireArcXY(position + cx - cy - cz, 270f, 90f, bevel);

            // xz plane arcs

            DrawWireArcXZ(position + cx + cy + cz, 0f, 90f, bevel);
            DrawWireArcXZ(position - cx + cy + cz, 90f, 90f, bevel);
            DrawWireArcXZ(position - cx + cy - cz, 180f, 90f, bevel);
            DrawWireArcXZ(position + cx + cy - cz, 270f, 90f, bevel);

            DrawWireArcXZ(position + cx - cy + cz, 0f, 90f, bevel);
            DrawWireArcXZ(position - cx - cy + cz, 90f, 90f, bevel);
            DrawWireArcXZ(position - cx - cy - cz, 180f, 90f, bevel);
            DrawWireArcXZ(position + cx - cy - cz, 270f, 90f, bevel);

            // yz plane arcs

            DrawWireArcZY(position + cx + cy + cz, 0f, 90f, bevel);
            DrawWireArcZY(position + cx + cy - cz, 90f, 90f, bevel);
            DrawWireArcZY(position + cx - cy - cz, 180f, 90f, bevel);
            DrawWireArcZY(position + cx - cy + cz, 270f, 90f, bevel);

            DrawWireArcZY(position - cx + cy + cz, 0f, 90f, bevel);
            DrawWireArcZY(position - cx + cy - cz, 90f, 90f, bevel);
            DrawWireArcZY(position - cx - cy - cz, 180f, 90f, bevel);
            DrawWireArcZY(position - cx - cy + cz, 270f, 90f, bevel);
        }


        /// <summary>
        /// Draw a gizmo quad on the XY plane.
        /// </summary>
        private static void DrawQuadXY(float3 position, float2 size)
        {
            DrawQuad(position, math.left() * 0.5f * size.x, math.up() * 0.5f * size.y);
        }

        /// <summary>
        /// Draw a gizmo quad on the XZ plane.
        /// </summary>
        private static void DrawQuadXZ(float3 position, float2 size)
        {
            DrawQuad(position, math.left() * 0.5f * size.x, math.forward() * 0.5f * size.y);
        }

        /// <summary>
        /// Draw a gizmo quad on the YZ plane.
        /// </summary>
        private static void DrawQuadYZ(float3 position, float2 size)
        {
            DrawQuad(position, math.up() * 0.5f * size.x, math.forward() * 0.5f * size.y);
        }

        /// <summary>
        /// Draw a wire arc on the XY plane.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="start">The start angle in degrees where 0 is the positive X axis and 90 is the positive Y axis.</param>
        /// <param name="angle">The sweep angle in degrees.</param>
        /// <param name="radius">The arc radius.</param>
        private static void DrawWireArcXY(float3 position, float start, float angle, float radius)
        {
            var t = math.radians(start);
            var v = new float3(math.cos(t), math.sin(t), 0f);

            Handles.DrawWireArc(position, math.forward(), v, angle, radius);
        }

        /// <summary>
        /// Draw a wire arc on the XZ plane.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="start">The start angle in degrees where 0 is the positive X axis and 90 is the positive Z axis.</param>
        /// <param name="angle">The sweep angle in degrees.</param>
        /// <param name="radius">The arc radius.</param>
        private static void DrawWireArcXZ(float3 position, float start, float angle, float radius)
        {
            var t = math.radians(start);
            var v = new float3(math.cos(t), 0f, math.sin(t));

            Handles.DrawWireArc(position, math.down(), v, angle, radius);
        }

        /// <summary>
        /// Draw a wire arc on the ZY plane.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="start">The start angle in degrees where 0 is the positive Z axis and 90 is the positive Y axis.</param>
        /// <param name="angle">The sweep angle in degrees.</param>
        /// <param name="radius">The arc radius.</param>
        private static void DrawWireArcZY(float3 position, float start, float angle, float radius)
        {
            var t = math.radians(start);
            var v = new float3(0f, math.sin(t), math.cos(t));

            Handles.DrawWireArc(position, math.left(), v, angle, radius);
        }

        private static void DrawQuad(float3 position, float3 u, float3 v)
        {
            var a = position + u + v;
            var b = position - u + v;
            var c = position - u - v;
            var d = position + u - v;

            Handles.DrawLine(a, b);
            Handles.DrawLine(b, c);
            Handles.DrawLine(c, d);
            Handles.DrawLine(d, a);
        }
    }
}