using System.Collections.Generic;
using DirectoryExplorer.Primitives;

namespace DirectoryExplorer.Utility
{
    static class Builder
    {
        public static IEnumerable<IEntity> Build<T>(int count) where T : IEntity, new()
        {
            while (count-- != 0) yield return new T();
        }
    }
}