using System;
using System.Runtime.CompilerServices;

namespace Hyxel.Math
{
    /// <summary>
    /// Represents a 4-dimensional vector in Euclidean space (x, y, z, w).
    /// Optimized for high-performance graphics calculations.
    /// </summary>
    public struct Vector4 : IEquatable<Vector4>
    {
        public float X, Y, Z, W;

        public static readonly Vector4 Zero = new Vector4(0, 0, 0, 0);
        public static readonly Vector4 One = new Vector4(1, 1, 1, 1);
        public static readonly Vector4 UnitX = new Vector4(1, 0, 0, 0);
        public static readonly Vector4 UnitY = new Vector4(0, 1, 0, 0);
        public static readonly Vector4 UnitZ = new Vector4(0, 0, 1, 0);
        public static readonly Vector4 UnitW = new Vector4(0, 0, 0, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4(float x, float y, float z, float w)
        {
            X = x; Y = y; Z = z; W = w;
        }

        #region Arithmetic Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator +(Vector4 a, Vector4 b) => new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator -(Vector4 a, Vector4 b) => new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator -(Vector4 a) => new Vector4(-a.X, -a.Y, -a.Z, -a.W);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator *(Vector4 a, float s) => new Vector4(a.X * s, a.Y * s, a.Z * s, a.W * s);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator *(float s, Vector4 a) => a * s;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 operator /(Vector4 a, float s) => a * (1.0f / s);
        #endregion

        #region Vector Operations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float LengthSquared() => X*X + Y*Y + Z*Z + W*W;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Length() => (float)System.Math.Sqrt(LengthSquared());

        public Vector4 Normalized()
        {
            float len = Length();
            if (len < 1e-6f) return Zero;
            return this * (1.0f / len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(in Vector4 a, in Vector4 b)
        {
            return a.X*b.X + a.Y*b.Y + a.Z*b.Z + a.W*b.W;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(in Vector4 a, in Vector4 b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;
            float dw = a.W - b.W;
            return (float)System.Math.Sqrt(dx*dx + dy*dy + dz*dz + dw*dw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Lerp(in Vector4 a, in Vector4 b, float t)
        {
            t = (t < 0) ? 0 : (t > 1) ? 1 : t;
            return new Vector4(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Min(in Vector4 a, in Vector4 b) => new Vector4(System.Math.Min(a.X, b.X), System.Math.Min(a.Y, b.Y), System.Math.Min(a.Z, b.Z), System.Math.Min(a.W, b.W));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Max(in Vector4 a, in Vector4 b) => new Vector4(System.Math.Max(a.X, b.X), System.Math.Max(a.Y, b.Y), System.Math.Max(a.Z, b.Z), System.Math.Max(a.W, b.W));
        #endregion

        #region Utils
        public override string ToString() => $"({X:F2}, {Y:F2}, {Z:F2}, {W:F2})";
        
        public override bool Equals(object obj) => obj is Vector4 v && Equals(v);
        
        public bool Equals(Vector4 other) => this == other; 
        
        public override int GetHashCode() => (X, Y, Z, W).GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !(left == right);
        }
        #endregion
    }
}
