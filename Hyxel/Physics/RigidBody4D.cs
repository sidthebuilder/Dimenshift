using System.Runtime.CompilerServices;
using Hyxel.Math;

namespace Hyxel.Physics
{
    public class RigidBody4D
    {
        public Vector4 Velocity;
        public Vector4 Acceleration;
        public float Mass = 1.0f;
        public float Drag = 0.05f;
        public bool IsStatic = false;
        public float Bounciness = 0.7f; // 0 = no bounce, 1 = perfectly elastic

        public RigidBody4D()
        {
            Velocity = Vector4.Zero;
            Acceleration = Vector4.Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddForce(Vector4 force)
        {
            if (IsStatic) return;
            // F = ma -> a = F/m
            Acceleration += force * (1.0f / Mass);
        }

        public void Stop()
        {
            Velocity = Vector4.Zero;
            Acceleration = Vector4.Zero;
        }
    }
}
