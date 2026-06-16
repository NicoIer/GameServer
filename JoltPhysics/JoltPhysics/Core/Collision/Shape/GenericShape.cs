// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{
    /// <summary>
    /// A concrete Shape wrapper used when the specific shape subtype is unknown
    /// (e.g., returned from JPH_Body_GetShape).
    /// </summary>
    internal sealed class GenericShape: Shape
    {
        internal GenericShape(IntPtr handle, bool ownsHandle): base(handle, ownsHandle)
        {
        }
    }
}