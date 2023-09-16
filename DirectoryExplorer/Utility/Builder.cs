using System.Collections.Generic;
using DirectoryExplorer.Primitives;

namespace DirectoryExplorer.Utility
{
    static class Builder
    {
        public static IEnumerable<IEntity> Build<T>(int size)
            where T : IEntity, new()
        {
            while (size-- != 0) yield return new T();
        }
    }
}