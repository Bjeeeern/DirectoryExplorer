using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Primitives
{
    internal interface IPositioned : IEntity
    {
        public Vector2 Pos { get; set; }
    }
}