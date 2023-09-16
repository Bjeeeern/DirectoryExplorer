using DirectoryExplorer.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DirectoryExplorer.Entities
{
    class Ball : IBody, ISprite
    {
        public string TextureName { get; set; } = "ball";
        public Color Color { get; set; } = Color.White;
        public Vector2 Pos { get; set; }
        public float Speed { get; set; } = 100.0f;
        public Vector2 Direction { get; set; }
    }
}