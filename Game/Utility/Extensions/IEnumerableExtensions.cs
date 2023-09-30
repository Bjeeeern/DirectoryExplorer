using System;
using System.Collections.Generic;
using System.Linq;
using Game.Primitives;

namespace Game.Utility.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<IEntity> enumerable) where T : class =>
            enumerable
                .Where(e => e is T)
                .Cast<T>();

        public static IEnumerable<IEntity> DoWhere<T>(this IEnumerable<IEntity> enumerable, Func<T, bool> test, Action<T> action) where T : class =>
            enumerable.Select(e =>
            {
                if (e is T t && test(t)) action(t);
                return e;
            });

        public static IEnumerable<IEntity> DoIf<T>(this IEnumerable<IEntity> enumerable, Action<T> action) where T : class =>
            enumerable.Select(e =>
            {
                if (e is T t) action(t);
                return e;
            });

        public static IEnumerable<(IEntity, IEntity)> AllInteractions(this IEnumerable<IEntity> enumerable) =>
            enumerable.SelectMany(a => enumerable.Where(b => a != b).Select(b => (a, b))).DistinctBy(x => x.a.GetHashCode() * x.b.GetHashCode());

        public static IEnumerable<(IEntity, IEntity)> DoIf<T1, T2>(this IEnumerable<(IEntity A, IEntity B)> enumerable, Action<T1, T2> action) where T1 : class where T2 : class =>
            enumerable.Select(e =>
            {
                if (e.A is T1 && e.B is T2)
                    action(e.A as T1, e.B as T2);
                else if (e.B is T1 && e.A is T2)
                    action(e.B as T1, e.A as T2);
                return e;
            });

        public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<T> action) =>
            enumerable.Select(e =>
            {
                action(e);
                return e;
            });

        public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<T, int> action) =>
            enumerable.Select((e, i) =>
            {
                action(e, i);
                return e;
            });

        public static IEnumerable<(T1, T2)> Do<T1, T2>(this IEnumerable<(T1 A, T2 B)> enumerable, Action<T1, T2> action) =>
            enumerable.Select(e =>
            {
                action(e.A, e.B);
                return e;
            });

        public static void Enumerate<T>(this IEnumerable<T> enumerable)
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
    }
}