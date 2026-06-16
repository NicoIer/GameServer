using System.Runtime.InteropServices;

namespace JoltPhysics
{
    internal partial struct JPH_Viewer
    {
    }

    [NativeTypeName("unsigned int")]
    internal enum JPH_ViewerBackend : uint
    {
        JPH_ViewerBackend_Auto = 0,
        JPH_ViewerBackend_Metal = 1,
        JPH_ViewerBackend_Vulkan = 2,
        JPH_ViewerBackend_DX12 = 3,
    }

    internal unsafe partial struct JPH_ViewerSettings
    {
        [NativeTypeName("const char *")]
        public sbyte* title;

        public JPH_ViewerBackend backend;

        public int width;

        public int height;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 cameraPosition;

        [NativeTypeName("JPH_RVec3")]
        public JPH_Vec3 cameraTarget;

        public float worldScale;
    }

    internal partial struct JPH_ViewerStats
    {
        [NativeTypeName("uint32_t")]
        public uint vertexCount;

        [NativeTypeName("uint32_t")]
        public uint lineCount;

        [NativeTypeName("uint32_t")]
        public uint triangleCount;

        [NativeTypeName("uint32_t")]
        public uint textCount;
    }

    internal static unsafe partial class Methods
    {
        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_ViewerSettings_InitDefault(JPH_ViewerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Viewer* JPH_Viewer_CreateObj([NativeTypeName("const char *")] sbyte* path);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Viewer* JPH_Viewer_CreateWindowed([NativeTypeName("const JPH_ViewerSettings *")] JPH_ViewerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Viewer* JPH_Viewer_CreateMetal([NativeTypeName("const JPH_ViewerSettings *")] JPH_ViewerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Viewer* JPH_Viewer_CreateVulkan([NativeTypeName("const JPH_ViewerSettings *")] JPH_ViewerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_Viewer* JPH_Viewer_CreateDX12([NativeTypeName("const JPH_ViewerSettings *")] JPH_ViewerSettings* settings);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_Destroy(JPH_Viewer* viewer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern JPH_DebugRenderer* JPH_Viewer_GetDebugRenderer(JPH_Viewer* viewer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_SetCameraPosition(JPH_Viewer* viewer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_SetCameraLookAt(JPH_Viewer* viewer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* target);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_SetCameraInputEnabled(JPH_Viewer* viewer, [NativeTypeName("bool")] byte enabled);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_SetCameraMoveSpeed(JPH_Viewer* viewer, float speed);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_SetCameraLookSpeed(JPH_Viewer* viewer, float degreesPerPixel);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_FocusCamera(JPH_Viewer* viewer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* target, float distance);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_Clear(JPH_Viewer* viewer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Viewer_PollEvents(JPH_Viewer* viewer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Viewer_ShouldClose([NativeTypeName("const JPH_Viewer *")] JPH_Viewer* viewer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Viewer_RenderFrame(JPH_Viewer* viewer, float deltaTime);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_Run(JPH_Viewer* viewer, [NativeTypeName("JPH_ViewerFrameCallback")] delegate* unmanaged[Cdecl]<JPH_Viewer*, float, void*, byte> callback, void* userData);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_NextFrame(JPH_Viewer* viewer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte JPH_Viewer_Flush(JPH_Viewer* viewer);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_GetStats([NativeTypeName("const JPH_Viewer *")] JPH_Viewer* viewer, JPH_ViewerStats* stats);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_DrawLine(JPH_Viewer* viewer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* from, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* to, [NativeTypeName("JPH_Color")] uint color);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_DrawTriangle(JPH_Viewer* viewer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* v1, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* v2, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* v3, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_DrawBox(JPH_Viewer* viewer, [NativeTypeName("const JPH_AABox *")] JPH_AABox* box, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_DrawSphere(JPH_Viewer* viewer, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* center, float radius, [NativeTypeName("JPH_Color")] uint color, JPH_DebugRenderer_CastShadow castShadow, JPH_DebugRenderer_DrawMode drawMode);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_DrawShape(JPH_Viewer* viewer, [NativeTypeName("const JPH_Shape *")] JPH_Shape* shape, [NativeTypeName("const JPH_RVec3 *")] JPH_Vec3* position, [NativeTypeName("const JPH_Quat *")] JPH_Quat* rotation, [NativeTypeName("const JPH_Vec3 *")] JPH_Vec3* scale, [NativeTypeName("JPH_Color")] uint color, [NativeTypeName("bool")] byte useMaterialColors, [NativeTypeName("bool")] byte drawWireframe);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_DrawBodies(JPH_Viewer* viewer, JPH_PhysicsSystem* system, [NativeTypeName("const JPH_DrawSettings *")] JPH_DrawSettings* settings, [NativeTypeName("const JPH_BodyDrawFilter *")] JPH_BodyDrawFilter* bodyFilter);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_DrawConstraints(JPH_Viewer* viewer, JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_DrawConstraintLimits(JPH_Viewer* viewer, JPH_PhysicsSystem* system);

        [DllImport(jolt_dll.DLL_NAME, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void JPH_Viewer_DrawConstraintReferenceFrame(JPH_Viewer* viewer, JPH_PhysicsSystem* system);
    }
}
