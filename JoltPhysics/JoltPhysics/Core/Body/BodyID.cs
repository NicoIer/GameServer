// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public readonly struct BodyID : IEquatable<BodyID>
    {
        public static readonly BodyID Invalid = new(0xffffffff);

        public readonly uint Value;

        public BodyID(uint value)
        {
            Value = value;
        }

        public bool IsValid => Value != 0xffffffff;

        public uint Index => Value & 0x00FFFFFF;

        public byte SequenceNumber => (byte)(Value >> 24);

        public bool Equals(BodyID other) => Value == other.Value;
        public override bool Equals(object obj) => obj is BodyID other && Equals(other);
        public override int GetHashCode() => (int)Value;
        public override string ToString() => $"BodyID({Value})";

        public static bool operator ==(BodyID left, BodyID right) => left.Value == right.Value;
        public static bool operator !=(BodyID left, BodyID right) => left.Value != right.Value;
    }
}