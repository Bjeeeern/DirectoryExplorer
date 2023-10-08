using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using System.Collections.Generic;

namespace Game.Primitives
{
    internal interface IRoom : ITintend, IEntity
    {
        void UpdateRoomSize();
        Vector2 DoRoomTransform(Vector2 point, float scalar);
        float GetHorizontalPointScalar(Vector2 point);
        List<Polyline> Walls { get; set; }
    }
}