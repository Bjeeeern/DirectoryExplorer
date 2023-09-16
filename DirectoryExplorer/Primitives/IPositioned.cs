using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DirectoryExplorer.Primitives
{
    internal interface IPositioned : IEntity
    {
        public Vector2 Pos { get; set; }
    }
}