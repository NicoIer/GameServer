// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace JoltPhysics
{
    public class NativeTypeNameAttribute : System.Attribute
    {
        public string Name { get; }

        public NativeTypeNameAttribute(string name)
        {
            Name = name;
        }
    }
    
    public class InlineArrayAttribute : System.Attribute
    {
        public int Length { get; }

        public InlineArrayAttribute(int length)
        {
            Length = length;
        }
    }
}