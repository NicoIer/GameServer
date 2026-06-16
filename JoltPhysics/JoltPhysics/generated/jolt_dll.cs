// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace JoltPhysics
{
    internal static class jolt_dll
    {
#if !UNITY_IOS || UNITY_EDITOR
        public const string DLL_NAME = "libjoltc";
#else
        public const string DLL_NAME = "__Internal";
#endif
    }
}