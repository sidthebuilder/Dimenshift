using System;

namespace SDL2
{
    public static class SDL
    {
        public const uint SDL_INIT_VIDEO = 0x00000020u;
        public const uint SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000u;
        public const uint SDL_WINDOW_SHOWN = 0x00000004u;

        // Mock Event Structure
        public struct SDL_Event 
        {
            public uint type;
        }

        // Mock Keyboard State
        public static byte[] SDL_GetKeyboardState(out int numkeys)
        {
            numkeys = 512;
            byte[] keys = new byte[512];
            // Simulate pressing 'W' (moves forward) roughly via logic or random? 
            // Better: Don't simulate random presses, let the Game Loop control "Auto-Pilot" if no real input.
            // We just return empty for now, Game Logic will handle "Demo Mode".
            return keys;
        }

        public static void SDL_GetMouseState(out int x, out int y)
        {
            // Simulate mouse movement for testing (linear motion)
            var ticks = DateTime.Now.Ticks / 10000;
            x = (int)(Math.Sin(ticks * 0.001) * 600 + 640);
            y = (int)(Math.Cos(ticks * 0.001) * 300 + 360);
        }

        public static int SDL_Init(uint flags)
        {
            Console.WriteLine($"[SDL2-CS] Initializing System 0x{flags:X}...");
            return 0;
        }

        public enum SDL_BlendMode
        {
            SDL_BLENDMODE_NONE = 0x00000000,
            SDL_BLENDMODE_BLEND = 0x00000001,
            SDL_BLENDMODE_ADD = 0x00000002,
            SDL_BLENDMODE_MOD = 0x00000004
        }

        public static void SDL_SetRenderDrawBlendMode(IntPtr renderer, SDL_BlendMode blendMode)
        {
            // Stub
        }

        public static void SDL_Quit()
        {
            Console.WriteLine("[SDL2-CS] Quitting...");
        }

        public static IntPtr SDL_CreateWindow(string title, int x, int y, int w, int h, uint flags)
        {
            Console.WriteLine($"[SDL2-CS] Created Window: {title} ({w}x{h})");
            return new IntPtr(1); // Return non-zero to simulate success
        }

        public static IntPtr SDL_CreateRenderer(IntPtr window, int index, uint flags)
        {
            Console.WriteLine("[SDL2-CS] Created Renderer");
            return new IntPtr(2);
        }

        public static int SDL_SetRenderDrawColor(IntPtr renderer, byte r, byte g, byte b, byte a)
        {
            return 0;
        }

        public static int SDL_RenderClear(IntPtr renderer)
        {
            // Clear screen
            return 0;
        }

        public static int SDL_RenderPresent(IntPtr renderer)
        {
            // Swap buffers
            return 0;
        }

        public static int SDL_RenderDrawLine(IntPtr renderer, int x1, int y1, int x2, int y2)
        {
            // In a real app, this draws. 
            // Here we might just log occasionally or do nothing to keep output clean.
            return 0;
        }

        public static int SDL_PollEvent(out SDL_Event e)
        {
            e = new SDL_Event();
            // Return 0 to imply no events for this stub, or we get stuck in infinite loops if we fake inputs too aggressively without a real window.
            return 0; 
        }

        public static void SDL_Delay(uint ms)
        {
            System.Threading.Thread.Sleep((int)ms);
        }
    }
}
