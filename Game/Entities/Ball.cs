using Game.Primitives;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Game.Entities
{
    class Ball : ICircle, ISprite
    {
        public string TextureName { get; set; } = "ball";
        public Color Color { get; set; } = Color.White;
        public Vector2 Pos { get; set; }
        public float Radius { get; set; } = 32.0f;
        public float Speed { get; set; } = 300.0f;
        public Vector2 Direction { get; set; }
    }
}