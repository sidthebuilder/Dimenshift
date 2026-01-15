using System.Runtime.CompilerServices;
using Hyxel.Math;

namespace Hyxel.Physics
{
    public struct AABB4D
    {
        public Vector4 Min;
        public Vector4 Max;

        public AABB4D(Vector4 min, Vector4 max)
        {
            Min = min;
            Max = max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in AABB4D other)
        {
            // Check for overlap on all 4 axes
            return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
                   (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
                   (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z) &&
                   (Min.W <= other.Max.W && Max.W >= other.Min.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Vector4 point)
        {
            return (point.X >= Min.X && point.X <= Max.X) &&
                   (point.Y >= Min.Y && point.Y <= Max.Y) &&
                   (point.Z >= Min.Z && point.Z <= Max.Z) &&
                   (point.W >= Min.W && point.W <= Max.W);
        }

        public static AABB4D FromCenterSize(Vector4 center, Vector4 size)
        {
            Vector4 half = size * 0.5f;
            return new AABB4D(center - half, center + half);
        }
    }
}
