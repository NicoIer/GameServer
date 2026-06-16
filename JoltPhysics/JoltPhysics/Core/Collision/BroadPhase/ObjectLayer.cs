// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{
    /// <summary>
    /// Jolt ObjectLayer 封装（底层 uint）。用于标识 body 所属的碰撞层。
    /// </summary>
    public readonly struct ObjectLayer: IEquatable<ObjectLayer>
    {
        public readonly uint Value;

        public ObjectLayer(uint value) => Value = value;

        public bool Equals(ObjectLayer other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ObjectLayer other && Equals(other);
        public override int GetHashCode() => (int)Value;
        public override string ToString() => $"ObjectLayer({Value})";

        public static bool operator ==(ObjectLayer left, ObjectLayer right) => left.Value == right.Value;
        public static bool operator !=(ObjectLayer left, ObjectLayer right) => left.Value != right.Value;

        public static implicit operator ObjectLayer(uint value) => new ObjectLayer(value);
        public static implicit operator uint(ObjectLayer layer) => layer.Value;
        public static implicit operator ObjectLayer(int value) => new ObjectLayer((uint)value);
        public static implicit operator int(ObjectLayer layer) => (int)layer.Value;
    }
}