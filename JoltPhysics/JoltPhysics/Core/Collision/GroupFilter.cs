// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    /// <summary>
    /// GroupFilter 基类，用于控制同一 CollisionGroup 内不同 SubGroup 之间的碰撞过滤。
    /// 封装 Jolt 的 JPH_GroupFilter，不暴露 Handle/IntPtr。
    /// </summary>
    public abstract class GroupFilter : NativeHandle
    {
        internal GroupFilter(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle)
        {
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_GroupFilter_Destroy((JPH_GroupFilter*)Handle);
            }
        }
    }
}
