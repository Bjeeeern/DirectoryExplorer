using Color = Microsoft.Xna.Framework.Color;

namespace DirectoryExplorer.Primitives
{
    internal interface ISprite : IPositioned, ITintend
    {
        string TextureName { get; set; }
    }
}