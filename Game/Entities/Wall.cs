using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DirectoryExplorer.Entities
{
    internal class Wall : IPolygon
    {
        public IList<Vector2> Vertices { get; set; }
        public Color Color { get; set; } = Color.DarkOrchid;
    }
}
