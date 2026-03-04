using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funk;

public static partial class Prelude
{
    /// <summary>
    /// Creates an immutable list from the specified items.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="items">The items.</param>
    /// <returns>An immutable list containing the specified items.</returns>
    [Pure]
    public static IImmutableList<T> list<T>(params T[] items) => items.Map();

    /// <summary>
    /// Creates an immutable list from non-empty Maybe values.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="items">The Maybe values to flatten.</param>
    /// <returns>An immutable list containing the non-empty values.</returns>
    [Pure]
    public static IImmutableList<T> list<T>(params Maybe<T>[] items) => items.Flatten();

    /// <summary>
    /// Creates an immutable list from the specified sequence.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="enumerable">The source sequence.</param>
    /// <returns>An immutable list.</returns>
    [Pure]
    public static IImmutableList<T> list<T>(IEnumerable<T> enumerable) => enumerable.Map();

    /// <summary>
    /// Creates an empty immutable list.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <returns>An empty immutable list.</returns>
    [Pure]
    public static IImmutableList<T> list<T>() => Enumerable.Empty<T>().Map();

    /// <summary>
    /// Creates an immutable list of integers from the specified start and count.
    /// </summary>
    /// <param name="start">The first integer.</param>
    /// <param name="count">The number of integers.</param>
    /// <returns>An immutable list of sequential integers.</returns>
    public static IImmutableList<int> range(int start, int count) => Enumerable.Range(start, count).Map();

    /// <summary>
    /// Creates an immutable list by repeating the specified element a given number of times.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="element">The value to repeat.</param>
    /// <param name="count">The number of repetitions.</param>
    /// <returns>An immutable list of repeated elements.</returns>
    public static IImmutableList<T> repeat<T>(T element, int count) => Enumerable.Repeat(element, count).Map();
}