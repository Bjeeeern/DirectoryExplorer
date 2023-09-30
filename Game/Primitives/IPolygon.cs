using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Game.Primitives
{
    internal interface IPolygon : IEntity, ITintend
    {
        IList<Vector2> Vertices { get; set; }
    }
}