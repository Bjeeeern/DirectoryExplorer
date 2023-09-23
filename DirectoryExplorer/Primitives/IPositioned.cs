using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DirectoryExplorer.Primitives
{
    internal interface IPositioned : ICameraEntity
    {
        public Vector2 Pos { get; set; }
    }
}