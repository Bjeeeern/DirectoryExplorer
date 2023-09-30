using Microsoft.Xna.Framework;

namespace Game.Primitives
{
    internal interface IMovable: IEntity
    {
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }
    }
}