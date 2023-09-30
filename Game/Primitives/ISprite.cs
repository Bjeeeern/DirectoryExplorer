namespace Game.Primitives
{
    internal interface ISprite : IPositioned, ITintend
    {
        string TextureName { get; set; }
    }
}