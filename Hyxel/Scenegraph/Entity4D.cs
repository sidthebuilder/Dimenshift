using Hyxel.Graphics;
using Hyxel.Physics;

namespace Hyxel.Scenegraph
{
    public class Entity4D : Node4D
    {
        public Mesh4D Mesh { get; set; }
        public Vector4 Color { get; set; } = new Vector4(0, 1, 0, 1); // RGBA
        public RigidBody4D RigidBody { get; set; }

        public Entity4D(Mesh4D mesh, string name = "Entity") : base(name)
        {
            Mesh = mesh;
            RigidBody = null; // Optional
        }
    }
}
