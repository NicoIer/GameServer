// Copyright (c) 2025 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace zstd
{
    internal static class zstd_dll
    {
#if UNITY_IOS && !UNITY_EDITOR
        public const string ZSTD_DLL_NAME = "__Internal";
#else
        public const string ZSTD_DLL_NAME = "libzstd";
#endif
    }
}
