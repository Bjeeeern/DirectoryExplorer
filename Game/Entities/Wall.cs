using Game.Primitives;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Game.Entities
{
    internal class Wall : IPolygon
    {
        public IList<Vector2> Vertices { get; set; }
        public Color Color { get; set; } = Color.DarkOrchid;
    }
}
