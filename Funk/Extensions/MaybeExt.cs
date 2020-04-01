using System;
using System.Diagnostics.Contracts;

namespace Funk
{
    public static class MaybeExt
    {
        /// <summary>
        /// Creates Maybe from item.
        /// </summary>
        [Pure]
        public static Maybe<T> AsMaybe<T>(this T item) => Maybe.Create(item);

        /// <summary>
        /// Creates Maybe from item.
        /// </summary>
        [Pure]
        public static Maybe<T> AsMaybe<T>(this T? item) where T : struct => Maybe.Create(item);

        /// <summary>
        /// Gets Maybe value if not empty. Otherwise, returns the result of the selector.
        /// </summary>
        public static R GetOr<T, R>(this Maybe<T> maybe, Func<Unit, R> selector) where T : R => maybe.Match(_ => selector(Unit.Value), v => v);

        /// <summary>
        /// Preferably use GetOr.
        /// Gets Maybe value if not empty. Otherwise, returns default value.
        /// </summary>
        [Pure]
        public static T GetOrDefault<T>(this Maybe<T> maybe) => maybe.Match(_ => default, v => v);

        /// <summary>
        /// Preferably use FlatMap.
        /// Returns Maybe of Maybe if not empty. Otherwise it returns empty Maybe.
        /// This is a FlatMap with a default selector.
        /// </summary>
        [Pure]
        public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> maybe) => maybe.FlatMap(m => m);

        /// <summary>
        /// Maps and resolves Maybe of a nullable value to a Maybe of a value.
        /// </summary>
        [Pure]
        public static Maybe<T> Map<T>(this Maybe<T?> maybe) where T : struct => maybe.FlatMap(v => v.AsMaybe());

        /// <summary>
        /// Returns either not empty Maybe or a Maybe specified by the selector.
        /// </summary>
        public static Maybe<T> Or<T>(this Maybe<T> maybe, Func<Unit, Maybe<T>> ifEmpty) => maybe.Match(_ => ifEmpty(Unit.Value), Maybe.Create);

        /// <summary>
        /// Returns either not empty Maybe or a Maybe specified by the selector.
        /// </summary>
        public static Maybe<T> Or<T>(this Maybe<T?> maybe, Func<Unit, Maybe<T>> ifEmpty) where T : struct => maybe.Match(_ => ifEmpty(Unit.Value), Maybe.Create);
    }
}
