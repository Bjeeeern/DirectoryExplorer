using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Entities
{
    internal class ParentDirectory : IText
    {
        public Color Color { get; set; } = Color.DarkOliveGreen;
        public Vector2 Pos { get; set; }
        public string Content { get; set; }
        public string SpriteFont { get; set; } = "default";
        public ICamera Camera { get; set; }
    }
}
