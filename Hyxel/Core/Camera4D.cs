using Hyxel.Math;
using Hyxel.Physics;

namespace Hyxel.Core
{
    public class Camera4D
    {
        public Vector4 Position { get; set; }
        public Matrix5 Orientation { get; set; } // Camera Rotation (World to Local if inverted, or Local to World standard)
        
        public float FocalLength4D { get; set; } = 2.0f; // Distance from "retina" to projection point in 4D
        
        public Camera4D(Vector4 startPos)
        {
            Position = startPos;
            Orientation = Matrix5.Identity();
        }

        // Move the camera in its local coordinate space
        // delta.X = Right, Y = Up, Z = Forward, W = Ana/Kata
        public void Move(Vector4 localDelta)
        {
            // Transform local delta by Orientation to get world delta
            Vector4 worldDelta = Matrix5.Multiply(Orientation, localDelta);
            Position += worldDelta;
        }

        public void Rotate(Matrix5 rotation)
        {
            Orientation = Orientation * rotation;
        }

        /// <summary>
        /// Projects a 4D point to 3D space relative to this camera.
        /// Uses a stereographic-style projection.
        /// </summary>
        public Vector4 Project(Vector4 v)
        {
            // 1. World -> Camera Space
            // ViewMatrix = Inverse(Orientation) * Translation(-Position)
            // Inverse of Rotation Matrix = Transpose
            
            // Step A: Translate
            Vector4 relative = v - Position;
            
            // Step B: Rotate (Transpose)
            // Hand-rolled Transpose Multiply for efficiency maybe? Or just use Matrix5 helper if we add Transpose.
            // Let's assume Matrix5.Transpose exists or implement inline.
            // Vector4 local = Matrix5.Multiply(Matrix5.Transpose(Orientation), relative);
            
            // INLINE TRANSPOSE MULTIPLY:
            // Row-major M. Transpose means rows become cols.
            // Result.X = Dot(Col0, relative)
            // Result.Y = Dot(Col1, relative) ...
            
            float x = Orientation.M[0,0]*relative.X + Orientation.M[1,0]*relative.Y + Orientation.M[2,0]*relative.Z + Orientation.M[3,0]*relative.W;
            float y = Orientation.M[0,1]*relative.X + Orientation.M[1,1]*relative.Y + Orientation.M[2,1]*relative.Z + Orientation.M[3,1]*relative.W;
            float z = Orientation.M[0,2]*relative.X + Orientation.M[1,2]*relative.Y + Orientation.M[2,2]*relative.Z + Orientation.M[3,2]*relative.W;
            float w = Orientation.M[0,3]*relative.X + Orientation.M[1,3]*relative.Y + Orientation.M[2,3]*relative.Z + Orientation.M[3,3]*relative.W;
            
            Vector4 cameraSpacePos = new Vector4(x, y, z, w);

            // 2. Project
            // If we look down the W axis, W is depth.
            // wProj = 1 / w_depth
            
            float wDepth = cameraSpacePos.W;
            
            float div = FocalLength4D - wDepth;
            
            if (System.Math.Abs(div) < 0.0001f) div = 0.0001f;

            float scale = 1.0f / div;
            
            // projected w is 0 (flat on 3D hyperplane)
            return new Vector4(cameraSpacePos.X * scale, cameraSpacePos.Y * scale, cameraSpacePos.Z * scale, 0); 
        }

        // Generate a 4D Ray from screen coordinates (simplified for this demo)
        // We assume screen is at Z=0 in the 3D slice, and the 3D slice is at W=0.
        // Camera is at W = -FocalLength.
        public Ray4D ScreenPointToRay(float screenX, float screenY, float screenWidth, float screenHeight)
        {
            // 1. Normalize Screen Coordinates (-1 to 1)
            float ndcX = (screenX / screenWidth) * 2.0f - 1.0f;
            float ndcY = -((screenY / screenHeight) * 2.0f - 1.0f); // Flip Y

            // 2. Unproject to 3D View Plane (Say, Z=0 plane in 3D space)
            // We need to account for the FOV scale used in ProjectToScreen logic (600.0f)
            float fovScale = 600.0f;
            // x = ndc * zDist / fovScale? No, let's reverse the ProjectToScreen logic.
            // X_screen = x_3d * scale + center
            // X_screen - center = x_3d * scale
            // x_3d = (X_screen - center) / scale
            // If we assume the 3D slice is at W=0.
            
            // Let's create a ray that originates from the 4D Camera position
            // And passes through the "pixel" in 4D space.
            // The pixel exists on the W=0 hyperplane (our 'retina').
            
            // Unproject 2D -> 3D position on the retina
            // We'll pick a Z for the retina plane, e.g., where the 3D projection happens.
            // In DimenshiftGame, ProjectToScreen uses "zDist = 4.0f - v.Z". 
            // If v.Z = 0, zDist = 4.0. Scale = 600/4 = 150.
            
            float targetZ = 0.0f; 
            float zDist = 4.0f - targetZ; 
            float scale = 600.0f / zDist; // 150

            float x3 = ndcX * (screenWidth * 0.5f) / scale; 
            float y3 = ndcY * (screenHeight * 0.5f) / scale;
            
            // Target point on the W=0 hyperplane
            Vector4 targetPixel = new Vector4(x3, y3, targetZ, 0.0f);
            
            // Ray Origin = Camera Position
            Vector4 origin = Position;
            
            // Direction = Target - Origin
            Vector4 dir = targetPixel - origin;
            
            return new Ray4D(origin, dir);
        }
    }
}
