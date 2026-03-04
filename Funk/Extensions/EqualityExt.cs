using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funk;

/// <summary>
/// Provides extension methods for null-safe equality comparisons.
/// </summary>
public static class EqualityExt
{
    /// <summary>
    /// Performs a null-safe equality comparison between two objects.
    /// </summary>
    /// <param name="t">The first object to compare.</param>
    /// <param name="other">The second object to compare.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeEquals<T>(this T t, T other) => Equals(t, other);

    /// <summary>
    /// Performs a null-safe equality comparison between a value and a nullable value.
    /// </summary>
    /// <param name="t">The value to compare.</param>
    /// <param name="other">The nullable value to compare.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeEquals<T>(this T t, T? other) where T : struct => ((T?)t).SafeEquals(other);

    /// <summary>
    /// Performs a null-safe equality comparison between a nullable value and a value.
    /// </summary>
    /// <param name="t">The nullable value to compare.</param>
    /// <param name="other">The value to compare.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeEquals<T>(this T? t, T other) where T : struct => t.SafeEquals((T?)other);

    /// <summary>
    /// Performs a null-safe inequality comparison between two objects.
    /// </summary>
    /// <param name="t">The first object to compare.</param>
    /// <param name="other">The second object to compare.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeNotEquals<T>(this T t, T other) => !t.SafeEquals(other);

    /// <summary>
    /// Performs a null-safe inequality comparison between a value and a nullable value.
    /// </summary>
    /// <param name="t">The value to compare.</param>
    /// <param name="other">The nullable value to compare.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeNotEquals<T>(this T t, T? other) where T : struct => !t.SafeEquals(other);

    /// <summary>
    /// Performs a null-safe inequality comparison between a nullable value and a value.
    /// </summary>
    /// <param name="t">The nullable value to compare.</param>
    /// <param name="other">The value to compare.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeNotEquals<T>(this T? t, T other) where T : struct => !t.SafeEquals(other);

    /// <summary>
    /// Returns true if any element in the sequence equals the specified item. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The sequence to search.</param>
    /// <param name="item">The item to compare against.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeAnyEquals<T>(this IEnumerable<T> enumerable, T item) => enumerable.Map().Any(i => i.SafeEquals(item));

    /// <summary>
    /// Returns true if any nullable element in the sequence equals the specified item. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The sequence of nullable values to search.</param>
    /// <param name="item">The item to compare against.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeAnyEquals<T>(this IEnumerable<T?> enumerable, T item) where T: struct => enumerable.Map().Any(i => i.SafeEquals(item));

    /// <summary>
    /// Returns true if any element in the sequence equals the specified nullable item. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The sequence to search.</param>
    /// <param name="item">The nullable item to compare against.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeAnyEquals<T>(this IEnumerable<T> enumerable, T? item) where T : struct => enumerable.Map().Any(i => i.SafeEquals(item));

    /// <summary>
    /// Returns true if all elements in the sequence equal the specified item. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The sequence to check.</param>
    /// <param name="item">The item to compare against.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeAllEquals<T>(this IEnumerable<T> enumerable, T item) => enumerable.Map().All(i => i.SafeEquals(item));

    /// <summary>
    /// Returns true if all nullable elements in the sequence equal the specified item. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The sequence of nullable values to check.</param>
    /// <param name="item">The item to compare against.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeAllEquals<T>(this IEnumerable<T?> enumerable, T item) where T : struct => enumerable.Map().All(i => i.SafeEquals(item));

    /// <summary>
    /// Returns true if all elements in the sequence equal the specified nullable item. Handles null sequence.
    /// </summary>
    /// <param name="enumerable">The sequence to check.</param>
    /// <param name="item">The nullable item to compare against.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeAllEquals<T>(this IEnumerable<T> enumerable, T? item) where T : struct => enumerable.Map().All(i => i.SafeEquals(item));

    extension<T>(T item)
    {
        /// <summary>
        /// Returns true if the item equals any element in the sequence. Handles null sequence.
        /// </summary>
        /// <param name="enumerable">The sequence to search.</param>
        /// <returns>true if the equality condition is met; otherwise, false.</returns>
        [Pure]
        public bool SafeEqualsToAny(IEnumerable<T> enumerable) => enumerable.SafeAnyEquals(item);

        /// <summary>
        /// Returns true if the item equals any of the specified values. Handles null array.
        /// </summary>
        /// <param name="items">The values to compare against.</param>
        /// <returns>true if the equality condition is met; otherwise, false.</returns>
        [Pure]
        public bool SafeEqualsToAny(params T[] items) => items.SafeAnyEquals(item);
    }

    /// <summary>
    /// Returns true if the item equals any nullable element in the sequence. Handles null sequence.
    /// </summary>
    /// <param name="item">The item to compare.</param>
    /// <param name="enumerable">The sequence of nullable values to search.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeEqualsToAny<T>(this T item, IEnumerable<T?> enumerable) where T : struct => enumerable.SafeAnyEquals(item);

    extension<T>(T? item) where T : struct
    {
        /// <summary>
        /// Returns true if the nullable item equals any of the specified values. Handles null array.
        /// </summary>
        /// <param name="items">The values to compare against.</param>
        /// <returns>true if the equality condition is met; otherwise, false.</returns>
        [Pure]
        public bool SafeEqualsToAny(params T[] items) => items.SafeAnyEquals(item);

        /// <summary>
        /// Returns true if the nullable item equals any element in the sequence. Handles null sequence.
        /// </summary>
        /// <param name="enumerable">The sequence to search.</param>
        /// <returns>true if the equality condition is met; otherwise, false.</returns>
        [Pure]
        public bool SafeEqualsToAny(IEnumerable<T> enumerable) => enumerable.SafeAnyEquals(item);
    }

    /// <summary>
    /// Returns true if the item equals any of the specified nullable values. Handles null array.
    /// </summary>
    /// <param name="item">The item to compare.</param>
    /// <param name="items">The nullable values to compare against.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeEqualsToAny<T>(this T item, params T?[] items) where T : struct => items.SafeAnyEquals(item);

    extension<T>(T item)
    {
        /// <summary>
        /// Returns true if the item equals all elements in the sequence. Handles null sequence.
        /// </summary>
        /// <param name="enumerable">The sequence to check.</param>
        /// <returns>true if the equality condition is met; otherwise, false.</returns>
        [Pure]
        public bool SafeEqualsToAll(IEnumerable<T> enumerable) => enumerable.SafeAllEquals(item);

        /// <summary>
        /// Returns true if the item equals all of the specified values. Handles null array.
        /// </summary>
        /// <param name="items">The values to compare against.</param>
        /// <returns>true if the equality condition is met; otherwise, false.</returns>
        [Pure]
        public bool SafeEqualsToAll(params T[] items) => items.SafeAllEquals(item);
    }

    /// <summary>
    /// Returns true if the item equals all nullable elements in the sequence. Handles null sequence.
    /// </summary>
    /// <param name="item">The item to compare.</param>
    /// <param name="enumerable">The sequence of nullable values to check.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeEqualsToAll<T>(this T item, IEnumerable<T?> enumerable) where T : struct => enumerable.SafeAllEquals(item);

    extension<T>(T? item) where T : struct
    {
        /// <summary>
        /// Returns true if the nullable item equals all of the specified values. Handles null array.
        /// </summary>
        /// <param name="items">The values to compare against.</param>
        /// <returns>true if the equality condition is met; otherwise, false.</returns>
        [Pure]
        public bool SafeEqualsToAll(params T[] items) => items.SafeAllEquals(item);

        /// <summary>
        /// Returns true if the nullable item equals all elements in the sequence. Handles null sequence.
        /// </summary>
        /// <param name="enumerable">The sequence to check.</param>
        /// <returns>true if the equality condition is met; otherwise, false.</returns>
        [Pure]
        public bool SafeEqualsToAll(IEnumerable<T> enumerable) => enumerable.SafeAllEquals(item);
    }

    /// <summary>
    /// Returns true if the item equals all of the specified nullable values. Handles null array.
    /// </summary>
    /// <param name="item">The item to compare.</param>
    /// <param name="items">The nullable values to compare against.</param>
    /// <returns>true if the equality condition is met; otherwise, false.</returns>
    [Pure]
    public static bool SafeEqualsToAll<T>(this T item, params T?[] items) where T : struct => items.SafeAllEquals(item);
}
