using System.Collections.Generic;

namespace DirectoryExplorer.Primitives
{
    internal interface IRoom : IEntity
    {
        IList<IPolygon> Walls { get; set; }
    }
}