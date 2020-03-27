﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static Funk.Prelude;

namespace Funk
{
    public static class EnumerableExt
    {
        /// <summary>
        /// Creates a collection out of Maybe value if not empty. Otherwise, it returns an empty collection.
        /// </summary>
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this Maybe<T> maybe) => maybe.Match(_ => ImmutableList<T>.Empty, ImmutableList.Create);

        /// <summary>
        /// Creates a read only collection of enumerable. Handles null enumerable.
        /// </summary>
        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return ImmutableList.CreateRange(enumerable ?? ImmutableList.Create<T>());
        }

        /// <summary>
        /// Returns a collection of not null values. Handles null enumerable.
        /// </summary>
        public static IReadOnlyCollection<T> ExceptNulls<T>(this IEnumerable<T> enumerable) where T : class
        {
            return enumerable.ToNotEmptyCollection().Match(
                _ => ImmutableList.Create<T>(),
                e => e.Where(i => i.SafeNotEquals(null)).ToReadOnlyCollection()
            );
        }

        /// <summary>
        /// Returns a collection of not null values. Handles null enumerable.
        /// </summary>
        public static IReadOnlyCollection<T> ExceptNulls<T>(this IEnumerable<T?> enumerable) where T : struct
        {
            return enumerable.ToNotEmptyCollection().Match(
                _ => ImmutableList.Create<T>(),
                e => e.Where(i => i.SafeNotEquals(null)).Select(j => j.Value).ToReadOnlyCollection()
            );
        }

        /// <summary>
        /// Flattens enumerable of not empty Maybes into one collection. Handles null enumerable.
        /// </summary>
        public static IReadOnlyCollection<T> Flatten<T>(this IEnumerable<Maybe<T>> enumerable) => enumerable.ToReadOnlyCollection().SelectMany(m => m.AsReadOnlyCollection()).ToReadOnlyCollection();

        /// <summary>
        /// Flattens enumerable of enumerables into one collection. Handles null enumerable and null children and their children.
        /// </summary>
        public static IReadOnlyCollection<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) where T : class => enumerable.ExceptNulls().SelectMany(i => i.ExceptNulls()).ToReadOnlyCollection();

        /// <summary>
        /// Flattens enumerable of enumerables into one collection. Handles null enumerable and null children and their children.
        /// </summary>
        public static IReadOnlyCollection<T> Flatten<T>(this IEnumerable<IEnumerable<T?>> enumerable) where T : struct => enumerable.ExceptNulls().SelectMany(i => i.ExceptNulls()).ToReadOnlyCollection();

        /// <summary>
        /// Returns collection of not empty Maybes. Handles null params.
        /// </summary>
        public static IReadOnlyCollection<T> Flatten<T>(params Maybe<T>[] maybes) => Flatten(maybes.ToReadOnlyCollection());

        /// <summary>
        /// Merges not empty Maybe with not empty Maybes collection into one collection. Handles null enumerable.
        /// </summary>
        public static IReadOnlyCollection<T> FlatMerge<T>(this Maybe<T> maybe, IEnumerable<Maybe<T>> enumerable) => Flatten(new[] { maybe }.Concat(enumerable.ToReadOnlyCollection()));

        /// <summary>
        /// Merges not empty Maybe with other not empty Maybes into one collection. Handles null params.
        /// </summary>
        public static IReadOnlyCollection<T> FlatMerge<T>(this Maybe<T> maybe, params Maybe<T>[] maybes) => maybe.FlatMerge(maybes.ToReadOnlyCollection());

        /// <summary>
        /// Returns a Maybe of the enumerable if not empty or null. Otherwise, it returns an empty Maybe. Handles null enumerable.
        /// </summary>
        public static Maybe<IReadOnlyCollection<T>> ToNotEmptyCollection<T>(this IEnumerable<T> enumerable)
        {
            var collection = enumerable.ToReadOnlyCollection();
            return collection.NotEmptyOrNull() ? collection.AsMaybe() : Empty;
        }

        /// <summary>
        /// Returns a Maybe of the first element in the enumerable that satisfies the condition or returns an empty Maybe. Handles null enumerable.
        /// </summary>
        public static Maybe<T> AsFirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate = null)
        {
            return enumerable.WhereOrDefault(predicate).Map(c => c.First());
        }

        /// <summary>
        /// Returns a Maybe of ReadOnlyCollection of items that satisfy the predicate or returns an empty Maybe. Handles null enumerable.
        /// </summary>
        public static Maybe<IReadOnlyCollection<T>> WhereOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate = null)
        {
            return enumerable.ToReadOnlyCollection().Where(predicate ?? (i => true)).ToNotEmptyCollection();
        }

        /// <summary>
        /// Returns a Maybe of the last element in the enumerable that satisfies the condition or returns an empty Maybe. Handles null enumerable.
        /// </summary>
        public static Maybe<T> AsLastOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate = null) => enumerable.ToReadOnlyCollection().Reverse().AsFirstOrDefault(predicate);

        /// <summary>
        /// Checks whether a given enumerable is empty or null.
        /// </summary>
        public static bool IsEmptyOrNull<T>(this IEnumerable<T> enumerable) => enumerable is null || !enumerable.Any();

        /// <summary>
        /// Checks whether a given enumerable is not empty or null.
        /// </summary>
        public static bool NotEmptyOrNull<T>(this IEnumerable<T> enumerable) => !enumerable.IsEmptyOrNull();

        /// <summary>
        /// Checks whether a given string is null or empty and returns a Maybe of that string if it is not. Otherwise, it returns empty Maybe.
        /// </summary>
        public static Maybe<string> AsNotEmptyString(this string item) => string.IsNullOrEmpty(item) ? Empty : item.AsMaybe();

        /// <summary>
        /// Returns a record of 2 collections where first collection satisfies the predicate and second doesn't. Handles null enumerable.
        /// </summary>
        public static Record<IReadOnlyCollection<T>, IReadOnlyCollection<T>> ConditionalSplit<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var collection = enumerable.ToReadOnlyCollection();
            var left = collection.Where(predicate).ToReadOnlyCollection();
            var right = collection.Except(left).ToReadOnlyCollection();
            return Record.Create(left, right);
        }

        /// <summary>
        /// Creates an immutable dictionary of key and enumerables out of enumerable by a specified selector.
        /// </summary>
        public static IImmutableDictionary<TKey, IReadOnlyCollection<TSource>> ToDictionary<TSource, TKey>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> keySelector)
        {
            return enumerable.ToReadOnlyCollection().GroupBy(keySelector).ToImmutableDictionary(i => i.Key, i => i.ToReadOnlyCollection());
        }

        /// <summary>
        /// Creates a collection of records out of enumerable by a specified selector.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Record<TKey, IReadOnlyCollection<TSource>>> ToRecord<TSource, TKey>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> keySelector)
        {
            return enumerable.ToDictionary(keySelector).Select(i => Record.Create(i.Key, i.Value)).ToReadOnlyCollection();
        }

        /// <summary>
        /// Structure-preserving map.
        /// Maps the specified enumerable to a new enumerable of specified type. Handles null enumerable.
        /// </summary>
        public static IReadOnlyCollection<R> Map<T, R>(this IEnumerable<T> enumerable, Func<T, R> mapper)
        {
            return enumerable.ToReadOnlyCollection().Select(mapper).ToReadOnlyCollection();
        }

        /// <summary>
        /// Aggregates enumerable to the specified result as Maybe. Handles null enumerable.
        /// </summary>
        public static Maybe<T> Reduce<T>(this IEnumerable<T> enumerable, Func<T, T, T> reducer)
        {
            return enumerable.ToNotEmptyCollection().Map(e => e.Aggregate(reducer));
        }

        /// <summary>
        /// Maps the specified enumerable to a new enumerable of specified type and aggregates enumerable of the new type to the specified result as Maybe. Handles null enumerable.
        /// </summary>
        public static Maybe<R> MapReduce<T, R>(this IEnumerable<T> enumerable, Func<T, R> mapper, Func<R, R, R> reducer)
        {
            return enumerable.Map(mapper).Reduce(reducer);
        }
    }
}
