using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Entities
{
    internal class SubDirectoryText : IText
    {
        public Color Color { get; set; } = Color.GreenYellow;
        public Vector2 Pos { get; set; }
        public string Content { get; set; }
        public string SpriteFont { get; set; } = "default";
    }
}
