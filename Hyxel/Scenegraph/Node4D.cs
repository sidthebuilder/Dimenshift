using System.Collections.Generic;
using Hyxel.Math;

namespace Hyxel.Scenegraph
{
    public class Node4D
    {
        public string Name { get; set; }
        public Transform4D Transform { get; private set; }
        public Node4D Parent { get; private set; }
        public List<Node4D> Children { get; private set; }

        public Matrix5 WorldMatrix { get; private set; }
        public bool IsDirty { get; set; } = true;

        public Node4D(string name = "Node")
        {
            Name = name;
            Transform = new Transform4D();
            Children = new List<Node4D>();
            WorldMatrix = Matrix5.Identity();
        }

        public void AddChild(Node4D child)
        {
            child.Parent = this;
            Children.Add(child);
            child.IsDirty = true;
        }

        public virtual void Update(float deltaTime)
        {
            if (IsDirty)
            {
                UpdateWorldMatrix();
            }

            foreach (var child in Children)
            {
                child.Update(deltaTime);
            }
        }

        private void UpdateWorldMatrix()
        {
            Matrix5 local = Transform.GetMatrix();
            if (Parent != null)
            {
                WorldMatrix = Parent.WorldMatrix * local;
            }
            else
            {
                WorldMatrix = local;
            }
            IsDirty = false; // logic would need to be smarter in a real engine (check parent dirty)
        }
    }
}
