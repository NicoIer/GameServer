// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    /// <summary>
    /// BodyLock 读锁 RAII 包装。通过 using 自动释放锁。
    /// 持有期间可通过 Body 属性访问 JPH_Body 的只读操作。
    /// </summary>
    public unsafe struct BodyLockRead : IDisposable
    {
        private JPH_BodyLockRead _native;
        private readonly JPH_BodyLockInterface* _lockInterface;
        private bool _released;

        internal BodyLockRead(JPH_BodyLockInterface* lockInterface, uint bodyID)
        {
            _lockInterface = lockInterface;
            _released = false;
            fixed (JPH_BodyLockRead* ptr = &_native)
            {
                Methods.JPH_BodyLockInterface_LockRead(lockInterface, bodyID, ptr);
            }
        }

        /// <summary>
        /// 锁定的 Body 指针。若 body 已被销毁则为 null。
        /// </summary>
        internal JPH_Body* BodyPtr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _native.body;
        }

        /// <summary>
        /// 获取锁定的 Body 包装。仅在 using 作用域内有效。
        /// </summary>
        public Body Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Body(_native.body);
        }

        /// <summary>
        /// Body 是否有效（非 null）。
        /// </summary>
        public bool Succeeded
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _native.body != null;
        }

        public void Dispose()
        {
            if (_released) return;
            _released = true;
            fixed (JPH_BodyLockRead* ptr = &_native)
            {
                Methods.JPH_BodyLockInterface_UnlockRead(_lockInterface, ptr);
            }
        }
    }

    /// <summary>
    /// BodyLock 写锁 RAII 包装。通过 using 自动释放锁。
    /// 持有期间可通过 Body 属性访问 JPH_Body 的读写操作。
    /// </summary>
    public unsafe struct BodyLockWrite : IDisposable
    {
        private JPH_BodyLockWrite _native;
        private readonly JPH_BodyLockInterface* _lockInterface;
        private bool _released;

        internal BodyLockWrite(JPH_BodyLockInterface* lockInterface, uint bodyID)
        {
            _lockInterface = lockInterface;
            _released = false;
            fixed (JPH_BodyLockWrite* ptr = &_native)
            {
                Methods.JPH_BodyLockInterface_LockWrite(lockInterface, bodyID, ptr);
            }
        }

        /// <summary>
        /// 锁定的 Body 指针。若 body 已被销毁则为 null。
        /// </summary>
        internal JPH_Body* BodyPtr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _native.body;
        }

        /// <summary>
        /// 获取锁定的 Body 包装。仅在 using 作用域内有效。
        /// </summary>
        public Body Body
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Body(_native.body);
        }

        /// <summary>
        /// Body 是否有效（非 null）。
        /// </summary>
        public bool Succeeded
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _native.body != null;
        }

        public void Dispose()
        {
            if (_released) return;
            _released = true;
            fixed (JPH_BodyLockWrite* ptr = &_native)
            {
                Methods.JPH_BodyLockInterface_UnlockWrite(_lockInterface, ptr);
            }
        }
    }

    /// <summary>
    /// Jolt BodyLockInterface 封装。提供对 Body 的线程安全读写锁访问。
    /// 通过 PhysicsSystem.GetBodyLockInterface() 获取。
    /// </summary>
    public readonly struct BodyLockInterface
    {
        internal readonly IntPtr Handle;

        internal BodyLockInterface(IntPtr handle)
        {
            Handle = handle;
        }

        private unsafe JPH_BodyLockInterface* Ptr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (JPH_BodyLockInterface*)Handle;
        }

        /// <summary>
        /// 获取指定 body 的读锁。使用 using 模式确保释放。
        /// </summary>
        public BodyLockRead LockRead(BodyID bodyID)
        {
            unsafe { return new BodyLockRead(Ptr, bodyID.Value); }
        }

        /// <summary>
        /// 获取指定 body 的写锁。使用 using 模式确保释放。
        /// </summary>
        public BodyLockWrite LockWrite(BodyID bodyID)
        {
            unsafe { return new BodyLockWrite(Ptr, bodyID.Value); }
        }

        #region Convenience Read-Only Accessors

        /// <summary>
        /// 获取 body 的 BroadPhaseLayer（只读，由 ObjectLayer 映射得到）。
        /// </summary>
        public BroadPhaseLayer GetBroadPhaseLayer(BodyID bodyID)
        {
            unsafe
            {
                using var lockRead = LockRead(bodyID);
                if (!lockRead.Succeeded) return new BroadPhaseLayer(0);
                return new BroadPhaseLayer(Methods.JPH_Body_GetBroadPhaseLayer(lockRead.BodyPtr));
            }
        }

        /// <summary>
        /// 获取 body 的 AllowSleeping 状态。
        /// </summary>
        public bool GetAllowSleeping(BodyID bodyID)
        {
            unsafe
            {
                using var lockRead = LockRead(bodyID);
                if (!lockRead.Succeeded) return true;
                return Methods.JPH_Body_GetAllowSleeping(lockRead.BodyPtr) != 0;
            }
        }

        /// <summary>
        /// 设置 body 的 AllowSleeping 状态。
        /// </summary>
        public void SetAllowSleeping(BodyID bodyID, bool allowSleeping)
        {
            unsafe
            {
                using var lockWrite = LockWrite(bodyID);
                if (!lockWrite.Succeeded) return;
                Methods.JPH_Body_SetAllowSleeping(lockWrite.BodyPtr, (byte)(allowSleeping ? 1 : 0));
            }
        }

        #endregion
    }
}