using System.Runtime.CompilerServices;
using Hyxel.Math;

namespace Hyxel.Physics
{
    public struct Ray4D
    {
        public Vector4 Origin;
        public Vector4 Direction;

        public Ray4D(Vector4 origin, Vector4 direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 GetPoint(float distance)
        {
            return Origin + Direction * distance;
        }

        // Slab method for AABB Intesection (extended to 4D)
        public bool Intersects(in AABB4D box, out float tMin, out float tMax)
        {
            tMin = 0.0f;
            tMax = float.MaxValue;

            // X Axis
            if (!TestAxis(Direction.X, Origin.X, box.Min.X, box.Max.X, ref tMin, ref tMax)) return false;
            // Y Axis
            if (!TestAxis(Direction.Y, Origin.Y, box.Min.Y, box.Max.Y, ref tMin, ref tMax)) return false;
            // Z Axis
            if (!TestAxis(Direction.Z, Origin.Z, box.Min.Z, box.Max.Z, ref tMin, ref tMax)) return false;
            // W Axis
            if (!TestAxis(Direction.W, Origin.W, box.Min.W, box.Max.W, ref tMin, ref tMax)) return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TestAxis(float dir, float origin, float min, float max, ref float tMin, ref float tMax)
        {
            if (System.Math.Abs(dir) < 1e-6f)
            {
                // Ray is parallel to slab. No hit if origin not within slab
                if (origin < min || origin > max) return false;
            }
            else
            {
                float t1 = (min - origin) / dir;
                float t2 = (max - origin) / dir;

                if (t1 > t2) { float temp = t1; t1 = t2; t2 = temp; }

                if (t1 > tMin) tMin = t1;
                if (t2 < tMax) tMax = t2;

                if (tMin > tMax) return false;
                if (tMax < 0) return false;
            }
            return true;
        }
    }
}
