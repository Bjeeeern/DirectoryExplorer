using Microsoft.Xna.Framework;

namespace Game.Primitives
{
    internal interface IRoom : IEntity
    {
        public void UpdateRoomSize(Vector2 playerPos);
    }
}