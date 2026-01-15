using System;
using SDL2;
using Hyxel.Math;

namespace Hyxel.Graphics
{
    public class SDLRenderer : IRenderer
    {
        private IntPtr _renderer;

        public SDLRenderer(IntPtr rendererPtr)
        {
            _renderer = rendererPtr;
            SDL.SDL_SetRenderDrawBlendMode(_renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
        }

        public void Clear(byte r, byte g, byte b, byte a)
        {
            SDL.SDL_SetRenderDrawColor(_renderer, r, g, b, a);
            SDL.SDL_RenderClear(_renderer);
        }

        public void Present()
        {
            SDL.SDL_RenderPresent(_renderer);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Vector4 color)
        {
            byte r = (byte)(color.X * 255);
            byte g = (byte)(color.Y * 255);
            byte b = (byte)(color.Z * 255);
            byte a = (byte)(color.W * 255);

            SDL.SDL_SetRenderDrawColor(_renderer, r, g, b, a);
            SDL.SDL_RenderDrawLine(_renderer, x1, y1, x2, y2);
        }
    }
}
