// Copyright (c)2026 NicoIer@163.com All Rights Reserved.
using System;
using System.Runtime.InteropServices;
#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace JoltPhysics
{
    public sealed class ContactListener : NativeHandle
    {
        public delegate bool ContactValidateHandler(BodyID bodyA, BodyID bodyB);
        public delegate void ContactHandler(BodyID bodyA, BodyID bodyB);
        public delegate void ContactRemovedHandler(BodyID bodyA, BodyID bodyB);

        public ContactValidateHandler OnContactValidate;
        public ContactHandler OnContactAdded;
        public ContactHandler OnContactPersisted;
        public ContactRemovedHandler OnContactRemoved;

        private GCHandle _gcHandle;

        // 必须是静态字段，防止被 GC 回收或栈帧销毁后指针悬空
        private static readonly JPH_ContactListener_Procs s_procs;

#if !NET5_0_OR_GREATER
        // 静态委托实例，防止被 GC 回收（仅非 CoreCLR 环境需要）
        private unsafe delegate JPH_ValidateResult ContactValidateNativeDelegate(
            void* userData, JPH_Body* body1, JPH_Body* body2,
            JPH_Vec3* baseOffset, JPH_CollideShapeResult* collisionResult);

        private unsafe delegate void ContactNativeDelegate(
            void* userData, JPH_Body* body1, JPH_Body* body2,
            JPH_ContactManifold* manifold, JPH_ContactSettings* settings);

        private unsafe delegate void ContactRemovedNativeDelegate(
            void* userData, JPH_SubShapeIDPair* subShapePair);

        private static readonly ContactValidateNativeDelegate s_validateDelegate;
        private static readonly ContactNativeDelegate s_addedDelegate;
        private static readonly ContactNativeDelegate s_persistedDelegate;
        private static readonly ContactRemovedNativeDelegate s_removedDelegate;
#endif

        static unsafe ContactListener()
        {
#if NET5_0_OR_GREATER
            s_procs = new JPH_ContactListener_Procs
            {
                OnContactValidate = &OnContactValidateCallback,
                OnContactAdded = &OnContactAddedCallback,
                OnContactPersisted = &OnContactPersistedCallback,
                OnContactRemoved = &OnContactRemovedCallback
            };
#else
            s_validateDelegate = OnContactValidateCallback;
            s_addedDelegate = OnContactAddedCallback;
            s_persistedDelegate = OnContactPersistedCallback;
            s_removedDelegate = OnContactRemovedCallback;

            s_procs = new JPH_ContactListener_Procs
            {
                OnContactValidate = (delegate* unmanaged[Cdecl]<void*, JPH_Body*, JPH_Body*, JPH_Vec3*, JPH_CollideShapeResult*, JPH_ValidateResult>)
                    Marshal.GetFunctionPointerForDelegate(s_validateDelegate),
                OnContactAdded = (delegate* unmanaged[Cdecl]<void*, JPH_Body*, JPH_Body*, JPH_ContactManifold*, JPH_ContactSettings*, void>)
                    Marshal.GetFunctionPointerForDelegate(s_addedDelegate),
                OnContactPersisted = (delegate* unmanaged[Cdecl]<void*, JPH_Body*, JPH_Body*, JPH_ContactManifold*, JPH_ContactSettings*, void>)
                    Marshal.GetFunctionPointerForDelegate(s_persistedDelegate),
                OnContactRemoved = (delegate* unmanaged[Cdecl]<void*, JPH_SubShapeIDPair*, void>)
                    Marshal.GetFunctionPointerForDelegate(s_removedDelegate),
            };
#endif
            fixed (JPH_ContactListener_Procs* ptr = &s_procs)
            {
                Methods.JPH_ContactListener_SetProcs(ptr);
            }
        }

        public ContactListener() : base(IntPtr.Zero, true)
        {
            _gcHandle = GCHandle.Alloc(this);

            unsafe
            {
                Handle = (IntPtr)Methods.JPH_ContactListener_Create(
                    (void*)GCHandle.ToIntPtr(_gcHandle));
            }
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe JPH_ValidateResult OnContactValidateCallback(
            void* userData, JPH_Body* body1, JPH_Body* body2,
            JPH_Vec3* baseOffset, JPH_CollideShapeResult* collisionResult)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is ContactListener listener && listener.OnContactValidate != null)
            {
                uint id1 = Methods.JPH_Body_GetID(body1);
                uint id2 = Methods.JPH_Body_GetID(body2);
                bool accept = listener.OnContactValidate(new BodyID(id1), new BodyID(id2));
                return accept
                    ? JPH_ValidateResult.JPH_ValidateResult_AcceptAllContactsForThisBodyPair
                    : JPH_ValidateResult.JPH_ValidateResult_RejectAllContactsForThisBodyPair;
            }

            return JPH_ValidateResult.JPH_ValidateResult_AcceptAllContactsForThisBodyPair;
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe void OnContactAddedCallback(
            void* userData, JPH_Body* body1, JPH_Body* body2,
            JPH_ContactManifold* manifold, JPH_ContactSettings* settings)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is ContactListener listener && listener.OnContactAdded != null)
            {
                uint id1 = Methods.JPH_Body_GetID(body1);
                uint id2 = Methods.JPH_Body_GetID(body2);
                listener.OnContactAdded(new BodyID(id1), new BodyID(id2));
            }
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe void OnContactPersistedCallback(
            void* userData, JPH_Body* body1, JPH_Body* body2,
            JPH_ContactManifold* manifold, JPH_ContactSettings* settings)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is ContactListener listener && listener.OnContactPersisted != null)
            {
                uint id1 = Methods.JPH_Body_GetID(body1);
                uint id2 = Methods.JPH_Body_GetID(body2);
                listener.OnContactPersisted(new BodyID(id1), new BodyID(id2));
            }
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe void OnContactRemovedCallback(
            void* userData, JPH_SubShapeIDPair* subShapePair)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is ContactListener listener && listener.OnContactRemoved != null)
            {
                listener.OnContactRemoved(
                    new BodyID(subShapePair->Body1ID),
                    new BodyID(subShapePair->Body2ID));
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_ContactListener_Destroy((JPH_ContactListener*)Handle);
            }

            if (_gcHandle.IsAllocated)
                _gcHandle.Free();
        }
    }
}
