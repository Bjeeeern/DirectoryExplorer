using Microsoft.Xna.Framework;

namespace Game.Primitives
{
    internal interface IRoom : IEntity
    {
        public (Vector2 Translation, Vector2 Angle) UpdateRoomSize(Vector2 playerPos);
    }
}