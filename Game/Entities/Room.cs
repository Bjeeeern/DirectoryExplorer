using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DirectoryExplorer.Entities
{
    internal class Room : IRoom
    {
        public IList<IPolygon> Walls { get; set; }
        public Color Color { get; set; } = Color.DarkOrchid;
    }
}
