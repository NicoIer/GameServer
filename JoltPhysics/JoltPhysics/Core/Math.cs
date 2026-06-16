// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace JoltPhysics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Float3: IEquatable<Float3>
    {
        public float x;
        public float y;
        public float z;

        public static readonly Float3 Zero = new(0f, 0f, 0f);
        public static readonly Float3 One = new(1f, 1f, 1f);
        public static readonly Float3 Up = new(0f, 1f, 0f);
        public static readonly Float3 Down = new(0f, -1f, 0f);

        public Float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(Float3 other) => x == other.x && y == other.y && z == other.z;
        public override bool Equals(object obj) => obj is Float3 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(x, y, z);
        public override string ToString() => $"Float3({x}, {y}, {z})";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Float3 left, Float3 right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Float3 left, Float3 right) => !left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator +(Float3 a, Float3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator -(Float3 a, Float3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(Float3 a, float s) => new(a.x * s, a.y * s, a.z * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(float s, Float3 a) => new(a.x * s, a.y * s, a.z * s);

#if UNITY_5_3_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Float3(UnityEngine.Vector3 v) => Unsafe.As<UnityEngine.Vector3, Float3>(ref v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UnityEngine.Vector3(Float3 f) => Unsafe.As<Float3, UnityEngine.Vector3>(ref f);
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Float3(System.Numerics.Vector3 v) => Unsafe.As<System.Numerics.Vector3, Float3>(ref v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Numerics.Vector3(Float3 f) => Unsafe.As<Float3, System.Numerics.Vector3>(ref f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 operator *(Float3 a, System.Numerics.Vector3 b) => new(a.x * b.X, a.y * b.Y, a.z * b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 operator *(System.Numerics.Vector3 a, Float3 b) => new(a.X * b.x, a.Y * b.y, a.Z * b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 operator /(Float3 a, System.Numerics.Vector3 b) => new(a.x / b.X, a.y / b.Y, a.z / b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 operator /(System.Numerics.Vector3 a, Float3 b) => new(a.X / b.x, a.Y / b.y, a.Z / b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 operator +(Float3 a, System.Numerics.Vector3 b) => new(a.x + b.X, a.y + b.Y, a.z + b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 operator +(System.Numerics.Vector3 a, Float3 b) => new(a.X + b.x, a.Y + b.y, a.Z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 operator -(Float3 a, System.Numerics.Vector3 b) => new(a.x - b.X, a.y - b.Y, a.z - b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector3 operator -(System.Numerics.Vector3 a, Float3 b) => new(a.X - b.x, a.Y - b.y, a.Z - b.z);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Float4: IEquatable<Float4>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Float4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override string ToString() => $"Float4({x}, {y}, {z}, {w})";

        public bool Equals(Float4 other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
        }

        public override bool Equals(object? obj)
        {
            return obj is Float4 other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.x.GetHashCode();
                hashCode = (hashCode * 397) ^ this.y.GetHashCode();
                hashCode = (hashCode * 397) ^ this.z.GetHashCode();
                hashCode = (hashCode * 397) ^ this.w.GetHashCode();
                return hashCode;
            }
        }
    }

    /// <summary>
    /// 4x4 column-major matrix. Layout matches JPH_Mat4 (4 columns of JPH_Vec4).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Mat4: IEquatable<Mat4>
    {
        public Float4 Column0;
        public Float4 Column1;
        public Float4 Column2;
        public Float4 Column3;

        public static readonly Mat4 Identity = new Mat4
        {
            Column0 = new Float4(1, 0, 0, 0), Column1 = new Float4(0, 1, 0, 0), Column2 = new Float4(0, 0, 1, 0), Column3 = new Float4(0, 0, 0, 1),
        };

        public override string ToString() => $"Mat4(\n  {Column0},\n  {Column1},\n  {Column2},\n  {Column3})";

        public bool Equals(Mat4 other)
        {
            return this.Column0.Equals(other.Column0) && this.Column1.Equals(other.Column1) && this.Column2.Equals(other.Column2) &&
                    this.Column3.Equals(other.Column3);
        }

        public override bool Equals(object? obj)
        {
            return obj is Mat4 other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Column0.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Column1.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Column2.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Column3.GetHashCode();
                return hashCode;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Quat: IEquatable<Quat>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static readonly Quat Identity = new(0f, 0f, 0f, 1f);

        public Quat(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool Equals(Quat other) => x == other.x && y == other.y && z == other.z && w == other.w;
        public override bool Equals(object obj) => obj is Quat other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(x, y, z, w);
        public override string ToString() => $"Quat({x}, {y}, {z}, {w})";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Quat left, Quat right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Quat left, Quat right) => !left.Equals(right);

#if UNITY_5_3_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Quat(UnityEngine.Quaternion q) => Unsafe.As<UnityEngine.Quaternion, Quat>(ref q);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UnityEngine.Quaternion(Quat q) => Unsafe.As<Quat, UnityEngine.Quaternion>(ref q);
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Quat(System.Numerics.Quaternion q) => Unsafe.As<System.Numerics.Quaternion, Quat>(ref q);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator System.Numerics.Quaternion(Quat q) => Unsafe.As<Quat, System.Numerics.Quaternion>(ref q);
    }
}