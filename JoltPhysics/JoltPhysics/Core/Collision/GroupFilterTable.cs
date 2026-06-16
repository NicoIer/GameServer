// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    /// <summary>
    /// 基于表的 GroupFilter 实现，使用一个 NxN 矩阵来控制 SubGroup 之间的碰撞。
    /// 封装 Jolt 的 JPH_GroupFilterTable。
    /// </summary>
    public sealed class GroupFilterTable : GroupFilter
    {
        public GroupFilterTable(uint numSubGroups)
            : base(CreateNative(numSubGroups), true)
        {
        }

        private static IntPtr CreateNative(uint numSubGroups)
        {
            unsafe
            {
                return (IntPtr)Methods.JPH_GroupFilterTable_Create(numSubGroups);
            }
        }

        public void DisableCollision(uint subGroup1, uint subGroup2)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_GroupFilterTable_DisableCollision(
                    (JPH_GroupFilterTable*)Handle, subGroup1, subGroup2);
            }
        }

        public void EnableCollision(uint subGroup1, uint subGroup2)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_GroupFilterTable_EnableCollision(
                    (JPH_GroupFilterTable*)Handle, subGroup1, subGroup2);
            }
        }

        public bool IsCollisionEnabled(uint subGroup1, uint subGroup2)
        {
            ThrowIfDisposed();
            unsafe
            {
                return Methods.JPH_GroupFilterTable_IsCollisionEnabled(
                    (JPH_GroupFilterTable*)Handle, subGroup1, subGroup2) != 0;
            }
        }
    }
}
