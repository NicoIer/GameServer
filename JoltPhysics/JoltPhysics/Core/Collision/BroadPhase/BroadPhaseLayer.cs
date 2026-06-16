// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{
    /// <summary>
    /// Jolt BroadPhaseLayer 封装（底层 byte）。用于 BroadPhase 阶段的碰撞过滤。
    /// </summary>
    public readonly struct BroadPhaseLayer: IEquatable<BroadPhaseLayer>
    {
        public readonly byte Value;

        public BroadPhaseLayer(byte value) => Value = value;

        public bool Equals(BroadPhaseLayer other) => Value == other.Value;
        public override bool Equals(object obj) => obj is BroadPhaseLayer other && Equals(other);
        public override int GetHashCode() => Value;
        public override string ToString() => $"BroadPhaseLayer({Value})";

        public static bool operator ==(BroadPhaseLayer left, BroadPhaseLayer right) => left.Value == right.Value;
        public static bool operator !=(BroadPhaseLayer left, BroadPhaseLayer right) => left.Value != right.Value;

        public static implicit operator BroadPhaseLayer(byte value) => new BroadPhaseLayer(value);
        public static implicit operator byte(BroadPhaseLayer layer) => layer.Value;
    }
}