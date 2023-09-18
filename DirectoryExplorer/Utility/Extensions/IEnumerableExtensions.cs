using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<T> action) where T : class =>
            enumerable.Select(e =>
            {
                action(e);
                return e;
            });

        public static IEnumerable<T> Only<T>(this IEnumerable<IEntity> enumerable) where T : class =>
            enumerable
                .Where(e => e is T)
                .Cast<T>();

        public static void Enumerate(this IEnumerable<IEntity> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext()) { }
        }

        public static IEnumerable<IEntity> Add<T>(this IEnumerable<IEntity> enumerable) where T : IEntity, new() =>
            enumerable.Append(new T());

        public static IEnumerable<IEntity> Add<T>(this IEnumerable<IEntity> enumerable, int count) where T : IEntity, new() =>
            enumerable.Concat(Generate<T>(count));

        private static IEnumerable<IEntity> Generate<T>(int count) where T : IEntity, new()
        {
            while (count-- != 0) yield return new T();
        }

        public static T Get<T>(this IEnumerable<IEntity> enumerable) where T : class, IEntity =>
            enumerable.Single(x => x is T) as T;
    }
}