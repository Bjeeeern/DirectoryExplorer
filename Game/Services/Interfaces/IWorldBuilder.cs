using DirectoryExplorer.Primitives;
using System.Collections.Generic;

namespace DirectoryExplorer
{
    internal interface IWorldBuilder
    {
        List<IEntity> BuildWorld();
    }
}