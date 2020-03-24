using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funk
{
    public static class EnumerableExt
    {
        /// <summary>
        /// Creates a collection out of Maybe value if not empty. Otherwise, it returns an empty collection.
        /// </summary>
        [Pure]
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this Maybe<T> maybe) => maybe.Match(_ => ImmutableList<T>.Empty, ImmutableList.Create);

        /// <summary>
        /// Creates a read only collection of enumerable. Handles null enumerable.
        /// </summary>
        [Pure]
        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return ImmutableList.CreateRange(enumerable ?? ImmutableList.Create<T>());
        }

        /// <summary>
        /// Returns a collection of not null values. Handles null enumerable.
        /// </summary>
        [Pure]
        public static IReadOnlyCollection<T> ExceptNulls<T>(this IEnumerable<T> enumerable) where T : class
        {
            var list = new List<T>();
            list.AddRange(enumerable ?? new List<T>());
            return list.Where(i => i.SafeNotEquals(null)).ToReadOnlyCollection();
        }

        /// <summary>
        /// Flattens enumerable of not empty Maybes into one collection. Handles null enumerable.
        /// </summary>
        [Pure]
        public static IReadOnlyCollection<T> Flatten<T>(this IEnumerable<Maybe<T>> enumerable) => enumerable.ToReadOnlyCollection().SelectMany(m => m.AsReadOnlyCollection()).ToReadOnlyCollection();

        /// <summary>
        /// Flattens enumerable of enumerables into one collection. Handles null enumerable and null children.
        /// </summary>
        [Pure]
        public static IReadOnlyCollection<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) => enumerable.ToReadOnlyCollection().SelectMany(i => i.ToReadOnlyCollection()).ToReadOnlyCollection();

        /// <summary>
        /// Returns collection of not empty Maybes.
        /// </summary>
        [Pure]
        public static IReadOnlyCollection<T> Flatten<T>(params Maybe<T>[] maybes) => Flatten(maybes.ToReadOnlyCollection());

        /// <summary>
        /// Merges not empty Maybe with not empty Maybes collection into one collection. Handles null enumerable.
        /// </summary>
        [Pure]
        public static IReadOnlyCollection<T> FlatMerge<T>(this Maybe<T> maybe, IEnumerable<Maybe<T>> enumerable) => Flatten(new[] { maybe }.Concat(enumerable.ToReadOnlyCollection()));

        /// <summary>
        /// Merges not empty Maybe with other not empty Maybes into one collection.
        /// </summary>
        [Pure]
        public static IReadOnlyCollection<T> FlatMerge<T>(this Maybe<T> maybe, params Maybe<T>[] maybes) => maybe.FlatMerge(maybes.ToReadOnlyCollection());

        /// <summary>
        /// Returns a Maybe of the enumerable if not empty or null. Otherwise, it returns an empty Maybe. Handles null enumerable.
        /// </summary>
        [Pure]
        public static Maybe<IReadOnlyCollection<T>> ToNotEmptyCollection<T>(this IEnumerable<T> enumerable)
        {
            var collection = enumerable.ToReadOnlyCollection();
            return collection.NotEmptyOrNull() ? collection.AsMaybe() : Maybe.Empty;
        }

        /// <summary>
        /// Returns a Maybe of the first element in the enumerable that satisfies the condition or returns an empty Maybe. Handles null enumerable.
        /// </summary>
        [Pure]
        public static Maybe<T> AsFirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate = null)
        {
            return enumerable.Where(predicate ?? (i => true)).ToNotEmptyCollection().Map(c => c.First());
        }

        /// <summary>
        /// Returns a Maybe of the last element in the enumerable that satisfies the condition or returns an empty Maybe. Handles null enumerable.
        /// </summary>
        [Pure]
        public static Maybe<T> AsLastOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate = null) => enumerable.ToReadOnlyCollection().Reverse().AsFirstOrDefault(predicate);

        /// <summary>
        /// Checks whether a given enumerable is empty or null.
        /// </summary>
        [Pure]
        public static bool IsEmptyOrNull<T>(this IEnumerable<T> enumerable) => enumerable is null || !enumerable.Any();

        /// <summary>
        /// Checks whether a given enumerable is not empty or null.
        /// </summary>
        [Pure]
        public static bool NotEmptyOrNull<T>(this IEnumerable<T> enumerable) => !enumerable.IsEmptyOrNull();

        /// <summary>
        /// Checks whether a given string is null or empty and returns a Maybe of that string if it is not. Otherwise, it returns empty Maybe.
        /// </summary>
        [Pure]
        public static Maybe<string> AsNotEmptyString(this string item) => string.IsNullOrEmpty(item) ? Maybe.Empty : item.AsMaybe();

        /// <summary>
        /// Returns a record of 2 collections where first collection satisfies the predicate and second doesn't. Handles null enumerable.
        /// </summary>
        [Pure]
        public static Record<IReadOnlyCollection<T>, IReadOnlyCollection<T>> ConditionalSplit<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var collection = enumerable.ToReadOnlyCollection();
            var left = collection.Where(predicate).ToReadOnlyCollection();
            var right = collection.Except(left).ToReadOnlyCollection();
            return Record.Create(left, right);
        }
    }
}
