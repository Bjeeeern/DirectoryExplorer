using System.Collections.Generic;

namespace DirectoryExplorer.Primitives
{
    internal interface IRoom : IEntity
    {
        IEnumerable<IPolygon> Walls { get; set; }
    }
}