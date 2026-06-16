// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public abstract class NativeHandle : IDisposable
    {
        internal IntPtr Handle;
        internal bool OwnsHandle;
        private bool _disposed;

        internal NativeHandle(IntPtr handle, bool ownsHandle)
        {
            Handle = handle;
            OwnsHandle = ownsHandle;
        }

        protected abstract void DestroyNative();

        public bool IsDisposed => _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (OwnsHandle && Handle != IntPtr.Zero)
            {
                DestroyNative();
            }

            Handle = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        ~NativeHandle()
        {
            if (!_disposed && OwnsHandle && Handle != IntPtr.Zero)
            {
                DestroyNative();
                Handle = IntPtr.Zero;
            }

            _disposed = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}