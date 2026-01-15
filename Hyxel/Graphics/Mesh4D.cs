using System.Collections.Generic;
using Hyxel.Math;

namespace Hyxel.Graphics
{
    public class Mesh4D
    {
        public List<Vector4> Vertices { get; private set; }
        public List<int[]> Edges { get; private set; }

        public Mesh4D()
        {
            Vertices = new List<Vector4>();
            Edges = new List<int[]>();
        }

        // The Hypercube (8-cell) - 16 vertices
        public static Mesh4D CreateTesseract()
        {
            Mesh4D mesh = new Mesh4D();
            for (int i = 0; i < 16; i++)
            {
                float x = (i & 1) == 0 ? -1 : 1;
                float y = (i & 2) == 0 ? -1 : 1;
                float z = (i & 4) == 0 ? -1 : 1;
                float w = (i & 8) == 0 ? -1 : 1;
                mesh.Vertices.Add(new Vector4(x, y, z, w));
            }
            GenerateEdgesByDistance(mesh, 2.0f); // Vertices distance 2 apart (edge length of 2)
            return mesh;
        }

        // The Pentatope (5-cell) - Simplest regular polychoron (Simplex)
        public static Mesh4D CreatePentatope()
        {
            Mesh4D mesh = new Mesh4D();
            float r = 1.0f;
            
            // Cartesian coordinates for a regular pentatope are tricky, using a simple simplex approximation centered somewhat at zero.
            // Or typically: (1,1,1,-1/√5), (1,-1,-1,-1/√5), (-1,1,-1,-1/√5), (-1,-1,1,-1/√5), (0,0,0, 4/√5) scaled.
            // Let's use a simpler uniform generation that forms a tetrahedron at W=-1 and a point at W=1.
            
            // Base Tetrahedron
            mesh.Vertices.Add(new Vector4( 1,  1,  1, -1));
            mesh.Vertices.Add(new Vector4( 1, -1, -1, -1));
            mesh.Vertices.Add(new Vector4(-1,  1, -1, -1));
            mesh.Vertices.Add(new Vector4(-1, -1,  1, -1));
            
            // Peak Step
            mesh.Vertices.Add(new Vector4( 0,  0,  0,  Math.Sqrt(5) - 1));

            // Connect everything to everything (Simplex property)
            for(int i=0; i<mesh.Vertices.Count; i++)
                for(int j=i+1; j<mesh.Vertices.Count; j++)
                    mesh.Edges.Add(new int[] { i, j });

            return mesh;
        }

        // The 16-Cell (Cross-Polytope) - 8 vertices, 24 edges. (Like a double-pyramid in 4D)
        public static Mesh4D Create16Cell()
        {
            Mesh4D mesh = new Mesh4D();
            // 8 Vertices (+-1 on each axis)
            mesh.Vertices.Add(new Vector4(1, 0, 0, 0));
            mesh.Vertices.Add(new Vector4(-1, 0, 0, 0));
            mesh.Vertices.Add(new Vector4(0, 1, 0, 0));
            mesh.Vertices.Add(new Vector4(0, -1, 0, 0));
            mesh.Vertices.Add(new Vector4(0, 0, 1, 0));
            mesh.Vertices.Add(new Vector4(0, 0, -1, 0));
            mesh.Vertices.Add(new Vector4(0, 0, 0, 1));
            mesh.Vertices.Add(new Vector4(0, 0, 0, -1));

            // 24 Edges (connect every vertex to every other vertex except opposite)
            // Easier way: loop all pairs, check dist.
            // Dist squared between adjacent (1,0..) and (0,1..) is 1+1=2.
            // Dist squared opposite (1,0..) and (-1,0..) is 4.
            for (int i = 0; i < 8; i++)
            {
                for (int j = i + 1; j < 8; j++)
                {
                    float d2 = Vector4.Distance(mesh.Vertices[i], mesh.Vertices[j]);
                    // Sqrt(2) is approx 1.414. Dist is 1.414.
                    if (d2 < 1.5f) // Check distance < 2 essentially
                    {
                        mesh.Edges.Add(new int[] { i, j });
                    }
                }
            }
            return mesh;
        }

        public static Mesh4D CreateHyperGrid(int halfSize, float step)
        {
            Mesh4D mesh = new Mesh4D();
            // Create a grid on the X-Z plane, repeated at W intervals?
            // Or just a single X-Z plane at W=0, but let's make it look cool.
            // Let's make a grid on X-Z plane at Y = -2.
            
            float y = -2.0f;
            // Lines along X
            for (int i = -halfSize; i <= halfSize; i++)
            {
                float z = i * step;
                // Line from -X to +X
                int vStart = mesh.Vertices.Count;
                mesh.Vertices.Add(new Vector4(-halfSize * step, y, z, 0));
                mesh.Vertices.Add(new Vector4(halfSize * step, y, z, 0));
                mesh.Edges.Add(new int[] { vStart, vStart + 1 });
            }
            // Lines along Z
            for (int i = -halfSize; i <= halfSize; i++)
            {
                float x = i * step;
                int vStart = mesh.Vertices.Count;
                mesh.Vertices.Add(new Vector4(x, y, -halfSize * step, 0));
                mesh.Vertices.Add(new Vector4(x, y, halfSize * step, 0));
                mesh.Edges.Add(new int[] { vStart, vStart + 1 });
            }
            
            return mesh;
        }

        private static void GenerateEdgesByDistance(Mesh4D mesh, float targetDist, float epsilon = 0.01f)
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                for (int j = i + 1; j < mesh.Vertices.Count; j++)
                {
                    float dist = Vector4.Distance(mesh.Vertices[i], mesh.Vertices[j]);
                    if (System.Math.Abs(dist - targetDist) < epsilon)
                    {
                        mesh.Edges.Add(new int[] { i, j });
                    }
                }
            }
        }
    }
}
