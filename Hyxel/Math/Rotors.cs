using System;

namespace Hyxel.Math
{
    /// <summary>
    /// Generates rotation matrices for the 6 principal planes of 4D space.
    /// In 3D, we rotate around an axis (X, Y, Z).
    /// In 4D, we rotate around a PLANE (leaving 2 other axes invariant).
    /// </summary>
    public static class Rotors
    {
        // 1. Rotation in XY Plane (Z, W invariant)
        public static Matrix5 RotationXY(float angle)
        {
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            return new Matrix5(new float[,] {
                { c, -s, 0, 0, 0 },
                { s,  c, 0, 0, 0 },
                { 0,  0, 1, 0, 0 },
                { 0,  0, 0, 1, 0 },
                { 0,  0, 0, 0, 1 }
            });
        }

        // 2. Rotation in YZ Plane (X, W invariant)
        public static Matrix5 RotationYZ(float angle)
        {
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            return new Matrix5(new float[,] {
                { 1, 0, 0, 0, 0 },
                { 0, c, -s, 0, 0 },
                { 0, s,  c, 0, 0 },
                { 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 1 }
            });
        }

        // 3. Rotation in ZX Plane (Y, W invariant) -> often called XZ
        public static Matrix5 RotationZX(float angle)
        {
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            return new Matrix5(new float[,] {
                { c, 0, s, 0, 0 },
                { 0, 1, 0, 0, 0 },
                {-s, 0, c, 0, 0 },
                { 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 1 }
            });
        }

        // 4. Rotation in XW Plane (Y, Z invariant)
        public static Matrix5 RotationXW(float angle)
        {
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            return new Matrix5(new float[,] {
                { c, 0, 0, -s, 0 },
                { 0, 1, 0, 0,  0 },
                { 0, 0, 1, 0,  0 },
                { s, 0, 0, c,  0 },
                { 0, 0, 0, 0,  1 }
            });
        }

        // 5. Rotation in YW Plane (X, Z invariant)
        public static Matrix5 RotationYW(float angle)
        {
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            return new Matrix5(new float[,] {
                { 1, 0, 0, 0,  0 },
                { 0, c, 0, -s, 0 },
                { 0, 0, 1, 0,  0 },
                { 0, s, 0, c,  0 },
                { 0, 0, 0, 0,  1 }
            });
        }

        // 6. Rotation in ZW Plane (X, Y invariant)
        public static Matrix5 RotationZW(float angle)
        {
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            return new Matrix5(new float[,] {
                { 1, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 0, 0, c, -s, 0 },
                { 0, 0, s, c,  0 },
                { 0, 0, 0, 0,  1 }
            });
        }
    }
}
