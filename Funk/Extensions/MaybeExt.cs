using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    public static class MaybeExt
    {
        /// <summary>
        /// Structure-preserving map.
        /// Maps not empty Maybe to the Task of new Maybe of the selector. Otherwise, returns Task of empty Maybe of the selector.
        /// Use FlatMap if you have nested Maybes.
        /// </summary>
        public static async Task<Maybe<R>> MapAsync<T, R>(this Task<Maybe<T>> maybe, Func<T, Task<R>> selector) => await (await maybe).MapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map.
        /// Binds not empty Maybe to the Task of new Maybe of the selector. Otherwise, returns Task of empty Maybe of the selector.
        /// </summary>
        public static async Task<Maybe<R>> FlatMapAsync<T, R>(this Task<Maybe<T>> maybe, Func<T, Task<Maybe<R>>> selector) => await (await maybe).FlatMapAsync(selector).ConfigureAwait(false);

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
        /// Gets Maybe value if not empty. Otherwise, returns the result of the selector.
        /// </summary>
        public static Task<R> GetOrAsync<T, R>(this Maybe<T> maybe, Func<Unit, Task<R>> selector) where T : R => maybe.ToTask().GetOrAsync(selector);

        /// <summary>
        /// Gets Maybe value if not empty. Otherwise, returns the result of the selector.
        /// </summary>
        public static async Task<R> GetOrAsync<T, R>(this Task<Maybe<T>> maybe, Func<Unit, Task<R>> selector) where T : R => await (await maybe).Match(_ => selector(Unit.Value), v => result((R)v)).ConfigureAwait(false);

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
        public static Maybe<R> Or<T, R>(this Maybe<T> maybe, Func<Unit, Maybe<R>> ifEmpty) where T : R => maybe.Match(_ => ifEmpty(Unit.Value), v => Maybe.Create((R)v));

        /// <summary>
        /// Returns either not empty Maybe or a Maybe specified by the selector.
        /// </summary>
        public static Task<Maybe<R>> OrAsync<T, R>(this Maybe<T> maybe, Func<Unit, Task<Maybe<R>>> ifEmpty) where T : R => maybe.ToTask().OrAsync(ifEmpty);

        /// <summary>
        /// Returns either not empty Maybe or a Maybe specified by the selector.
        /// </summary>
        public static async Task<Maybe<R>> OrAsync<T, R>(this Task<Maybe<T>> maybe, Func<Unit, Task<Maybe<R>>> ifEmpty) where T : R => await (await maybe).Match(_ => ifEmpty(Unit.Value), v => Maybe.Create((R)v).ToTask()).ConfigureAwait(false);

        /// <summary>
        /// Creates Exc from Maybe with the specified Exception if Maybe is empty.
        /// </summary>
        public static Exc<T, E> ToExc<T, E>(this Maybe<T> maybe, Func<Unit, E> ifEmpty) where E : Exception => maybe.Match(_ => Exc.Failure<T, E>(ifEmpty(Unit.Value)), Exc.Success<T, E>);

        /// <summary>
        /// Creates Exc from Maybe with the specified or default EmptyValueException if Maybe is empty.
        /// </summary>
        public static Exc<T, EmptyValueException> ToExc<T>(this Maybe<T> maybe, Func<Unit, EmptyValueException> ifEmpty = null) =>
            maybe.Match(_ => Exc.Failure<T, EmptyValueException>(ifEmpty.AsMaybe().Map(f => f(Unit.Value)).GetOr(__ => new EmptyValueException("Maybe value is empty."))), Exc.Success<T, EmptyValueException>);
    }
}
