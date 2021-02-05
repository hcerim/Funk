using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Provides the Record related extension methods.
    /// </summary>
    public static class MaybeExt
    {
        /// <summary>
        /// Structure-preserving map.
        /// Maps this Maybe to the new Maybe.
        /// If the underlying value is empty, the specified function is not executed and the operation results in an empty Maybe.
        /// Otherwise, the specified function is executed and its result is wrapped in the Maybe indicating the possible absence of returned data.
        /// In case the specified function already returns the Maybe, use FlatMapAsync instead.
        /// </summary>
        public static async Task<Maybe<R>> MapAsync<T, R>(this Task<Maybe<T>> maybe, Func<T, Task<R>> selector) => await (await maybe).MapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map.
        /// Maps this Maybe to the new Maybe.
        /// If the underlying value is empty, the specified function is not executed and the operation results in an empty Maybe.
        /// Otherwise, the specified function is executed and its result is wrapped in the Maybe indicating the possible absence of returned data.
        /// </summary>
        public static async Task<Maybe<R>> FlatMapAsync<T, R>(this Task<Maybe<T>> maybe, Func<T, Task<Maybe<R>>> selector) => await (await maybe).FlatMapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Creates a Maybe of object.
        /// </summary>
        [Pure]
        public static Maybe<T> AsMaybe<T>(this T item) => Maybe.Create(item);

        /// <summary>
        /// Creates a Maybe of nullable value object.
        /// </summary>
        [Pure]
        public static Maybe<T> AsMaybe<T>(this T? item) where T : struct => Maybe.Create(item);

        /// <summary>
        /// Returns the underlying value if not empty.
        /// Otherwise, returns the result of the specified fallback function.
        /// </summary>
        public static R GetOr<T, R>(this Maybe<T> maybe, Func<Unit, R> selector) where T : R => maybe.Match(_ => selector(Unit.Value), v => v);

        /// <summary>
        /// Returns the underlying value if not empty.
        /// Otherwise, returns the result of the specified fallback function.
        /// </summary>
        public static Task<R> GetOrAsync<T, R>(this Maybe<T> maybe, Func<Unit, Task<R>> selector) where T : R => maybe.ToTask().GetOrAsync(selector);

        /// <summary>
        /// Returns the underlying value if not empty.
        /// Otherwise, returns the result of the specified fallback function.
        /// </summary>
        public static async Task<R> GetOrAsync<T, R>(this Task<Maybe<T>> maybe, Func<Unit, Task<R>> selector) where T : R => await (await maybe).Match(_ => selector(Unit.Value), v => result((R)v)).ConfigureAwait(false);

        /// <summary>
        /// USE GetOr PREFERABLY!
        /// Returns the underlying value if not empty.
        /// Otherwise, returns the default types' value (null for reference types).
        /// </summary>
        [Pure]
        public static T GetOrDefault<T>(this Maybe<T> maybe) => maybe.Match(_ => default, v => v);
        
        /// <summary>
        /// USE GetOrAsync PREFERABLY!
        /// Returns the underlying value if not empty.
        /// Otherwise, returns the default types' value (null for reference types).
        /// </summary>
        public static async Task<T> GetOrDefaultAsync<T>(this Task<Maybe<T>> maybe) => (await maybe).Match(_ => default, v => v);

        /// <summary>
        /// Flattens the nested Maybe object to the single Maybe object. 
        /// </summary>
        [Pure]
        public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> maybe) => maybe.FlatMap(m => m);

        /// <summary>
        /// Resolves and maps the Maybe of the nullable value type to the Maybe of value type.
        /// </summary>
        [Pure]
        public static Maybe<T> Map<T>(this Maybe<T?> maybe) where T : struct => maybe.FlatMap(v => v.AsMaybe());

        /// <summary>
        /// Returns the first non-empty Maybe object.
        /// In case, the first object is not empty, the specified function is not executed.
        /// In case, both objects are empty, the result will be an empty Maybe.
        /// </summary>
        public static Maybe<R> Or<T, R>(this Maybe<T> maybe, Func<Unit, Maybe<R>> ifEmpty) where T : R => maybe.Match(_ => ifEmpty(Unit.Value), v => Maybe.Create((R)v));

        /// <summary>
        /// Returns the first non-empty Maybe object.
        /// In case, the first object is not empty, the specified function is not executed.
        /// In case, both objects are empty, the result will be an empty Maybe.
        /// </summary>
        public static Task<Maybe<R>> OrAsync<T, R>(this Maybe<T> maybe, Func<Unit, Task<Maybe<R>>> ifEmpty) where T : R => maybe.ToTask().OrAsync(ifEmpty);

        /// <summary>
        /// Returns the first non-empty Maybe object.
        /// In case, the first object is not empty, the specified function is not executed.
        /// In case, both objects are empty, the result will be an empty Maybe.
        /// </summary>
        public static async Task<Maybe<R>> OrAsync<T, R>(this Task<Maybe<T>> maybe, Func<Unit, Task<Maybe<R>>> ifEmpty) where T : R => await (await maybe).Match(_ => ifEmpty(Unit.Value), v => Maybe.Create((R)v).ToTask()).ConfigureAwait(false);

        /// <summary>
        /// Creates an Exc from the Maybe object.
        /// In case the object is not empty, the Exc will be in a success state containing the underlying value.
        /// Otherwise, it will contain the exception returned in the specified function.
        /// </summary>
        public static Exc<T, E> ToExc<T, E>(this Maybe<T> maybe, Func<Unit, E> ifEmpty) where E : Exception => maybe.Match(_ => Exc.Failure<T, E>(ifEmpty(Unit.Value)), Exc.Success<T, E>);

        /// <summary>
        /// Creates an Exc from the Maybe object.
        /// In case the object is not empty, the Exc will be in a success state containing the underlying value.
        /// Otherwise, it will contain the exception returned in the specified function.
        /// If the function is not specified, Exc will contain EmptyValueException with a predefined message.
        /// </summary>
        public static Exc<T, EmptyValueException> ToExc<T>(this Maybe<T> maybe, Func<Unit, EmptyValueException> ifEmpty = null) =>
            maybe.Match(_ => Exc.Failure<T, EmptyValueException>(ifEmpty.AsMaybe().Map(f => f(Unit.Value)).GetOr(__ => new EmptyValueException("Maybe value is empty."))), Exc.Success<T, EmptyValueException>);
    }
}
