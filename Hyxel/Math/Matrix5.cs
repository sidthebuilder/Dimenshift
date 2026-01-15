using System;
using System.Runtime.CompilerServices;

namespace Hyxel.Math
{
    /// <summary>
    /// A 5x5 Matrix for handling 4D affine transformations.
    /// Optimized for memory alignment and performance.
    /// </summary>
    public struct Matrix5
    {
        // Using a flat array can sometimes be faster for stack allocation, but multi-dim is clearer.
        // For 'portfolio quality' we keep multi-dim for readability unless profiling demands unsafe pointers.
        public float[,] M; 

        public float this[int row, int col]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => M[row, col];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => M[row, col] = value;
        }

        public Matrix5(float[,] values)
        {
            if (values.GetLength(0) != 5 || values.GetLength(1) != 5)
                throw new ArgumentException("Matrix must be 5x5");
            M = values;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5 Identity()
        {
            return new Matrix5(new float[,] {
                { 1, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 1 }
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5 Translation(float x, float y, float z, float w)
        {
            var mat = Identity();
            mat.M[0, 4] = x;
            mat.M[1, 4] = y;
            mat.M[2, 4] = z;
            mat.M[3, 4] = w;
            return mat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5 Translation(in Vector4 v) => Translation(v.X, v.Y, v.Z, v.W);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5 Scale(float s)
        {
            return new Matrix5(new float[,] {
                { s, 0, 0, 0, 0 },
                { 0, s, 0, 0, 0 },
                { 0, 0, s, 0, 0 },
                { 0, 0, 0, s, 0 },
                { 0, 0, 0, 0, 1 }
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5 Scale(float x, float y, float z, float w)
        {
            return new Matrix5(new float[,] {
                { x, 0, 0, 0, 0 },
                { 0, y, 0, 0, 0 },
                { 0, 0, z, 0, 0 },
                { 0, 0, 0, w, 0 },
                { 0, 0, 0, 0, 1 }
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Multiply(in Matrix5 mat, in Vector4 v)
        {
            return new Vector4(
                mat.M[0,0]*v.X + mat.M[0,1]*v.Y + mat.M[0,2]*v.Z + mat.M[0,3]*v.W + mat.M[0,4],
                mat.M[1,0]*v.X + mat.M[1,1]*v.Y + mat.M[1,2]*v.Z + mat.M[1,3]*v.W + mat.M[1,4],
                mat.M[2,0]*v.X + mat.M[2,1]*v.Y + mat.M[2,2]*v.Z + mat.M[2,3]*v.W + mat.M[2,4],
                mat.M[3,0]*v.X + mat.M[3,1]*v.Y + mat.M[3,2]*v.Z + mat.M[3,3]*v.W + mat.M[3,4]
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix5 operator *(in Matrix5 A, in Matrix5 B)
        {
            float[,] result = new float[5, 5];
            // Unrolling loops for specific 5x5 optimization would be the next step, but compiler does a decent job here.
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    float sum = 0;
                    for (int k = 0; k < 5; k++)
                        sum += A.M[r, k] * B.M[k, c];
                    result[r, c] = sum;
                }
            }
            return new Matrix5(result);
        }

        public override string ToString()
        {
           return "Matrix5x5"; 
        }
    }
}
