using System;
using Hyxel.Graphics;
using SDL2;

namespace Hyxel.Core
{
    public class Engine : IDisposable
    {
        private bool _isRunning;
        private IntPtr _window;
        private IntPtr _renderer;
        private SDLRenderer _graphicsDevice;
        private IGame _game;

        public SDLRenderer GraphicsDevice => _graphicsDevice;

        public Engine(IGame game, string title, int width, int height)
        {
            _game = game;
            
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
                throw new Exception("SDL Init Failed");

            _window = SDL.SDL_CreateWindow(title, 
                SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 
                width, height, SDL.SDL_WINDOW_SHOWN);

            _renderer = SDL.SDL_CreateRenderer(_window, -1, 0);
            _graphicsDevice = new SDLRenderer(_renderer);
        }

        public void SetGame(IGame game)
        {
            _game = game;
        }

        public void Run()
        {
            if (_game == null) return;

            _game.Initialize();
            _game.LoadContent();
            
            _isRunning = true;
            
            while (_isRunning)
            {
                // Input
                SDL.SDL_Event e;
                while (SDL.SDL_PollEvent(out e) != 0)
                {
                    if (e.type == 0x100) _isRunning = false;
                    // Pass input to game (stub)
                }

                // Loop
                float deltaTime = 0.016f; // Fixed 60fps stub
                _game.Update(deltaTime);
                
                _graphicsDevice.Clear(10, 10, 15, 255); // Default clear
                _game.Draw(deltaTime);
                _graphicsDevice.Present();

                SDL.SDL_Delay(16);
            }

            _game.UnloadContent();
        }

        public void Dispose()
        {
            SDL.SDL_DestroyRenderer(_renderer);
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
        }
    }
}
