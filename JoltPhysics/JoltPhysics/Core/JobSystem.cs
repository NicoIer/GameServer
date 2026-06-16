// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class JobSystemThreadPool : NativeHandle
    {
        public JobSystemThreadPool(int maxJobs = 2048, int maxBarriers = 8, int numThreads = -1)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                var config = new JobSystemThreadPoolConfig
                {
                    maxJobs = (uint)maxJobs,
                    maxBarriers = (uint)maxBarriers,
                    numThreads = numThreads,
                };
                Handle = (IntPtr)Methods.JPH_JobSystemThreadPool_Create(&config);
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_JobSystem_Destroy((JPH_JobSystem*)Handle);
            }
        }
    }
}
