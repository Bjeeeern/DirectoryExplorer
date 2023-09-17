using DirectoryExplorer.Primitives;
using System.Collections.Generic;

namespace DirectoryExplorer.Services.Interfaces
{
    internal interface IDirectoryExplorer
    {
        IEnumerable<IEntity> BuildEntitiesFromPath(string path);
    }
}