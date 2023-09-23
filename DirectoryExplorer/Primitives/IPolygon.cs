using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DirectoryExplorer.Entities
{
    internal interface IPolygon : ICameraEntity, ITintend
    {
        IEnumerable<Vector2> Vertices { get; set; }
    }
}