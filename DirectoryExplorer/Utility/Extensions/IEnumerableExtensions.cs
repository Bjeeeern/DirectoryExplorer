using System;
using System.Collections.Generic;
using System.Linq;
using DirectoryExplorer.Primitives;

namespace DirectoryExplorer.Utility.Extensions
{
    static class IEnumerableExtensions
    {
        public static IEnumerable<IEntity> WhereDo<T>(this IEnumerable<IEntity> enumerable, Func<T, bool> test, Action<T> action) where T : class =>
            enumerable.Select(e =>
            {
                if (e is T t && test(t)) action(t);
                return e;
            });

        public static IEnumerable<IEntity> IfDo<T>(this IEnumerable<IEntity> enumerable, Action<T> action) where T : class =>
            enumerable.Select(e =>
            {
                if (e is T t) action(t);
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