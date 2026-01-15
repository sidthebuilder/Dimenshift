using System;
using Hyxel.Core;

namespace Hyxel
{
    class Program
    {
        static void Main(string[] args)
        {
            try 
            {
                // Engine acts as the host
                // Game acts as the logic provider
                // Dependency Injection style construction
                
                // We need a slight refactor: Engine takes Game. But Game usually needs Engine services.
                // We'll construct Engine first without Game, or create Game using a Factory/bootstrap.
                // Let's modify Engine to accept Game in Run or Setter to fix circular dependency in constructor if needed.
                // Or simplified:
                
                // 1. Create Game (null engine reference initially?)
                // Actually Engine creating the renderer is key.
                
                // Let's pass Engine to Game constructor, but Engine needs Game to Run.
                // Solution: Two-stage init.
                
                var shimGame = new GameCommon(); // placeholder
                using (var engine = new Engine(null, "Dimenshift Enterprise", 1280, 720))
                {
                    var game = new DimenshiftGame(engine);
                    engine.SetGame(game); // We need to add this method to Engine
                    engine.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATAL] Engine Crash: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
    
    // Stub to fix compilation until I edit Engine.cs to allow SetGame
    public class GameCommon : IGame {
        public void Draw(float dt){}
        public void Initialize(){}
        public void LoadContent(){}
        public void UnloadContent(){}
        public void Update(float dt){}
    }
}
