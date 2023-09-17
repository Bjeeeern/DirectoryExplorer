using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Entities
{
    internal class Text : IText
    {
        public Color Color { get; set; } = Color.White;
        public Vector2 Pos { get; set; }
        string IText.Text { get; set; }
        public string SpriteFont { get; set; } = "default";
    }
}
