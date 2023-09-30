using Microsoft.Xna.Framework;

namespace Game.Primitives
{
    internal interface IPositioned : IEntity
    {
        public Vector2 Pos { get; set; }
    }
}