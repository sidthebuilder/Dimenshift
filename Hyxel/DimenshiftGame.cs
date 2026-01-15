using System;
using System.Collections.Generic;
using Hyxel.Core;
using Hyxel.Graphics;
using Hyxel.Math;
using Hyxel.Scenegraph;
using Hyxel.Physics;

namespace Hyxel
{
    public class DimenshiftGame : IGame
    {
        private Engine _engine;
        private Camera4D _camera;
        private Node4D _rootNode;
        private List<Entity4D> _entities;
        
        private float _rotationAlpha;
        private float _rotationBeta;
        private int _currentEntityIndex = 0;

        public DimenshiftGame(Engine engine)
        {
            _engine = engine;
        }

        public void Initialize()
        {
            Console.WriteLine("[Game] Initializing 4D Viewport...");
            _camera = new Camera4D(new Vector4(0, 0, 0, -3.5f));
            _rootNode = new Node4D("Root");
            _entities = new List<Entity4D>();
        }

        public void LoadContent()
        {
            Console.WriteLine("[Game] Constructing Hyper-Geometry...");

            // Create Entities
            var tesseract = new Entity4D(Mesh4D.CreateTesseract(), "Tesseract");
            tesseract.Color = new Vector4(0, 1, 0.5f, 1); // Cyan-ish

            var pentatope = new Entity4D(Mesh4D.CreatePentatope(), "Pentatope");
            pentatope.Color = new Vector4(1, 0.5f, 0, 1); // Orange

            var crossPoly = new Entity4D(Mesh4D.Create16Cell(), "16-Cell");
            crossPoly.Color = new Vector4(0.8f, 0.2f, 1, 1); // Purple

            var grid = new Entity4D(Mesh4D.CreateHyperGrid(10, 0.5f), "Grid");
            grid.Color = new Vector4(0.3f, 0.3f, 0.3f, 0.5f); // Gray, semi-transparent

            _entities.Add(tesseract);
            _entities.Add(pentatope);
            _entities.Add(crossPoly);
            
            // Grid is always visible, maybe add it to root but not the "active cycler" list?
            _rootNode.AddChild(grid);
            
            // PHYSICS DEMO: Bouncing Hyper-Sphere (actually a Pentatope for now)
            var bouncer = new Entity4D(Mesh4D.CreatePentatope(), "Bouncer");
            bouncer.Color = new Vector4(1, 1, 0, 1); // Yellow
            bouncer.Transform.Position = new Vector4(2, 4, 0, 1); // Start high up
            bouncer.Transform.Scale = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
            bouncer.RigidBody = new RigidBody4D(); // Enable Physics
            
            _entities.Add(bouncer); // Add to entities loop for physics processing
            _rootNode.AddChild(bouncer);

            // Add to Scene Graph (conceptually)
            foreach(var e in _entities) 
            {
                 // Avoid adding duplicates if we already added specifics methods
                 // Current logic: _entities has ALL. _rootNode has visuals.
                 // We added 'grid' and 'bouncer' to root. We added 'bouncer' to entities.
                 // We need to be careful not to add 'bouncer' twice to root or logic might break if we iterate logic differently.
                 // The loop below adds ALL entities to root.
                 if(e != bouncer && e != grid) _rootNode.AddChild(e);
            }

            Console.WriteLine("[Game] Ready.");
        }

        public void Update(float deltaTime)
        {
            // Input logic simulation
            _rotationAlpha += 1.0f * deltaTime;
            _rotationBeta += 0.3f * deltaTime;

            // Cycle shapes
            int time = (int)(DateTime.Now.Ticks / 10000000) % 15;
            if (time < 5) _currentEntityIndex = 0;
            else if (time < 10) _currentEntityIndex = 1;
            else _currentEntityIndex = 2;

            Entity4D active = _entities[_currentEntityIndex];
            
            // Physics / Picking Logic
            SDL2.SDL.SDL_GetMouseState(out int mouseX, out int mouseY);
            Ray4D ray = _camera.ScreenPointToRay(mouseX, mouseY, 1280, 720);

            // --- CAMERA CONTROLLER (Cinematic Auto-Pilot) ---
            // Orbit the center in XZ plane while bobbing in W
            float timeSec = (float)(DateTime.Now.Ticks / 10000000.0);
            
            // Circle radius 6
            float camX = (float)Math.Sin(timeSec * 0.5f) * 6.0f;
            float camZ = (float)Math.Cos(timeSec * 0.5f) * 6.0f;
            float camW = (float)Math.Sin(timeSec * 0.3f) * 2.0f - 4.0f; // -2 to -6 range
            
            _camera.Position = new Vector4(camX, 2.0f, camZ, camW);
            
            // Look At Origin (0,0,0,0)
            // Simplified LookAt Logic:
            // Forward = Normalize(Target - Pos)
            // We need a full Basis construction for Matrix5 LookAt.
            // For now, simpler to just translate. Orientation is Identity implies looking forward along +Z?
            // Actually our camera projects along W.
            // So default view is looking into W.
            // If we move side to side, we should rotate to face center.
            // Implementing full LookAtMatrix is complex for this step. 
            // Let's stick to Translation-only fly-by for the demo, simpler and less nauseating.
            // Reset Orientation every frame just in case
            _camera.Orientation = Matrix5.Identity();
            
            // Rotate camera to look at center?
            // Let's apply a simple Y-rotation to face inward on XZ plane
            float angle = -timeSec * 0.5f + 3.14159f; // Face opposite to circle pos
            _camera.Rotate(Rotors.RotationZX(angle)); // ZX is the horizontal plane rotation
            
            // ------------------------------------------------
            
            // Only Update the "Active" rotating shape
            for(int i=0; i<_entities.Count; i++)
            {
                // Simple visibility toggle: Scale down inactive ones to 0? 
                // Or just set their transform to Identity and hide them from physics?
                // Since our Renderer draws everything in SceneGraph, we should remove them from parent if inactive.
                // But let's just rotate the active one and maybe keep others as "Background Objects" or hide them.
                
                if (i == _currentEntityIndex)
                {
                    var rot1 = Rotors.RotationXW(_rotationAlpha);
                    var rot2 = Rotors.RotationZW(_rotationBeta);
                    var rot3 = Rotors.RotationXY(_rotationAlpha * 0.5f);
                    _entities[i].Transform.Rotation = rot1 * rot2 * rot3;
                    _entities[i].Transform.Scale = Vector4.One; // Show it
                }
                else
                {
                    _entities[i].Transform.Scale = Vector4.Zero; // Hide it (hacky but works)
                }
            }
            
            // ... Rotation Logic (keep existing) ...

            // --- PHYSICS SIMULATION STEP ---
            // Apply Gravity and Integration
            Vector4 gravity = new Vector4(0, -9.8f, 0, 0); // Standard Gravity in Y
            // Maybe add "HyperGravity" in W?
            // Vector4 hyperGravity = new Vector4(0, 0, 0, -2.0f);
            
            foreach (var entity in _entities)
            {
                if (entity.RigidBody != null && !entity.RigidBody.IsStatic)
                {
                    RigidBody4D rb = entity.RigidBody;

                    // 1. Apply Forces
                    rb.AddForce(gravity * rb.Mass * deltaTime);

                    // 2. Integrate Velocity (Euler)
                    rb.Velocity += rb.Acceleration * deltaTime;
                    
                    // Apply Drag
                    rb.Velocity *= (1.0f - rb.Drag * deltaTime);

                    // 3. Integrate Position
                    entity.Transform.Position += rb.Velocity * deltaTime;

                    // 4. Reset Acceleration
                    rb.Acceleration = Vector4.Zero;

                    // --- COLLISION (Hack) ---
                    // Floor at Y = -2.0
                    if (entity.Transform.Position.Y < -1.0f) // -1.0 is considering size approx 1
                    {
                        entity.Transform.Position.Y = -1.0f;
                        // Bounce
                        rb.Velocity.Y = -rb.Velocity.Y * rb.Bounciness;
                        
                        // Friction
                        rb.Velocity.X *= 0.9f;
                        rb.Velocity.Z *= 0.9f;
                        rb.Velocity.W *= 0.9f;
                    }
                    
                    entity.IsDirty = true;
                }
            }
            
            _rootNode.Update(deltaTime);

            // Check Collision against transformed AABB
            // Simplify: Transform ray to local space instead of transforming AABB to world?
            // AABB is axis aligned, transforming it makes it an OBB (Oriented). 
            // For simplicity, let's test ray against a bounding sphere or point-cloud AABB recomputed every frame.
            
            // Recompute World AABB (Expensive but correct for a demo)
            Vector4 min = new Vector4(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue);
            Vector4 max = new Vector4(float.MinValue, float.MinValue, float.MinValue, float.MinValue);
            
            // Get World Vertices
            foreach(var v in active.Mesh.Vertices)
            {
               Vector4 worldV = Matrix5.Multiply(active.WorldMatrix, v);
               min = Vector4.Min(min, worldV);
               max = Vector4.Max(max, worldV);
            }
            AABB4D bounds = new AABB4D(min, max);

            if (ray.Intersects(bounds, out float tMin, out float tMax))
            {
                // HIT! Change color
                active.Color = new Vector4(1, 0, 0, 1); // Red on Hover
            }
            else
            {
                // Reset Color
                 if (_currentEntityIndex == 0) active.Color = new Vector4(0, 1, 0.5f, 1);
            }
        }

        public void Draw(float deltaTime)
        {
            // Traverse Scene Graph for Rendering
            // For this demo, we'll just iterate the root children manually or use a recursive helper.
            DrawNode(_rootNode, deltaTime);
        }

        private void DrawNode(Node4D node, float deltaTime)
        {
            if (node is Entity4D entity)
            {
                RenderEntity(entity);
            }

            foreach(var child in node.Children)
            {
                DrawNode(child, deltaTime);
            }
        }

        private void RenderEntity(Entity4D entity)
        {
            var renderer = _engine.GraphicsDevice;
            Matrix5 world = entity.WorldMatrix;
            var screenPoints = new List<(Point, float)>();

            foreach (var v in entity.Mesh.Vertices)
            {
                Vector4 worldPos = Matrix5.Multiply(world, v);
                Vector4 viewPos = _camera.Project(worldPos);
                float wDepth = worldPos.W;
                screenPoints.Add((ProjectToScreen(viewPos), wDepth));
            }

            foreach (var edge in entity.Mesh.Edges)
            {
                var v1Data = screenPoints[edge[0]];
                var v2Data = screenPoints[edge[1]];

                Point p1 = v1Data.Item1;
                Point p2 = v2Data.Item1;
                float w1 = v1Data.Item2;
                float w2 = v2Data.Item2;

                if (p1.Valid && p2.Valid)
                {
                    Vector4 c1 = GetColorFromW(w1);
                    Vector4 c2 = GetColorFromW(w2);
                    Vector4 baseColor = Vector4.Lerp(c1, c2, 0.5f);
                    
                    // Blend with Entity Color (Multiply)
                    // Result = BaseGradient * EntityColor
                    Vector4 finalColor = new Vector4(
                        baseColor.X * entity.Color.X,
                        baseColor.Y * entity.Color.Y,
                        baseColor.Z * entity.Color.Z,
                        entity.Color.W // Keep alpha from entity? Or gradient? Let's use entity alpha.
                    );
                    
                    if (entity.Color.X > 0.9f && entity.Color.Y < 0.2f) // Selection Red
                    {
                         finalColor = entity.Color;
                    }

                    renderer.DrawLine(p1.X, p1.Y, p2.X, p2.Y, finalColor);
                }
            }
        }

        private Vector4 GetColorFromW(float w)
        {
            // W typically oscillates between -1.5 and 1.5 in this demo
            // Normalize to 0-1 range
            float t = (w + 2.0f) / 4.0f; 
            // Clamp
            t = (t < 0) ? 0 : (t > 1) ? 1 : t;
            
            // Gradient: Purple (0, 0.2, 1) -> Cyan (0, 1, 1) -> White (1, 1, 1)
            // Or "Cold" to "Hot" depth
            
            Vector4 far = new Vector4(0.5f, 0.0f, 1.0f, 0.4f); // Deep Purple, Low Alpha
            Vector4 near = new Vector4(0.0f, 1.0f, 1.0f, 1.0f); // Bright Cyan, High Alpha
            
            return Vector4.Lerp(far, near, t);
        }

        public void UnloadContent() { }

        // Helper for 3D->2D
        struct Point { public int X, Y; public bool Valid; }
        private Point ProjectToScreen(Vector4 v)
        {
            float zDist = 4.0f - v.Z; // Simple camera Z offset
            if (zDist <= 0.1f) return new Point { Valid = false };

            float scale = 600.0f / zDist;
            return new Point
            {
                X = (int)(v.X * scale + 1280 / 2),
                Y = (int)(v.Y * scale + 720 / 2),
                Valid = true
            };
        }
    }
}
