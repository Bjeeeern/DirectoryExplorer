using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Entities
{
    internal class File : IText
    {
        public Color Color { get; set; } = Color.White;
        public Vector2 Pos { get; set; }
        public string Content { get; set; }
        public string SpriteFont { get; set; } = "default";
        public ICamera Camera { get; set; }
    }
}
