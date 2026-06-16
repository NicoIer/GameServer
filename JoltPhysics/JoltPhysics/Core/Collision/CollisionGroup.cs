// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    /// <summary>
    /// 碰撞分组，用于控制哪些 body 之间可以碰撞。
    /// groupFilter 用于细粒度的 SubGroup 级别过滤。
    /// </summary>
    public struct CollisionGroup
    {
        public GroupFilter? groupFilter;
        public uint groupID;
        public uint subGroupID;

        public CollisionGroup(uint groupID, uint subGroupID)
        {
            this.groupFilter = null;
            this.groupID = groupID;
            this.subGroupID = subGroupID;
        }

        public CollisionGroup(GroupFilter groupFilter, uint groupID, uint subGroupID)
        {
            this.groupFilter = groupFilter;
            this.groupID = groupID;
            this.subGroupID = subGroupID;
        }

        internal unsafe void ToNative(out JPH_CollisionGroup result)
        {
            result = new JPH_CollisionGroup
            {
                groupFilter = (groupFilter != null && !groupFilter.IsDisposed)
                    ? (JPH_GroupFilter*)groupFilter.Handle
                    : null,
                groupID = groupID,
                subGroupID = subGroupID,
            };
        }

        internal static unsafe CollisionGroup FromNative(in JPH_CollisionGroup native)
        {
            // 从 native 回读时不恢复 GroupFilter 托管引用（Jolt 不提供反查机制）
            return new CollisionGroup
            {
                groupFilter = null,
                groupID = native.groupID,
                subGroupID = native.subGroupID,
            };
        }
    }
}
