using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    public static class EnumerableExt
    {
        /// <summary>
        /// Returns Maybe of immutable list of items that can be safely converted to the specified type.
        /// </summary>
        public static Maybe<IImmutableList<R>> OfSafeType<T, R>(this IEnumerable<T> enumerable) => enumerable.Map(i => i.SafeCast<R>()).Flatten().AsNotEmptyList();

        /// <summary>
        /// Creates an immutable sequence of item if not null. Otherwise returns empty immutable sequence.
        /// </summary>
        public static IImmutableList<T> ToImmutableList<T>(this T item) => item.AsMaybe().AsImmutableList();

        /// <summary>
        /// Creates an immutable sequence out of Maybe value if not empty. Otherwise, it returns empty immutable sequence.
        /// </summary>
        public static IImmutableList<T> AsImmutableList<T>(this Maybe<T> maybe) => maybe.Match(_ => list<T>(), ImmutableList.Create);

        /// <summary>
        /// Creates an immutable sequence from sequence. Handles null sequence.
        /// </summary>
        public static IImmutableList<T> Map<T>(this IEnumerable<T> enumerable) => ImmutableList.CreateRange(enumerable ?? list<T>());

        /// <summary>
        /// Returns an immutable sequence of not null values. Handles null sequence.
        /// </summary>
        public static IImmutableList<T> ExceptNulls<T>(this IEnumerable<T> enumerable) where T : class => enumerable.FlatMap(i => i.AsMaybe().AsImmutableList());

        /// <summary>
        /// Returns an immutable sequence of not null values. Handles null sequence.
        /// </summary>
        public static IImmutableList<T> ExceptNulls<T>(this IEnumerable<T?> enumerable) where T : struct => enumerable.FlatMap(i => i.AsMaybe().AsImmutableList());

        /// <summary>
        /// Flattens sequence of not empty Maybes into an immutable sequence. Handles null sequence.
        /// </summary>
        public static IImmutableList<T> Flatten<T>(this IEnumerable<Maybe<T>> enumerable) => enumerable.FlatMap(m => m.AsImmutableList());

        /// <summary>
        /// Flattens sequence of sequences into an immutable sequence. Handles null sequence and null children and their children.
        /// </summary>
        public static IImmutableList<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) where T : class => enumerable.FlatMap(i => i.ExceptNulls());

        /// <summary>
        /// Flattens sequence of sequences into an immutable sequence. Handles null sequence and null children and their children.
        /// </summary>
        public static IImmutableList<T> Flatten<T>(this IEnumerable<IEnumerable<T?>> enumerable) where T : struct => enumerable.FlatMap(i => i.ExceptNulls());

        /// <summary>
        /// Returns an immutable sequence of not empty Maybes.
        /// </summary>
        public static IImmutableList<T> Flatten<T>(params Maybe<T>[] maybes) => Flatten(maybes.Map());

        /// <summary>
        /// Merges not empty Maybe with not empty Maybes collection into an immutable sequence. Handles null sequence.
        /// </summary>
        public static IImmutableList<T> FlatMerge<T>(this Maybe<T> maybe, IEnumerable<Maybe<T>> enumerable) => Flatten(new[] { maybe }.Concat(enumerable.Map()));

        /// <summary>
        /// Merges not empty Maybe with other not empty Maybes into an immutable sequence.
        /// </summary>
        public static IImmutableList<T> FlatMerge<T>(this Maybe<T> maybe, params Maybe<T>[] maybes) => maybe.FlatMerge(maybes.Map());

        /// <summary>
        /// Returns a Maybe of an immutable sequence if sequence not empty or null. Otherwise, it returns an empty Maybe. Handles null sequence.
        /// </summary>
        public static Maybe<IImmutableList<T>> AsNotEmptyList<T>(this IEnumerable<T> enumerable)
        {
            var collection = enumerable.Map();
            return collection.NotEmpty() ? collection.AsMaybe() : empty;
        }

        /// <summary>
        /// Returns a Maybe of the first element in the sequence that satisfies the condition or returns an empty Maybe. Handles null sequence.
        /// </summary>
        public static Maybe<T> AsFirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate = null) => enumerable.WhereOrDefault(predicate).Map(c => c.First());

        /// <summary>
        /// Returns a Maybe of an immutable sequence of items that satisfy the predicate or returns an empty Maybe. Handles null sequence.
        /// </summary>
        public static Maybe<IImmutableList<T>> WhereOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate = null) => enumerable.Map().Where(predicate ?? (i => true)).AsNotEmptyList();

        /// <summary>
        /// Returns a Maybe of the last element in the sequence that satisfies the condition or returns an empty Maybe. Handles null sequence.
        /// </summary>
        public static Maybe<T> AsLastOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate = null) => enumerable.Map().Reverse().AsFirstOrDefault(predicate);

        /// <summary>
        /// Checks whether a given sequence is empty. Handles null sequence.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable.Map().Count.SafeEquals(0);

        /// <summary>
        /// Checks whether a given sequence is not empty or null. Handles null sequence.
        /// </summary>
        public static bool NotEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.IsEmpty();

        /// <summary>
        /// Checks whether a given string is null or empty and returns a Maybe of that string if it is not. Otherwise, it returns empty Maybe.
        /// </summary>
        public static Maybe<string> AsNotEmptyString(this string item) => string.IsNullOrEmpty(item) ? empty : item.AsMaybe();

        /// <summary>
        /// Returns a record of 2 immutable sequences where first sequence satisfies the predicate and second doesn't. Handles null sequence.
        /// </summary>
        public static Record<IImmutableList<T>, IImmutableList<T>> ConditionalSplit<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var collection = enumerable.Map();
            var left = collection.Where(predicate).Map();
            var right = collection.Except(left).Map();
            return Record.Create(left, right);
        }

        /// <summary>
        /// Creates an immutable dictionary of key as a discriminator and corresponding immutable sequence. Handles null sequence.
        /// </summary>
        public static IImmutableDictionary<TKey, IImmutableList<TSource>> ToDictionary<TSource, TKey>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> keySelector)
        {
            return enumerable.Map().GroupBy(keySelector).ToImmutableDictionary(i => i.Key, i => i.Map());
        }

        /// <summary>
        /// Creates an immutable sequence of records of key as a discriminator and corresponding enumerable. Handles null sequence.
        /// </summary>
        /// <returns></returns>
        public static IImmutableList<Record<TKey, IImmutableList<TSource>>> ToRecordList<TSource, TKey>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> keySelector)
        {
            return enumerable.ToDictionary(keySelector).Select(i => Record.Create(i.Key, i.Value)).Map();
        }

        /// <summary>
        /// Concatenates 2 sequences with null removal into an immutable sequence. Handles null sequences.
        /// </summary>
        /// <returns></returns>
        public static IImmutableList<T> SafeConcat<T>(this IEnumerable<T> first, IEnumerable<T> second) where T : class => first.ExceptNulls().Concat(second.ExceptNulls()).Map();

        /// <summary>
        /// Concatenates 2 sequences with null removal into an immutable sequence. Handles null sequences.
        /// </summary>
        /// <returns></returns>
        public static IImmutableList<T> SafeConcat<T>(this IEnumerable<T?> first, IEnumerable<T?> second) where T : struct => first.ExceptNulls().Concat(second.ExceptNulls()).Map();

        /// <summary>
        /// Maps the specified sequence to an immutable sequence of specified type. Handles null sequence.
        /// </summary>
        public static IImmutableList<R> Map<T, R>(this IEnumerable<T> enumerable, Func<T, R> mapper) => enumerable.Map().Select(mapper).Map();

        /// <summary>
        /// Maps the specified sequence to an immutable sequence of specified type. Handles null sequence.
        /// </summary>
        public static IImmutableList<R> FlatMap<T, R>(this IEnumerable<T> enumerable, Func<T, IEnumerable<R>> mapper) => enumerable.Map().SelectMany(mapper).Map();

        /// <summary>
        /// Aggregates sequence to the specified result as Maybe. Handles null sequence.
        /// Use Fold if you have sequence of Maybes.
        /// </summary>
        public static Maybe<T> Reduce<T>(this IEnumerable<T> enumerable, Func<T, T, T> reducer) => enumerable.AsNotEmptyList().Map(e => e.Aggregate(reducer));

        /// <summary>
        /// Aggregates sequence of not empty Maybes to the specified result as Maybe. Handles null sequence.
        /// </summary>
        public static Maybe<T> Fold<T>(this IEnumerable<Maybe<T>> enumerable, Func<T, T, T> reducer) => enumerable.Flatten().Reduce(reducer);

        /// <summary>
        /// Maps the specified sequence to an immutable sequence of specified type and aggregates sequence of the new type to the specified result as Maybe. Handles null sequence.
        /// </summary>
        public static Maybe<R> MapReduce<T, R>(this IEnumerable<T> enumerable, Func<T, R> mapper, Func<R, R, R> reducer) => enumerable.Map(mapper).Reduce(reducer);

        /// <summary>
        /// Maps the specified sequence to an immutable sequence of specified type and aggregates sequence of the new type to the specified result as Maybe. Handles null sequence.
        /// </summary>
        public static Maybe<R> FlatMapReduce<T, R>(this IEnumerable<T> enumerable, Func<T, IEnumerable<R>> mapper, Func<R, R, R> reducer) => enumerable.FlatMap(mapper).Reduce(reducer);

        /// <summary>
        /// Executes specified operation on items from sequence and return successful Exc (represented by unit) if all operations are successful. Otherwise, return failure. Handles null enumerable
        /// </summary>
        public static Exc<Unit, E> ForEach<T, E>(this IEnumerable<T> enumerable, Action<T> operation) where E : Exception
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
        public static Task<Exc<Unit, E>> ForEachAsync<T, E>(this IEnumerable<T> enumerable, Func<T, Task> operation) where E : Exception
        {
            return Exc.CreateAsync<Unit, E>(async _ =>
            {
                foreach (var item in enumerable)
                {
                    await operation(item);
                }
                return Unit.Value;
            });
        }
    }
}
