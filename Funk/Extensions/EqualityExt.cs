using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funk
{
    public static class EqualityExt
    {
        [Pure]
        public static bool SafeEquals<T>(this T t, T other) => Equals(t, other);

        [Pure]
        public static bool SafeEquals<T>(this T t, T? other) where T : struct => ((T?)t).SafeEquals(other);

        [Pure]
        public static bool SafeEquals<T>(this T? t, T other) where T : struct => t.SafeEquals((T?)other);

        [Pure]
        public static bool SafeNotEquals<T>(this T t, T other) => !t.SafeEquals(other);

        [Pure]
        public static bool SafeNotEquals<T>(this T t, T? other) where T : struct => !t.SafeEquals(other);

        [Pure]
        public static bool SafeNotEquals<T>(this T? t, T other) where T : struct => !t.SafeEquals(other);

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeAnyEquals<T>(this IEnumerable<T> enumerable, T item) => enumerable.ToReadOnlyCollection().Any(i => i.SafeEquals(item));

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeAnyEquals<T>(this IEnumerable<T?> enumerable, T item) where T: struct => enumerable.ToReadOnlyCollection().Any(i => i.SafeEquals(item));

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeAnyEquals<T>(this IEnumerable<T> enumerable, T? item) where T : struct => enumerable.ToReadOnlyCollection().Any(i => i.SafeEquals(item));

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeAllEquals<T>(this IEnumerable<T> enumerable, T item) => enumerable.ToReadOnlyCollection().All(i => i.SafeEquals(item));

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeAllEquals<T>(this IEnumerable<T?> enumerable, T item) where T : struct => enumerable.ToReadOnlyCollection().All(i => i.SafeEquals(item));

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeAllEquals<T>(this IEnumerable<T> enumerable, T? item) where T : struct => enumerable.ToReadOnlyCollection().All(i => i.SafeEquals(item));

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAny<T>(this T item, IEnumerable<T> enumerable) => enumerable.SafeAnyEquals(item);

        /// <summary>
        /// Handles null params.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAny<T>(this T item, params T[] items) => items.SafeAnyEquals(item);

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAny<T>(this T item, IEnumerable<T?> enumerable) where T : struct => enumerable.SafeAnyEquals(item);

        /// <summary>
        /// Handles null params.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAny<T>(this T? item, params T[] items) where T : struct => items.SafeAnyEquals(item);

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAny<T>(this T? item, IEnumerable<T> enumerable) where T : struct => enumerable.SafeAnyEquals(item);

        /// <summary>
        /// Handles null params.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAny<T>(this T item, params T?[] items) where T : struct => items.SafeAnyEquals(item);

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAll<T>(this T item, IEnumerable<T> enumerable) => enumerable.SafeAllEquals(item);

        /// <summary>
        /// Handles null params.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAll<T>(this T item, params T[] items) => items.SafeAllEquals(item);

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAll<T>(this T item, IEnumerable<T?> enumerable) where T : struct => enumerable.SafeAllEquals(item);

        /// <summary>
        /// Handles null params.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAll<T>(this T? item, params T[] items) where T : struct => items.SafeAllEquals(item);

        /// <summary>
        /// Handles null enumerable.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAll<T>(this T? item, IEnumerable<T> enumerable) where T : struct => enumerable.SafeAllEquals(item);

        /// <summary>
        /// Handles null params.
        /// </summary>
        [Pure]
        public static bool SafeEqualsToAll<T>(this T item, params T?[] items) where T : struct => items.SafeAllEquals(item);
    }
}
