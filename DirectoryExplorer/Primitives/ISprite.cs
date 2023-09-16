using Color = Microsoft.Xna.Framework.Color;

namespace DirectoryExplorer.Primitives
{
    internal interface ISprite : IPositioned
    {
        string TextureName { get; set; }
        Color Color { get; set; }
    }
}