namespace Hyxel.Core
{
    public interface IGame
    {
        void Initialize();
        void LoadContent();
        void Update(float deltaTime);
        void Draw(float deltaTime);
        void UnloadContent();
    }
}
