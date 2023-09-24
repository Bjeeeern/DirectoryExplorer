using DirectoryExplorer.Primitives;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DirectoryExplorer.Services.Interfaces
{
    internal interface IDirectoryExplorer
    {
        IEnumerable<IEntity> BuildEntitiesFromPath(List<IEntity> entities, string path, Vector2 origin = new Vector2());
    }
}