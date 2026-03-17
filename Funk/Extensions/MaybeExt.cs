using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk;

/// <summary>
/// Provides extension methods for the Maybe type.
/// </summary>
public static class MaybeExt
{
    extension<T>(Task<Maybe<T>> maybe)
    {
        /// <summary>
        /// Structure-preserving map.
        /// Maps this Maybe to the new Maybe.
        /// If the underlying value is empty, the specified function is not executed and the operation results in an empty Maybe.
        /// Otherwise, the specified function is executed and its result is wrapped in the Maybe indicating the possible absence of returned data.
        /// In case the specified function already returns the Maybe, use FlatMapAsync instead.
        /// </summary>
        /// <typeparam name="R">The type of the mapped value.</typeparam>
        /// <param name="selector">The async mapping function.</param>
        /// <returns>A task containing a Maybe with the mapped value, or an empty Maybe.</returns>
        public async Task<Maybe<R>> MapAsync<R>(Func<T, Task<R>> selector) => await (await maybe.ConfigureAwait(false)).MapAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map.
        /// Maps this Maybe to the new Maybe.
        /// If the underlying value is empty, the specified function is not executed and the operation results in an empty Maybe.
        /// Otherwise, the specified function is executed and its result is wrapped in the Maybe indicating the possible absence of returned data.
        /// </summary>
        /// <typeparam name="R">The type of the mapped value.</typeparam>
        /// <param name="selector">The async mapping function returning a Maybe.</param>
        /// <returns>A task containing a Maybe with the mapped value, or an empty Maybe.</returns>
        public async Task<Maybe<R>> FlatMapAsync<R>(Func<T, Task<Maybe<R>>> selector) => await (await maybe.ConfigureAwait(false)).FlatMapAsync(selector).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a Maybe of object.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="item">The value to wrap.</param>
    /// <returns>A Maybe containing the value if not null, or an empty Maybe.</returns>
    [Pure]
    public static Maybe<T> AsMaybe<T>(this T item) => Maybe.Create(item);

    /// <summary>
    /// Creates a Maybe of nullable value object.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <param name="item">The nullable value to wrap.</param>
    /// <returns>A Maybe containing the value if not null, or an empty Maybe.</returns>
    [Pure]
    public static Maybe<T> AsMaybe<T>(this T? item) where T : struct => Maybe.Create(item);

    /// <summary>
    /// Returns the underlying value if not empty.
    /// Otherwise, returns the result of the specified fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the Maybe value.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="maybe">The Maybe value.</param>
    /// <param name="selector">The fallback function to execute if the value is empty.</param>
    /// <returns>The underlying value or the result of the fallback function.</returns>
    public static R GetOr<T, R>(this Maybe<T> maybe, Func<Unit, R> selector) where T : R => maybe.Match(_ => selector(Unit.Value), v => v);

    /// <summary>
    /// Returns the underlying value if not empty.
    /// Otherwise, returns the result of the specified fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the Maybe value.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="maybe">The Maybe value.</param>
    /// <param name="selector">The async fallback function to execute if the value is empty.</param>
    /// <returns>A task containing the underlying value or the result of the fallback function.</returns>
    public static Task<R> GetOrAsync<T, R>(this Maybe<T> maybe, Func<Unit, Task<R>> selector) where T : R => maybe.ToTask().GetOrAsync(selector);

    /// <summary>
    /// Returns the underlying value if not empty.
    /// Otherwise, returns the result of the specified fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the Maybe value.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="maybe">The Task of Maybe value.</param>
    /// <param name="selector">The async fallback function to execute if the value is empty.</param>
    /// <returns>A task containing the underlying value or the result of the fallback function.</returns>
    public static async Task<R> GetOrAsync<T, R>(this Task<Maybe<T>> maybe, Func<Unit, Task<R>> selector) where T : R => await (await maybe.ConfigureAwait(false)).Match(_ => selector(Unit.Value), v => result((R)v)).ConfigureAwait(false);

    /// <summary>
    /// USE GetOr PREFERABLY!
    /// Returns the underlying value if not empty.
    /// Otherwise, returns the default types' value (null for reference types).
    /// </summary>
    /// <returns>The underlying value or the default value for the type.</returns>
    [Pure]
    public static T GetOrDefault<T>(this Maybe<T> maybe) => maybe.Match(_ => default, v => v);
        
    /// <summary>
    /// USE GetOrAsync PREFERABLY!
    /// Returns the underlying value if not empty.
    /// Otherwise, returns the default types' value (null for reference types).
    /// </summary>
    /// <returns>A task containing the underlying value or the default value for the type.</returns>
    public static async Task<T> GetOrDefaultAsync<T>(this Task<Maybe<T>> maybe) => (await maybe.ConfigureAwait(false)).Match(_ => default, v => v);

    /// <summary>
    /// Flattens the nested Maybe object to the single Maybe object. 
    /// </summary>
    /// <returns>A flattened Maybe.</returns>
    [Pure]
    public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> maybe) => maybe.FlatMap(m => m);

    /// <summary>
    /// Resolves and maps the Maybe of the nullable value type to the Maybe of value type.
    /// </summary>
    /// <returns>A Maybe containing the non-nullable value, or an empty Maybe.</returns>
    [Pure]
    public static Maybe<T> Map<T>(this Maybe<T?> maybe) where T : struct => maybe.FlatMap(v => v.AsMaybe());

    /// <summary>
    /// Returns the first non-empty Maybe object.
    /// In case, the first object is not empty, the specified function is not executed.
    /// In case, both objects are empty, the result will be an empty Maybe.
    /// </summary>
    /// <typeparam name="T">The type of the source Maybe value.</typeparam>
    /// <typeparam name="R">The type of the result Maybe value.</typeparam>
    /// <param name="maybe">The Maybe value.</param>
    /// <param name="ifEmpty">The fallback function to execute if the value is empty.</param>
    /// <returns>The first non-empty Maybe, or an empty Maybe.</returns>
    public static Maybe<R> Or<T, R>(this Maybe<T> maybe, Func<Unit, Maybe<R>> ifEmpty) where T : R => maybe.Match(_ => ifEmpty(Unit.Value), v => Maybe.Create((R)v));

    /// <summary>
    /// Returns the first non-empty Maybe object.
    /// In case, the first object is not empty, the specified function is not executed.
    /// In case, both objects are empty, the result will be an empty Maybe.
    /// </summary>
    /// <typeparam name="T">The type of the source Maybe value.</typeparam>
    /// <typeparam name="R">The type of the result Maybe value.</typeparam>
    /// <param name="maybe">The Maybe value.</param>
    /// <param name="ifEmpty">The async fallback function to execute if the value is empty.</param>
    /// <returns>A task containing the first non-empty Maybe, or an empty Maybe.</returns>
    public static Task<Maybe<R>> OrAsync<T, R>(this Maybe<T> maybe, Func<Unit, Task<Maybe<R>>> ifEmpty) where T : R => maybe.ToTask().OrAsync(ifEmpty);

    /// <summary>
    /// Returns the first non-empty Maybe object.
    /// In case, the first object is not empty, the specified function is not executed.
    /// In case, both objects are empty, the result will be an empty Maybe.
    /// </summary>
    /// <typeparam name="T">The type of the source Maybe value.</typeparam>
    /// <typeparam name="R">The type of the result Maybe value.</typeparam>
    /// <param name="maybe">The Task of Maybe value.</param>
    /// <param name="ifEmpty">The async fallback function to execute if the value is empty.</param>
    /// <returns>A task containing the first non-empty Maybe, or an empty Maybe.</returns>
    public static async Task<Maybe<R>> OrAsync<T, R>(this Task<Maybe<T>> maybe, Func<Unit, Task<Maybe<R>>> ifEmpty) where T : R => await (await maybe.ConfigureAwait(false)).Match(_ => ifEmpty(Unit.Value), v => Maybe.Create((R)v).ToTask()).ConfigureAwait(false);

    extension<T>(Maybe<T> first)
    {
        /// <summary>
        /// Aggregates Maybe with another Maybe. If both are not empty the result will be a non-empty Maybe of an immutable list of results.
        /// If any is empty, the result will be an empty Maybe.
        /// </summary>
        /// <param name="second">The other Maybe to merge with.</param>
        /// <returns>A Maybe containing a list of values, or an empty Maybe if any is empty.</returns>
        public Maybe<IImmutableList<T>> Merge(Maybe<T> second) => first.MergeRange(ImmutableList.Create(second));

        /// <summary>
        /// Aggregates Maybe with the sequence of Maybes. If all are not empty the result will be a non-empty Maybe of an immutable list of results.
        /// If any is empty, the result will be an empty Maybe.
        /// </summary>
        /// <param name="items">The sequence of Maybe values to merge with.</param>
        /// <returns>A Maybe containing a list of values, or an empty Maybe if any is empty.</returns>
        public Maybe<IImmutableList<T>> MergeRange(IEnumerable<Maybe<T>> items)
        {
            var all = ImmutableList.Create(first).AddRange(items.Map());
            return all.Any(m => m.IsEmpty) ? Maybe.Empty<IImmutableList<T>>() : Maybe.Create(all.Flatten());
        }
    }

    extension<T>(Maybe<T> maybe)
    {
        /// <summary>
        /// Creates an Exc from the Maybe object.
        /// In case the object is not empty, the Exc will be in a success state containing the underlying value.
        /// Otherwise, it will contain the exception returned in the specified function.
        /// </summary>
        /// <typeparam name="E">The type of the exception.</typeparam>
        /// <param name="ifEmpty">The function returning the exception if the value is empty.</param>
        /// <returns>An Exc in a success or failure state.</returns>
        public Exc<T, E> ToExc<E>(Func<Unit, E> ifEmpty) where E : Exception => maybe.Match(_ => Exc.Failure<T, E>(ifEmpty(Unit.Value)), Exc.Success<T, E>);

        /// <summary>
        /// Creates an Exc from the Maybe object.
        /// In case the object is not empty, the Exc will be in a success state containing the underlying value.
        /// Otherwise, it will contain the exception returned in the specified function.
        /// If the function is not specified, Exc will contain EmptyValueException with a predefined message.
        /// </summary>
        /// <param name="ifEmpty">Optional function returning the exception if the value is empty.</param>
        /// <returns>An Exc in a success or failure state.</returns>
        public Exc<T, EmptyValueException> ToExc(Func<Unit, EmptyValueException> ifEmpty = null) =>
            maybe.Match(_ => Exc.Failure<T, EmptyValueException>(ifEmpty.AsMaybe().Map(f => f(Unit.Value)).GetOr(__ => new EmptyValueException("Maybe value is empty."))), Exc.Success<T, EmptyValueException>);
    }
}