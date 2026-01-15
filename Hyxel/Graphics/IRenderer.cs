using Hyxel.Math;

namespace Hyxel.Graphics
{
    public interface IRenderer
    {
        void Clear(byte r, byte g, byte b, byte a);
        void Present();
        void DrawLine(int x1, int y1, int x2, int y2, Vector4 color);
        
        // Future expansion: DrawTriangle, DrawText, etc.
    }
}
