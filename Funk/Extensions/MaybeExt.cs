using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funk
{
    public static class MaybeExt
    {
        [Pure]
        public static Maybe<T> AsMaybe<T>(this T item) => Maybe.Create(item);

        [Pure]
        public static Maybe<T> AsMaybe<T>(this T? item) where T : struct => Maybe.Create(item);

        [Pure]
        public static R GetOrElse<T, R>(this Maybe<T> maybe, Func<Unit, R> selector) where T : R => maybe.Match(_ => selector(Unit.Value), v => v);

        [Pure]
        public static T GetOrNull<T>(this Maybe<T> maybe) where T : class => maybe.Match(_ => null, v => v);

        [Pure]
        public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> maybe) => maybe.FlatMap(m => m);

        [Pure]
        public static Maybe<T> Map<T>(this Maybe<T?> maybe) where T : struct => maybe.FlatMap(v => v.AsMaybe());

        [Pure]
        public static IEnumerable<T> GetOr<T>(this Maybe<IEnumerable<T>> maybe, Func<Unit, IEnumerable<T>> otherwise = null)
        {
            return maybe.GetOrElse(_ => otherwise.AsMaybe().Match(
                f => Enumerable.Empty<T>(), 
                f => f(Unit.Value))
            );
        }

        [Pure]
        public static Maybe<R> Or<T, R>(this Maybe<T> maybe, Func<Unit, Maybe<R>> ifEmpty)
            where T : R
        {
            return maybe.Match(
                _ => ifEmpty(Unit.Value),
                v => Maybe.Create((R)v)
            );
        }
    }
}
