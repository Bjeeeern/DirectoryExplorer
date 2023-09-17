namespace DirectoryExplorer.Primitives
{
    internal interface IText : IPositioned, ITintend
    {
        string Text { get; set; }
        string SpriteFont { get; set; }
    }
}
