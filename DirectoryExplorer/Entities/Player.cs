using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Entities
{
    class Player : IPlayer, ICamera, IRoomTracker, ISprite
    {
        public string CurrentDirectory { get; set; } = ".";
        public Matrix Transform { get; set; } = Matrix.Identity;
        public string TextureName { get; set; } = "ball";
        public Vector2 Pos { get => new Vector2(-Transform.Translation.X, -Transform.Translation.Y); set => new Vector2(Transform.Translation.X, Transform.Translation.Y); }
        public Color Color { get; set; } = Color.White;
        public float Speed { get; set; } = 300.0f;
        public Vector2 Direction { get; set; }
    }
}