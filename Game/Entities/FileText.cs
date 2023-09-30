using Game.Primitives;
using Microsoft.Xna.Framework;

namespace Game.Entities
{
    internal class FileText : IText
    {
        public Color Color { get; set; } = Color.White;
        public Vector2 Pos { get; set; }
        public string Content { get; set; }
        public string SpriteFont { get; set; } = "default";
        public int? Limit { get; set; }
    }
}
