using Game.Primitives;
using Microsoft.Xna.Framework;

namespace Game.Entities
{
    internal class SubDirectoryText : IText
    {
        public Color Color { get; set; } = Color.GreenYellow;
        public Vector2 Pos { get; set; }
        public string Content { get; set; }
        public string SpriteFont { get; set; } = "default";
        public int? Limit { get; set; }
    }
}
