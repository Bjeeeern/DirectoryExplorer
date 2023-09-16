using System;
using System.Collections.Generic;
using System.Linq;
using DirectoryExplorer.Primitives;

namespace DirectoryExplorer.Utility.Extensions
{
    static class IEnumerableExtensions
    {
        public static IEnumerable<IEntity> WhereDo<T>(this IEnumerable<IEntity> enumerable, Action<T> action) where T : class =>
            throw new NotImplementedException();

        public static IEnumerable<IEntity> IfDo<T>(this IEnumerable<IEntity> enumerable, Action<T> action) where T : class =>
            enumerable.Select(e =>
            {
                if (e is T) action(e as T);
                return e;
            });

        public static IEnumerable<IEntity> Do<T>(this IEnumerable<IEntity> enumerable, Action<T> action) where T : class =>
            enumerable.Select(e =>
            {
                action(e as T);
                return e;
            });

        public static void Enumerate(this IEnumerable<IEntity> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext()) { }
        }

        public static IEnumerable<IEntity> Add<T>(this IEnumerable<IEntity> enumerable) where T : IEntity, new() =>
            enumerable.Append(new T());
    }
}