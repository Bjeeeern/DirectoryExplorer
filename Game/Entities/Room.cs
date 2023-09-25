using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DirectoryExplorer.Entities
{
    internal class Room : IRoom
    {
        public IEnumerable<IPolygon> Walls { get; set; }
        public Color Color { get; set; } = Color.DarkOrchid;
    }
}
