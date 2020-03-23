using System;
using System.Diagnostics.Contracts;

namespace Funk
{
    public static class MaybeExt
    {
        [Pure]
        public static Maybe<T> AsMaybe<T>(this T item) => Maybe.Create(item);

        [Pure]
        public static R GetOrElse<T, R>(this Maybe<T> maybe, Func<Unit, R> selector) where T : R => maybe.Match(_ => selector(Unit.Value), v => v);

        [Pure]
        public static T GetOrNull<T>(this Maybe<T> maybe) where T : class => maybe.Match(_ => null, v => v);

        [Pure]
        public static Maybe<R> Or<A, R>(this Maybe<A> maybe, Func<Unit, Maybe<R>> ifEmpty)
            where A : R
        {
            return maybe.Match(
                _ => ifEmpty(Unit.Value),
                v => Maybe.Create((R)v)
            );
        }
    }
}
