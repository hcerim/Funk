using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk;

/// <summary>
/// Provides extension methods for working with sequences, including null-safe operations, immutable collections, and functional transformations.
/// </summary>
public static class EnumerableExt
{
    /// <summary>
    /// Pattern-matches on the sequence. Handles null sequence.
    /// In case of more items, performs aggregation.
    /// </summary>
    /// <param name="sequence">The source sequence.</param>
    /// <param name="ifEmpty">The function to execute when the sequence is empty.</param>
    /// <param name="ifSingle">The function to execute when the sequence contains a single element.</param>
    /// <param name="ifMultiple">The aggregation function to execute when the sequence contains multiple elements.</param>
    /// <returns>The result of the matched function.</returns>
    public static R Match<T, R>(this IEnumerable<T> sequence, Func<Unit, R> ifEmpty, Func<T, R> ifSingle, Func<T, T, T> ifMultiple) where T : R
    {
        var list = sequence.Map();
        return list.Count.Match(
            0, _ => ifEmpty(Unit.Value),
            1, _ => ifSingle(list.First()),
            _ => list.Aggregate(ifMultiple)
        );
    }

    extension<T>(IEnumerable<T> sequence)
    {
        /// <summary>
        /// Pattern-matches on the sequence. Handles null sequence.
        /// </summary>
        /// <param name="ifEmpty">The function to execute when the sequence is empty.</param>
        /// <param name="ifSingle">The function to execute when the sequence contains a single element.</param>
        /// <param name="ifMultiple">The function to execute when the sequence contains multiple elements.</param>
        /// <returns>The result of the matched function.</returns>
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T, R> ifSingle, Func<IImmutableList<T>, R> ifMultiple)
        {
            var list = sequence.Map();
            return list.Count.Match(
                0, _ => ifEmpty(Unit.Value),
                1, _ => ifSingle(list.First()),
                _ => ifMultiple(list)
            );
        }

        /// <summary>
        /// Pattern-matches on the sequence. Handles null sequence.
        /// </summary>
        /// <param name="ifEmpty">The function to execute when the sequence is empty.</param>
        /// <param name="ifNotEmpty">The function to execute when the sequence is not empty, receiving the first element and the rest.</param>
        /// <returns>The result of the matched function.</returns>
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T, IImmutableList<T>, R> ifNotEmpty)
        {
            var list = sequence.Map();
            return list.Count.Match(
                0, _ => ifEmpty(Unit.Value),
                _ => ifNotEmpty(list.First(), list.Skip(1).Map())
            );
        }

        /// <summary>
        /// Returns Maybe of immutable list of items that can be safely converted to the specified type.
        /// </summary>
        /// <typeparam name="R">The target type to cast elements to.</typeparam>
        /// <returns>A Maybe containing an immutable list of elements successfully cast to the specified type, or an empty Maybe if none match.</returns>
        public Maybe<IImmutableList<R>> OfSafeType<R>() => sequence.Map(i => i.SafeCast<R>()).Flatten().AsNotEmptyList();
    }

    /// <summary>
    /// Creates an immutable sequence of item if not null. Otherwise returns empty immutable sequence.
    /// </summary>
    /// <returns>An immutable list containing the item, or an empty list if the item is null.</returns>
    public static IImmutableList<T> ToImmutableList<T>(this T item) => item.AsMaybe().AsImmutableList();

    /// <summary>
    /// Creates an immutable sequence out of Maybe value if not empty. Otherwise, it returns empty immutable sequence.
    /// </summary>
    /// <returns>An immutable list containing the Maybe value if not empty, or an empty list.</returns>
    public static IImmutableList<T> AsImmutableList<T>(this Maybe<T> maybe) => maybe.Match(_ => list<T>(), ImmutableList.Create);

    /// <summary>
    /// Creates an immutable sequence from sequence. Handles null sequence.
    /// </summary>
    /// <returns>An immutable list containing the elements of the sequence, or an empty list if the sequence is null.</returns>
    public static IImmutableList<T> Map<T>(this IEnumerable<T> enumerable) => ImmutableList.CreateRange(enumerable ?? list<T>());

    /// <summary>
    /// Returns an immutable sequence of not null values. Handles null sequence.
    /// </summary>
    /// <returns>An immutable list with null values removed.</returns>
    public static IImmutableList<T> ExceptNulls<T>(this IEnumerable<T> enumerable) where T : class => enumerable.FlatMap(i => i.AsMaybe().AsImmutableList());

    /// <summary>
    /// Returns an immutable sequence of not null values. Handles null sequence.
    /// </summary>
    /// <returns>An immutable list with null values removed.</returns>
    public static IImmutableList<T> ExceptNulls<T>(this IEnumerable<T?> enumerable) where T : struct => enumerable.FlatMap(i => i.AsMaybe().AsImmutableList());

    /// <summary>
    /// Flattens sequence of not empty Maybes into an immutable sequence. Handles null sequence.
    /// </summary>
    /// <returns>An immutable list containing the values of all non-empty Maybes.</returns>
    public static IImmutableList<T> Flatten<T>(this IEnumerable<Maybe<T>> enumerable) => enumerable.FlatMap(m => m.AsImmutableList());

    /// <summary>
    /// Flattens sequence of sequences into an immutable sequence. Handles null sequence and null children and their children.
    /// </summary>
    /// <returns>An immutable list containing all non-null elements from all nested sequences.</returns>
    public static IImmutableList<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) where T : class => enumerable.FlatMap(i => i.ExceptNulls());

    /// <summary>
    /// Flattens sequence of sequences into an immutable sequence. Handles null sequence and null children and their children.
    /// </summary>
    /// <returns>An immutable list containing all non-null elements from all nested sequences.</returns>
    public static IImmutableList<T> Flatten<T>(this IEnumerable<IEnumerable<T?>> enumerable) where T : struct => enumerable.FlatMap(i => i.ExceptNulls());

    extension<T>(Maybe<T> maybe)
    {
        /// <summary>
        /// Merges not empty Maybe with not empty Maybes collection into an immutable sequence. Handles null sequence.
        /// </summary>
        /// <param name="enumerable">The sequence of Maybes to merge with.</param>
        /// <returns>An immutable list containing all non-empty Maybe values including the receiver.</returns>
        public IImmutableList<T> FlatMerge(IEnumerable<Maybe<T>> enumerable) => Flatten(new[] { maybe }.Concat(enumerable.Map()));

        /// <summary>
        /// Merges not empty Maybe with other not empty Maybes into an immutable sequence.
        /// </summary>
        /// <param name="maybes">The Maybes to merge with.</param>
        /// <returns>An immutable list containing all non-empty Maybe values including the receiver.</returns>
        public IImmutableList<T> FlatMerge(params Maybe<T>[] maybes) => maybe.FlatMerge(maybes.Map());
    }

    extension<T>(IEnumerable<T> enumerable)
    {
        /// <summary>
        /// Returns a Maybe of an immutable sequence if sequence not empty or null. Otherwise, it returns an empty Maybe. Handles null sequence.
        /// </summary>
        /// <returns>A Maybe containing the immutable list if non-empty, or an empty Maybe.</returns>
        public Maybe<IImmutableList<T>> AsNotEmptyList()
        {
            var collection = enumerable.Map();
            return collection.NotEmpty() ? collection.AsMaybe() : empty;
        }

        /// <summary>
        /// Returns a Maybe of the first element in the sequence that satisfies the condition or returns an empty Maybe. Handles null sequence.
        /// </summary>
        /// <param name="predicate">The condition to match. If null, the first element is returned.</param>
        /// <returns>A Maybe containing the first matching element, or an empty Maybe.</returns>
        public Maybe<T> AsFirstOrDefault(Func<T, bool> predicate = null) => enumerable.WhereOrDefault(predicate).Map(c => c.First());

        /// <summary>
        /// Returns a Maybe of an immutable sequence of items that satisfy the predicate or returns an empty Maybe. Handles null sequence.
        /// </summary>
        /// <param name="predicate">The condition to match. If null, all elements are included.</param>
        /// <returns>A Maybe containing an immutable list of matching elements, or an empty Maybe.</returns>
        public Maybe<IImmutableList<T>> WhereOrDefault(Func<T, bool> predicate = null) => enumerable.Map().Where(predicate ?? (i => true)).AsNotEmptyList();

        /// <summary>
        /// Returns a Maybe of the last element in the sequence that satisfies the condition or returns an empty Maybe. Handles null sequence.
        /// </summary>
        /// <param name="predicate">The condition to match. If null, the last element is returned.</param>
        /// <returns>A Maybe containing the last matching element, or an empty Maybe.</returns>
        public Maybe<T> AsLastOrDefault(Func<T, bool> predicate = null) => enumerable.Map().Reverse().AsFirstOrDefault(predicate);

        /// <summary>
        /// Checks whether a given sequence is empty. Handles null sequence.
        /// </summary>
        /// <returns>true if the sequence is empty or null; otherwise, false.</returns>
        public bool IsEmpty() => enumerable.Map().Count.SafeEquals(0);

        /// <summary>
        /// Checks whether a given sequence is not empty or null. Handles null sequence.
        /// </summary>
        /// <returns>true if the sequence contains at least one element; otherwise, false.</returns>
        public bool NotEmpty() => !enumerable.IsEmpty();
    }

    /// <summary>
    /// Checks whether a given string is null or empty and returns a Maybe of that string if it is not. Otherwise, it returns empty Maybe.
    /// </summary>
    /// <param name="item">The string to check.</param>
    /// <returns>A Maybe containing the string if not null or empty, or an empty Maybe.</returns>
    public static Maybe<string> AsNotEmptyString(this string item) => string.IsNullOrEmpty(item) ? empty : item.AsMaybe();

    extension<T>(IEnumerable<T> enumerable)
    {
        /// <summary>
        /// Returns a record of 2 immutable sequences where first sequence satisfies the predicate and second doesn't. Handles null sequence.
        /// </summary>
        /// <param name="predicate">The condition used to split the sequence.</param>
        /// <returns>A record where the first element contains items satisfying the predicate and the second contains the rest.</returns>
        public Record<IImmutableList<T>, IImmutableList<T>> ConditionalSplit(Func<T, bool> predicate)
        {
            var collection = enumerable.Map();
            var left = collection.Where(predicate).Map();
            var right = collection.Where(i => !predicate(i)).Map();
            return Record.Create(left, right);
        }

        /// <summary>
        /// Returns unique items specified by a selector.
        /// </summary>
        /// <param name="selector">The function to extract the key used to determine uniqueness.</param>
        /// <returns>An immutable list containing distinct elements based on the specified key.</returns>
        public IImmutableList<T> DistinctBy<TKey>(Func<T, TKey> selector) =>
            enumerable.Map().GroupBy(selector).Select(g => g.First()).Map();

        /// <summary>
        /// Returns unique items specified by a selector.
        /// </summary>
        /// <param name="selector">The function to extract the key used to determine uniqueness.</param>
        /// <param name="comparer">The equality comparer to use when comparing keys.</param>
        /// <returns>An immutable list containing distinct elements based on the specified key and comparer.</returns>
        public IImmutableList<T> DistinctBy<TKey>(Func<T, TKey> selector, IEqualityComparer<TKey> comparer) =>
            enumerable.Map().GroupBy(selector, comparer).Select(g => g.First()).Map();

        /// <summary>
        /// Creates an immutable dictionary of key as a discriminator and corresponding immutable sequence. Handles null sequence.
        /// </summary>
        /// <param name="keySelector">The function to extract the key from each element.</param>
        /// <returns>An immutable dictionary mapping each key to its corresponding immutable list of elements.</returns>
        public IImmutableDictionary<TKey, IImmutableList<T>> ToDictionary<TKey>(Func<T, TKey> keySelector)
        {
            return enumerable.Map().GroupBy(keySelector).ToImmutableDictionary(i => i.Key, i => i.Map());
        }

        /// <summary>
        /// Creates an immutable sequence of records of key as a discriminator and corresponding enumerable. Handles null sequence.
        /// </summary>
        /// <param name="keySelector">The function to extract the key from each element.</param>
        /// <returns>An immutable list of records containing grouped keys and their corresponding items.</returns>
        public IImmutableList<Record<TKey, IImmutableList<T>>> ToRecordList<TKey>(Func<T, TKey> keySelector)
        {
            return enumerable.ToDictionary(keySelector).Select(i => Record.Create(i.Key, i.Value)).Map();
        }
    }

    /// <summary>
    /// Concatenates 2 sequences with null removal into an immutable sequence. Handles null sequences.
    /// </summary>
    /// <param name="first">The first sequence.</param>
    /// <param name="second">The second sequence.</param>
    /// <returns>An immutable list containing elements from both sequences with nulls removed.</returns>
    public static IImmutableList<T> SafeConcat<T>(this IEnumerable<T> first, IEnumerable<T> second) where T : class => first.ExceptNulls().Concat(second.ExceptNulls()).Map();

    /// <summary>
    /// Concatenates 2 sequences with null removal into an immutable sequence. Handles null sequences.
    /// </summary>
    /// <param name="first">The first sequence.</param>
    /// <param name="second">The second sequence.</param>
    /// <returns>An immutable list containing elements from both sequences with nulls removed.</returns>
    public static IImmutableList<T> SafeConcat<T>(this IEnumerable<T?> first, IEnumerable<T?> second) where T : struct => first.ExceptNulls().Concat(second.ExceptNulls()).Map();

    extension<T>(IEnumerable<T> enumerable)
    {
        /// <summary>
        /// Maps the specified sequence to an immutable sequence of specified type. Handles null sequence.
        /// </summary>
        /// <param name="mapper">The function to transform each element.</param>
        /// <returns>An immutable list containing the transformed elements.</returns>
        public IImmutableList<R> Map<R>(Func<T, R> mapper) => enumerable.Map().Select(mapper).Map();

        /// <summary>
        /// Maps the specified sequence to an immutable sequence of specified type. Handles null sequence.
        /// </summary>
        /// <param name="mapper">The function to transform each element into a sequence.</param>
        /// <returns>An immutable list containing the flattened transformed elements.</returns>
        public IImmutableList<R> FlatMap<R>(Func<T, IEnumerable<R>> mapper) => enumerable.Map().SelectMany(mapper).Map();

        /// <summary>
        /// Aggregates sequence to the specified result as Maybe. Handles null sequence.
        /// Use Fold if you have sequence of Maybes.
        /// </summary>
        /// <param name="reducer">The aggregation function to apply to each pair of elements.</param>
        /// <returns>A Maybe containing the aggregated result, or an empty Maybe if the sequence is empty.</returns>
        public Maybe<T> Reduce(Func<T, T, T> reducer) => enumerable.AsNotEmptyList().Map(e => e.Aggregate(reducer));
    }

    /// <summary>
    /// Aggregates sequence of not empty Maybes to the specified result as Maybe. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The source sequence of Maybe values.</param>
    /// <param name="reducer">The aggregation function to apply to each pair of elements.</param>
    /// <returns>A Maybe containing the aggregated result, or an empty Maybe if no non-empty Maybes exist.</returns>
    public static Maybe<T> Fold<T>(this IEnumerable<Maybe<T>> enumerable, Func<T, T, T> reducer) => enumerable.Flatten().Reduce(reducer);

    /// <summary>
    /// Maps the specified sequence to an immutable sequence of specified type and aggregates sequence of the new type to the specified result as Maybe. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The source sequence.</param>
    /// <param name="mapper">The function to transform each element.</param>
    /// <param name="reducer">The aggregation function to apply to each pair of transformed elements.</param>
    /// <returns>A Maybe containing the aggregated result of the mapped sequence, or an empty Maybe if the sequence is empty.</returns>
    public static Maybe<R> MapReduce<T, R>(this IEnumerable<T> enumerable, Func<T, R> mapper, Func<R, R, R> reducer) => enumerable.Map(mapper).Reduce(reducer);

    /// <summary>
    /// Maps the specified sequence to an immutable sequence of not empty maybes and aggregates sequence of the new type to the specified result as Maybe. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The source sequence of Maybe values.</param>
    /// <param name="mapper">The function to transform each element.</param>
    /// <param name="reducer">The aggregation function to apply to each pair of transformed elements.</param>
    /// <returns>A Maybe containing the aggregated result of the mapped flattened sequence, or an empty Maybe if empty.</returns>
    public static Maybe<R> MapFold<T, R>(this IEnumerable<Maybe<T>> enumerable, Func<T, R> mapper, Func<R, R, R> reducer) => enumerable.Flatten().Map(mapper).Reduce(reducer);

    /// <summary>
    /// Maps the specified sequence to an immutable sequence of specified type and aggregates sequence of the new type to the specified result as Maybe. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The source sequence.</param>
    /// <param name="mapper">The function to transform each element into a sequence.</param>
    /// <param name="reducer">The aggregation function to apply to each pair of transformed elements.</param>
    /// <returns>A Maybe containing the aggregated result of the flat-mapped sequence, or an empty Maybe if the sequence is empty.</returns>
    public static Maybe<R> FlatMapReduce<T, R>(this IEnumerable<T> enumerable, Func<T, IEnumerable<R>> mapper, Func<R, R, R> reducer) => enumerable.FlatMap(mapper).Reduce(reducer);

    /// <summary>
    /// Maps the specified sequence of not empty maybes to an immutable sequence of specified type and aggregates sequence of the new type to the specified result as Maybe. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The source sequence of Maybe values.</param>
    /// <param name="mapper">The function to transform each element into a sequence.</param>
    /// <param name="reducer">The aggregation function to apply to each pair of transformed elements.</param>
    /// <returns>A Maybe containing the aggregated result of the flat-mapped flattened sequence, or an empty Maybe if empty.</returns>
    public static Maybe<R> FlatMapFold<T, R>(this IEnumerable<Maybe<T>> enumerable, Func<T, IEnumerable<R>> mapper, Func<R, R, R> reducer) => enumerable.Flatten().FlatMap(mapper).Reduce(reducer);

    extension<T>(IEnumerable<T> enumerable)
    {
        /// <summary>
        /// Executes specified operation on items from sequence and return successful Exc (represented by unit) if all operations are successful. Otherwise, return failure. Handles null enumerable
        /// </summary>
        /// <param name="operation">The action to execute on each element.</param>
        /// <returns>A successful Exc of Unit if all operations succeed; otherwise, a failed Exc containing the exception.</returns>
        public Exc<Unit, E> ForEach<E>(Action<T> operation) where E : Exception
        {
            return Exc.Create<Unit, E>(_ =>
            {
                enumerable.Map().ToList().ForEach(operation);
                return Unit.Value;
            });
        }

        /// <summary>
        /// Executes specified operation on items from sequence and return successful Exc (represented by unit) if all operations are successful. Otherwise, return failure. Handles null enumerable
        /// </summary>
        /// <param name="operation">The asynchronous action to execute on each element.</param>
        /// <returns>A Task containing a successful Exc of Unit if all operations succeed; otherwise, a failed Exc containing the exception.</returns>
        public Task<Exc<Unit, E>> ForEachAsync<E>(Func<T, Task> operation) where E : Exception
        {
            return Exc.CreateAsync<Unit, E>(async _ =>
            {
                foreach (var item in enumerable)
                {
                    await operation(item).ConfigureAwait(false);
                }
                return Unit.Value;
            });
        }
    }
}
