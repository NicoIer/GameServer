// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

#ifndef JOLT_VIEWER_C_H_
#define JOLT_VIEWER_C_H_ 1

#include "joltc.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct JPH_Viewer JPH_Viewer;

typedef enum JPH_ViewerBackend {
	JPH_ViewerBackend_Auto = 0,
	JPH_ViewerBackend_Metal = 1,
	JPH_ViewerBackend_Vulkan = 2,
} JPH_ViewerBackend;

typedef struct JPH_ViewerSettings {
	const char* title;
	JPH_ViewerBackend backend;
	int width;
	int height;
	JPH_RVec3 cameraPosition;
	JPH_RVec3 cameraTarget;
	float worldScale;
} JPH_ViewerSettings;

typedef struct JPH_ViewerStats {
	uint32_t vertexCount;
	uint32_t lineCount;
	uint32_t triangleCount;
	uint32_t textCount;
} JPH_ViewerStats;

typedef bool (*JPH_ViewerFrameCallback)(JPH_Viewer* viewer, float deltaTime, void* userData);

JPH_CAPI void JPH_ViewerSettings_InitDefault(JPH_ViewerSettings* settings);
JPH_CAPI JPH_Viewer* JPH_Viewer_CreateObj(const char* path);
JPH_CAPI JPH_Viewer* JPH_Viewer_CreateWindowed(const JPH_ViewerSettings* settings);
JPH_CAPI JPH_Viewer* JPH_Viewer_CreateMetal(const JPH_ViewerSettings* settings);
JPH_CAPI JPH_Viewer* JPH_Viewer_CreateVulkan(const JPH_ViewerSettings* settings);
JPH_CAPI void JPH_Viewer_Destroy(JPH_Viewer* viewer);

JPH_CAPI JPH_DebugRenderer* JPH_Viewer_GetDebugRenderer(JPH_Viewer* viewer);
JPH_CAPI void JPH_Viewer_SetCameraPosition(JPH_Viewer* viewer, const JPH_RVec3* position);
JPH_CAPI void JPH_Viewer_SetCameraLookAt(JPH_Viewer* viewer, const JPH_RVec3* position, const JPH_RVec3* target);
JPH_CAPI void JPH_Viewer_Clear(JPH_Viewer* viewer);
JPH_CAPI bool JPH_Viewer_RenderFrame(JPH_Viewer* viewer, float deltaTime);
JPH_CAPI void JPH_Viewer_Run(JPH_Viewer* viewer, JPH_ViewerFrameCallback callback, void* userData);
JPH_CAPI void JPH_Viewer_NextFrame(JPH_Viewer* viewer);
JPH_CAPI bool JPH_Viewer_Flush(JPH_Viewer* viewer);
JPH_CAPI void JPH_Viewer_GetStats(const JPH_Viewer* viewer, JPH_ViewerStats* stats);

JPH_CAPI void JPH_Viewer_DrawBodies(JPH_Viewer* viewer, JPH_PhysicsSystem* system, const JPH_DrawSettings* settings, const JPH_BodyDrawFilter* bodyFilter);
JPH_CAPI void JPH_Viewer_DrawConstraints(JPH_Viewer* viewer, JPH_PhysicsSystem* system);
JPH_CAPI void JPH_Viewer_DrawConstraintLimits(JPH_Viewer* viewer, JPH_PhysicsSystem* system);
JPH_CAPI void JPH_Viewer_DrawConstraintReferenceFrame(JPH_Viewer* viewer, JPH_PhysicsSystem* system);

#ifdef __cplusplus
}
#endif

#endif /* JOLT_VIEWER_C_H_ */
