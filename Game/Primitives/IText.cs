using Game.Utility.Extensions;

namespace Game.Primitives
{
    internal interface IText : IPositioned, ITintend
    {
        int? Limit { get; set; }
        string Content { get; set; }
        string Text => Limit.HasValue ? Content.Shorten(Limit.Value) : Content;
        string SpriteFont { get; set; }
    }
}
