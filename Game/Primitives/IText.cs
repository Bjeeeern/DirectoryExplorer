namespace DirectoryExplorer.Primitives
{
    internal interface IText : IPositioned, ITintend
    {
        string Content { get; set; }
        string SpriteFont { get; set; }
    }
}
