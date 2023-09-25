using DirectoryExplorer.Primitives;
using System.Collections.Generic;

namespace DirectoryExplorer.Utility.Extensions
{
    internal static class ListExtensions
    {
        public static List<IEntity> Add<T>(this List<IEntity> list) where T : IEntity, new() {
            list.Add(new T());
            return list;
        }
    }
}
