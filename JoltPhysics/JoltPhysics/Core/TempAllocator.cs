// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class TempAllocator : NativeHandle
    {
        /// <summary>
        /// Creates a TempAllocator with the specified size in bytes.
        /// Each PhysicsSystem should have its own TempAllocator for thread safety.
        /// </summary>
        /// <param name="size">Size in bytes (e.g. 10 * 1024 * 1024 for 10MB)</param>
        public TempAllocator(uint size = 10 * 1024 * 1024)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_TempAllocator_Create(size);
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_TempAllocator_Destroy((JPH_TempAllocator*)Handle);
            }
        }
    }
}