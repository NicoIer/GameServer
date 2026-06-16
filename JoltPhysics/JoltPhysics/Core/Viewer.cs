// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

#if !UNITY && !UNITY_5_3_OR_NEWER

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace JoltPhysics
{
    public enum ViewerBackend : uint
    {
        Auto = 0,
        Metal = 1,
        Vulkan = 2,
        DX12 = 3,
    }

    public enum DebugCastShadow : uint
    {
        On = 0,
        Off = 1,
    }

    public enum DebugDrawMode : uint
    {
        Solid = 0,
        Wireframe = 1,
    }

    public readonly struct DebugColor
    {
        public readonly uint Value;

        public DebugColor(uint value)
        {
            Value = value;
        }

        public static DebugColor Rgba(byte r, byte g, byte b, byte a = 255)
        {
            return new DebugColor((uint)(r | (g << 8) | (b << 16) | (a << 24)));
        }

        public static readonly DebugColor Black = Rgba(0, 0, 0);
        public static readonly DebugColor Red = Rgba(255, 0, 0);
        public static readonly DebugColor Green = Rgba(0, 255, 0);
        public static readonly DebugColor Blue = Rgba(0, 0, 255);
        public static readonly DebugColor Yellow = Rgba(255, 255, 0);
        public static readonly DebugColor Cyan = Rgba(0, 255, 255);
        public static readonly DebugColor Magenta = Rgba(255, 0, 255);
        public static readonly DebugColor Orange = Rgba(255, 165, 0);
        public static readonly DebugColor White = Rgba(255, 255, 255);
        public static readonly DebugColor Grey = Rgba(128, 128, 128);
    }

    public enum BodyManagerShapeColor : uint
    {
        InstanceColor = 0,
        ShapeTypeColor = 1,
        MotionTypeColor = 2,
        SleepColor = 3,
        IslandColor = 4,
        MaterialColor = 5,
    }

    public readonly struct ViewerStats
    {
        public readonly uint VertexCount;
        public readonly uint LineCount;
        public readonly uint TriangleCount;
        public readonly uint TextCount;

        internal ViewerStats(JPH_ViewerStats stats)
        {
            VertexCount = stats.vertexCount;
            LineCount = stats.lineCount;
            TriangleCount = stats.triangleCount;
            TextCount = stats.textCount;
        }
    }

    public struct ViewerSettings
    {
        public string? Title;
        public ViewerBackend Backend;
        public int Width;
        public int Height;
        public Float3 CameraPosition;
        public Float3 CameraTarget;
        public float WorldScale;

        public static ViewerSettings Default
        {
            get
            {
                unsafe
                {
                    JPH_ViewerSettings native;
                    Methods.JPH_ViewerSettings_InitDefault(&native);
                    return FromNative(native);
                }
            }
        }

        internal unsafe JPH_ViewerSettings ToNative(sbyte* title)
        {
            return new JPH_ViewerSettings
            {
                title = title,
                backend = (JPH_ViewerBackend)Backend,
                width = Width,
                height = Height,
                cameraPosition = ViewerNative.ToNative(CameraPosition),
                cameraTarget = ViewerNative.ToNative(CameraTarget),
                worldScale = WorldScale,
            };
        }

        private static unsafe ViewerSettings FromNative(JPH_ViewerSettings native)
        {
            return new ViewerSettings
            {
                Title = native.title != null ? Marshal.PtrToStringUTF8((IntPtr)native.title) : null,
                Backend = (ViewerBackend)native.backend,
                Width = native.width,
                Height = native.height,
                CameraPosition = ViewerNative.FromNative(native.cameraPosition),
                CameraTarget = ViewerNative.FromNative(native.cameraTarget),
                WorldScale = native.worldScale,
            };
        }
    }

    public struct ViewerDrawSettings
    {
        public bool DrawGetSupportFunction;
        public bool DrawSupportDirection;
        public bool DrawGetSupportingFace;
        public bool DrawShape;
        public bool DrawShapeWireframe;
        public BodyManagerShapeColor DrawShapeColor;
        public bool DrawBoundingBox;
        public bool DrawCenterOfMassTransform;
        public bool DrawWorldTransform;
        public bool DrawVelocity;
        public bool DrawMassAndInertia;
        public bool DrawSleepStats;
        public bool DrawSoftBodyVertices;
        public bool DrawSoftBodyVertexVelocities;
        public bool DrawSoftBodyEdgeConstraints;
        public bool DrawSoftBodyBendConstraints;
        public bool DrawSoftBodyVolumeConstraints;
        public bool DrawSoftBodySkinConstraints;
        public bool DrawSoftBodyLRAConstraints;
        public bool DrawSoftBodyPredictedBounds;

        public static ViewerDrawSettings Default
        {
            get
            {
                unsafe
                {
                    JPH_DrawSettings native;
                    Methods.JPH_DrawSettings_InitDefault(&native);
                    return FromNative(native);
                }
            }
        }

        public static ViewerDrawSettings Shapes => Default with { DrawShape = true };

        internal JPH_DrawSettings ToNative()
        {
            unsafe
            {
                JPH_DrawSettings native;
                Methods.JPH_DrawSettings_InitDefault(&native);
                native.drawGetSupportFunction = ViewerNative.ToNative(DrawGetSupportFunction);
                native.drawSupportDirection = ViewerNative.ToNative(DrawSupportDirection);
                native.drawGetSupportingFace = ViewerNative.ToNative(DrawGetSupportingFace);
                native.drawShape = ViewerNative.ToNative(DrawShape);
                native.drawShapeWireframe = ViewerNative.ToNative(DrawShapeWireframe);
                native.drawShapeColor = (JPH_BodyManager_ShapeColor)DrawShapeColor;
                native.drawBoundingBox = ViewerNative.ToNative(DrawBoundingBox);
                native.drawCenterOfMassTransform = ViewerNative.ToNative(DrawCenterOfMassTransform);
                native.drawWorldTransform = ViewerNative.ToNative(DrawWorldTransform);
                native.drawVelocity = ViewerNative.ToNative(DrawVelocity);
                native.drawMassAndInertia = ViewerNative.ToNative(DrawMassAndInertia);
                native.drawSleepStats = ViewerNative.ToNative(DrawSleepStats);
                native.drawSoftBodyVertices = ViewerNative.ToNative(DrawSoftBodyVertices);
                native.drawSoftBodyVertexVelocities = ViewerNative.ToNative(DrawSoftBodyVertexVelocities);
                native.drawSoftBodyEdgeConstraints = ViewerNative.ToNative(DrawSoftBodyEdgeConstraints);
                native.drawSoftBodyBendConstraints = ViewerNative.ToNative(DrawSoftBodyBendConstraints);
                native.drawSoftBodyVolumeConstraints = ViewerNative.ToNative(DrawSoftBodyVolumeConstraints);
                native.drawSoftBodySkinConstraints = ViewerNative.ToNative(DrawSoftBodySkinConstraints);
                native.drawSoftBodyLRAConstraints = ViewerNative.ToNative(DrawSoftBodyLRAConstraints);
                native.drawSoftBodyPredictedBounds = ViewerNative.ToNative(DrawSoftBodyPredictedBounds);
                return native;
            }
        }

        private static ViewerDrawSettings FromNative(JPH_DrawSettings native)
        {
            return new ViewerDrawSettings
            {
                DrawGetSupportFunction = ViewerNative.FromNative(native.drawGetSupportFunction),
                DrawSupportDirection = ViewerNative.FromNative(native.drawSupportDirection),
                DrawGetSupportingFace = ViewerNative.FromNative(native.drawGetSupportingFace),
                DrawShape = ViewerNative.FromNative(native.drawShape),
                DrawShapeWireframe = ViewerNative.FromNative(native.drawShapeWireframe),
                DrawShapeColor = (BodyManagerShapeColor)native.drawShapeColor,
                DrawBoundingBox = ViewerNative.FromNative(native.drawBoundingBox),
                DrawCenterOfMassTransform = ViewerNative.FromNative(native.drawCenterOfMassTransform),
                DrawWorldTransform = ViewerNative.FromNative(native.drawWorldTransform),
                DrawVelocity = ViewerNative.FromNative(native.drawVelocity),
                DrawMassAndInertia = ViewerNative.FromNative(native.drawMassAndInertia),
                DrawSleepStats = ViewerNative.FromNative(native.drawSleepStats),
                DrawSoftBodyVertices = ViewerNative.FromNative(native.drawSoftBodyVertices),
                DrawSoftBodyVertexVelocities = ViewerNative.FromNative(native.drawSoftBodyVertexVelocities),
                DrawSoftBodyEdgeConstraints = ViewerNative.FromNative(native.drawSoftBodyEdgeConstraints),
                DrawSoftBodyBendConstraints = ViewerNative.FromNative(native.drawSoftBodyBendConstraints),
                DrawSoftBodyVolumeConstraints = ViewerNative.FromNative(native.drawSoftBodyVolumeConstraints),
                DrawSoftBodySkinConstraints = ViewerNative.FromNative(native.drawSoftBodySkinConstraints),
                DrawSoftBodyLRAConstraints = ViewerNative.FromNative(native.drawSoftBodyLRAConstraints),
                DrawSoftBodyPredictedBounds = ViewerNative.FromNative(native.drawSoftBodyPredictedBounds),
            };
        }
    }

    public sealed unsafe class Viewer : NativeHandle
    {
        public delegate bool FrameCallback(Viewer viewer, float deltaTime);

        private static readonly delegate* unmanaged[Cdecl]<JPH_Viewer*, float, void*, byte> s_frameCallback = &OnFrame;
        private GCHandle _callbackHandle;

        private Viewer(IntPtr handle)
            : base(handle, true)
        {
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create Jolt viewer.");
        }

        public static Viewer CreateObj(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path must not be null or empty.", nameof(path));

            byte[] pathBytes = NullTerminatedUtf8(path);
            fixed (byte* pathPtr = pathBytes)
                return new Viewer((IntPtr)Methods.JPH_Viewer_CreateObj((sbyte*)pathPtr));
        }
        
        public static Viewer CreateViewer(ViewerSettings settings)
        {
            return Viewer.CreateWindowed(settings);
        }

        public void SetCameraPosition(Float3 position)
        {
            ThrowIfDisposed();
            var native = ViewerNative.ToNative(position);
            Methods.JPH_Viewer_SetCameraPosition((JPH_Viewer*)Handle, &native);
        }

        public void SetCameraLookAt(Float3 position, Float3 target)
        {
            ThrowIfDisposed();
            var nativePosition = ViewerNative.ToNative(position);
            var nativeTarget = ViewerNative.ToNative(target);
            Methods.JPH_Viewer_SetCameraLookAt((JPH_Viewer*)Handle, &nativePosition, &nativeTarget);
        }

        public void SetCameraInputEnabled(bool enabled)
        {
            ThrowIfDisposed();
            Methods.JPH_Viewer_SetCameraInputEnabled((JPH_Viewer*)Handle, ViewerNative.ToNative(enabled));
        }

        public void SetCameraMoveSpeed(float speed)
        {
            ThrowIfDisposed();
            Methods.JPH_Viewer_SetCameraMoveSpeed((JPH_Viewer*)Handle, speed);
        }

        public void SetCameraLookSpeed(float degreesPerPixel)
        {
            ThrowIfDisposed();
            Methods.JPH_Viewer_SetCameraLookSpeed((JPH_Viewer*)Handle, degreesPerPixel);
        }

        public void FocusCamera(Float3 target, float distance = 10.0f)
        {
            ThrowIfDisposed();
            var nativeTarget = ViewerNative.ToNative(target);
            Methods.JPH_Viewer_FocusCamera((JPH_Viewer*)Handle, &nativeTarget, distance);
        }

        public void Clear()
        {
            ThrowIfDisposed();
            Methods.JPH_Viewer_Clear((JPH_Viewer*)Handle);
        }

        public bool PollEvents()
        {
            ThrowIfDisposed();
            return Methods.JPH_Viewer_PollEvents((JPH_Viewer*)Handle) != 0;
        }

        public bool ShouldClose
        {
            get
            {
                ThrowIfDisposed();
                return Methods.JPH_Viewer_ShouldClose((JPH_Viewer*)Handle) != 0;
            }
        }

        public bool RenderFrame(float deltaTime)
        {
            ThrowIfDisposed();
            return Methods.JPH_Viewer_RenderFrame((JPH_Viewer*)Handle, deltaTime) != 0;
        }

        public bool Tick(float deltaTime)
        {
            ThrowIfDisposed();
            return Methods.JPH_Viewer_PollEvents((JPH_Viewer*)Handle) != 0
                && Methods.JPH_Viewer_RenderFrame((JPH_Viewer*)Handle, deltaTime) != 0;
        }

        public void Run(FrameCallback callback)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(callback);

            if (_callbackHandle.IsAllocated)
                throw new InvalidOperationException("Viewer is already running.");

            _callbackHandle = GCHandle.Alloc(new CallbackState(this, callback));
            try
            {
                Methods.JPH_Viewer_Run((JPH_Viewer*)Handle, s_frameCallback, (void*)GCHandle.ToIntPtr(_callbackHandle));
            }
            finally
            {
                _callbackHandle.Free();
            }
        }

        public void NextFrame()
        {
            ThrowIfDisposed();
            Methods.JPH_Viewer_NextFrame((JPH_Viewer*)Handle);
        }

        public bool Flush()
        {
            ThrowIfDisposed();
            return Methods.JPH_Viewer_Flush((JPH_Viewer*)Handle) != 0;
        }

        public ViewerStats GetStats()
        {
            ThrowIfDisposed();
            JPH_ViewerStats stats;
            Methods.JPH_Viewer_GetStats((JPH_Viewer*)Handle, &stats);
            return new ViewerStats(stats);
        }

        public void DrawLine(Float3 from, Float3 to, DebugColor color)
        {
            ThrowIfDisposed();
            var nativeFrom = ViewerNative.ToNative(from);
            var nativeTo = ViewerNative.ToNative(to);
            Methods.JPH_Viewer_DrawLine((JPH_Viewer*)Handle, &nativeFrom, &nativeTo, color.Value);
        }

        public void DrawTriangle(Float3 v1, Float3 v2, Float3 v3, DebugColor color, DebugCastShadow castShadow = DebugCastShadow.Off)
        {
            ThrowIfDisposed();
            var nativeV1 = ViewerNative.ToNative(v1);
            var nativeV2 = ViewerNative.ToNative(v2);
            var nativeV3 = ViewerNative.ToNative(v3);
            Methods.JPH_Viewer_DrawTriangle((JPH_Viewer*)Handle, &nativeV1, &nativeV2, &nativeV3, color.Value, (JPH_DebugRenderer_CastShadow)castShadow);
        }

        public void DrawBox(BoundingBox box, DebugColor color, DebugCastShadow castShadow = DebugCastShadow.On, DebugDrawMode drawMode = DebugDrawMode.Solid)
        {
            ThrowIfDisposed();
            var nativeBox = new JPH_AABox
            {
                min = ViewerNative.ToNative(box.Min),
                max = ViewerNative.ToNative(box.Max),
            };
            Methods.JPH_Viewer_DrawBox((JPH_Viewer*)Handle, &nativeBox, color.Value, (JPH_DebugRenderer_CastShadow)castShadow, (JPH_DebugRenderer_DrawMode)drawMode);
        }

        public void DrawSphere(Float3 center, float radius, DebugColor color, DebugCastShadow castShadow = DebugCastShadow.On, DebugDrawMode drawMode = DebugDrawMode.Solid)
        {
            ThrowIfDisposed();
            var nativeCenter = ViewerNative.ToNative(center);
            Methods.JPH_Viewer_DrawSphere((JPH_Viewer*)Handle, &nativeCenter, radius, color.Value, (JPH_DebugRenderer_CastShadow)castShadow, (JPH_DebugRenderer_DrawMode)drawMode);
        }

        public void DrawShape(Shape shape, Float3 position, Quat rotation, DebugColor color, bool useMaterialColors = false, bool wireframe = false)
        {
            DrawShape(shape, position, rotation, Float3.One, color, useMaterialColors, wireframe);
        }

        public void DrawShape(Shape shape, Float3 position, Quat rotation, Float3 scale, DebugColor color, bool useMaterialColors = false, bool wireframe = false)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(shape);
            shape.ThrowIfDisposed();

            var nativePosition = ViewerNative.ToNative(position);
            var nativeScale = ViewerNative.ToNative(scale);
            Methods.JPH_Viewer_DrawShape(
                (JPH_Viewer*)Handle,
                (JPH_Shape*)shape.Handle,
                &nativePosition,
                (JPH_Quat*)&rotation,
                &nativeScale,
                color.Value,
                ViewerNative.ToNative(useMaterialColors),
                ViewerNative.ToNative(wireframe));
        }

        public void DrawBodies(PhysicsSystem system, ViewerDrawSettings settings)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(system);
            system.ThrowIfDisposed();

            var native = settings.ToNative();
            Methods.JPH_Viewer_DrawBodies((JPH_Viewer*)Handle, (JPH_PhysicsSystem*)system.Handle, &native, null);
        }

        public void DrawConstraints(PhysicsSystem system)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(system);
            system.ThrowIfDisposed();
            Methods.JPH_Viewer_DrawConstraints((JPH_Viewer*)Handle, (JPH_PhysicsSystem*)system.Handle);
        }

        public void DrawConstraintLimits(PhysicsSystem system)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(system);
            system.ThrowIfDisposed();
            Methods.JPH_Viewer_DrawConstraintLimits((JPH_Viewer*)Handle, (JPH_PhysicsSystem*)system.Handle);
        }

        public void DrawConstraintReferenceFrame(PhysicsSystem system)
        {
            ThrowIfDisposed();
            ArgumentNullException.ThrowIfNull(system);
            system.ThrowIfDisposed();
            Methods.JPH_Viewer_DrawConstraintReferenceFrame((JPH_Viewer*)Handle, (JPH_PhysicsSystem*)system.Handle);
        }

        protected override void DestroyNative()
        {
            if (_callbackHandle.IsAllocated)
                _callbackHandle.Free();

            Methods.JPH_Viewer_Destroy((JPH_Viewer*)Handle);
        }

        private static Viewer CreateWindowed(ViewerSettings settings)
        {
            byte[] titleBytes = NullTerminatedUtf8(settings.Title);
            fixed (byte* titlePtr = titleBytes)
            {
                var native = settings.ToNative((sbyte*)titlePtr);
                native.backend = (JPH_ViewerBackend)settings.Backend;

                JPH_Viewer* viewer = settings.Backend switch
                {
                    ViewerBackend.Metal => Methods.JPH_Viewer_CreateMetal(&native),
                    ViewerBackend.Vulkan => Methods.JPH_Viewer_CreateVulkan(&native),
                    ViewerBackend.DX12 => Methods.JPH_Viewer_CreateDX12(&native),
                    _ => Methods.JPH_Viewer_CreateWindowed(&native),
                };

                return new Viewer((IntPtr)viewer);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static byte OnFrame(JPH_Viewer* viewer, float deltaTime, void* userData)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            var state = (CallbackState)handle.Target!;
            return ViewerNative.ToNative(state.Callback(state.Viewer, deltaTime));
        }

        private static byte[] NullTerminatedUtf8(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return new byte[] { 0 };

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            Array.Resize(ref bytes, bytes.Length + 1);
            return bytes;
        }

        private sealed class CallbackState
        {
            public readonly Viewer Viewer;
            public readonly FrameCallback Callback;

            public CallbackState(Viewer viewer, FrameCallback callback)
            {
                Viewer = viewer;
                Callback = callback;
            }
        }
    }

    internal static class ViewerNative
    {
        public static JPH_Vec3 ToNative(Float3 value)
        {
            return new JPH_Vec3 { x = value.x, y = value.y, z = value.z };
        }

        public static Float3 FromNative(JPH_Vec3 value)
        {
            return new Float3(value.x, value.y, value.z);
        }

        public static byte ToNative(bool value)
        {
            return (byte)(value ? 1 : 0);
        }

        public static bool FromNative(byte value)
        {
            return value != 0;
        }
    }
}

#endif
