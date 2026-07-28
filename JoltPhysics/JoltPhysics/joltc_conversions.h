// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

#pragma once

#include "joltc.h"

#include <Jolt/Jolt.h>
#include <Jolt/Geometry/AABox.h>
#include <Jolt/Math/Mat44.h>

// Shared internal math conversion helpers.
// Keep this header limited to plain value types used by multiple translation units.

static inline JPH_Vec3 FromJolt(const JPH::Vec3& vec)
{
	return { vec.GetX(), vec.GetY(), vec.GetZ() };
}

static inline JPH_Vec4 FromJolt(const JPH::Vec4& vec)
{
	return { vec.GetX(), vec.GetY(), vec.GetZ(), vec.GetW() };
}

static inline void FromJolt(const JPH::Vec3& vec, JPH_Vec3* result)
{
	result->x = vec.GetX();
	result->y = vec.GetY();
	result->z = vec.GetZ();
}

static inline void FromJolt(const JPH::Float3& vec, JPH_Vec3* result)
{
	result->x = vec.x;
	result->y = vec.y;
	result->z = vec.z;
}

static inline void FromJolt(const JPH::Quat& quat, JPH_Quat* result)
{
	result->x = quat.GetX();
	result->y = quat.GetY();
	result->z = quat.GetZ();
	result->w = quat.GetW();
}

static inline void FromJolt(const JPH::AABox& value, JPH_AABox* result)
{
	FromJolt(value.mMin, &result->min);
	FromJolt(value.mMax, &result->max);
}

#if defined(JPH_DOUBLE_PRECISION)
static inline void FromJolt(const JPH::RVec3& vec, JPH_RVec3* result)
{
	result->x = vec.GetX();
	result->y = vec.GetY();
	result->z = vec.GetZ();
}

static inline JPH_RVec3 FromJolt(const JPH::RVec3& vec)
{
	return { vec.GetX(), vec.GetY(), vec.GetZ() };
}
#endif /* defined(JPH_DOUBLE_PRECISION) */

static inline JPH::Vec3 ToJolt(const JPH_Vec3& vec)
{
	return JPH::Vec3(vec.x, vec.y, vec.z);
}

static inline JPH::Vec3 ToJolt(const JPH_Vec3* vec)
{
	return JPH::Vec3(vec->x, vec->y, vec->z);
}

static inline JPH::Vec4 ToJolt(const JPH_Vec4& vec)
{
	return JPH::Vec4(vec.x, vec.y, vec.z, vec.w);
}

static inline JPH::Quat ToJolt(const JPH_Quat* quat)
{
	return JPH::Quat(quat->x, quat->y, quat->z, quat->w);
}

static inline JPH::Quat ToJolt(const JPH_Quat& quat)
{
	return ToJolt(&quat);
}

static inline JPH::Float3 ToJoltFloat3(const JPH_Vec3& vec)
{
	return JPH::Float3(vec.x, vec.y, vec.z);
}

static inline JPH::AABox ToJolt(const JPH_AABox* value)
{
	return JPH::AABox(ToJolt(value->min), ToJolt(value->max));
}

static inline JPH::AABox ToJolt(const JPH_AABox& value)
{
	return ToJolt(&value);
}

#if defined(JPH_DOUBLE_PRECISION)
static inline JPH::RVec3 ToJolt(const JPH_RVec3& vec)
{
	return JPH::RVec3(vec.x, vec.y, vec.z);
}

static inline JPH::RVec3 ToJolt(const JPH_RVec3* vec)
{
	return JPH::RVec3(vec->x, vec->y, vec->z);
}
#endif /* defined(JPH_DOUBLE_PRECISION) */
