using Hyxel.Math;

namespace Hyxel.Scenegraph
{
    public class Transform4D
    {
        public Vector4 Position;
        public Vector4 Scale;
        // In 4D, rotation is complex. We'll store it as a Matrix5 for now.
        // A more advanced system might use 4D Rotors (Clifford Algebra) or double-quaternions.
        public Matrix5 Rotation; 

        public Transform4D()
        {
            Position = Vector4.Zero;
            Scale = Vector4.One;
            Rotation = Matrix5.Identity();
        }

        public Matrix5 GetMatrix()
        {
            // T * R * S
            Matrix5 matT = Matrix5.Translation(Position);
            Matrix5 matS = Matrix5.Scale(Scale.X, Scale.Y, Scale.Z, Scale.W);
            
            // Optimization: If we had cached matrices, we'd use them.
            // Order: Scale, then Rotate, then Translate
            return matT * (Rotation * matS);
        }
    }
}
