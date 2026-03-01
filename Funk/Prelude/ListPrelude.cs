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
    [Pure]
    public static IImmutableList<T> list<T>(params T[] items) => items.Map();

    /// <summary>
    /// Creates an immutable list from non-empty Maybe values.
    /// </summary>
    [Pure]
    public static IImmutableList<T> list<T>(params Maybe<T>[] items) => items.Flatten();

    /// <summary>
    /// Creates an immutable list from the specified sequence.
    /// </summary>
    [Pure]
    public static IImmutableList<T> list<T>(IEnumerable<T> enumerable) => enumerable.Map();

    /// <summary>
    /// Creates an empty immutable list.
    /// </summary>
    [Pure]
    public static IImmutableList<T> list<T>() => Enumerable.Empty<T>().Map();

    /// <summary>
    /// Creates an immutable list of integers from the specified start and count.
    /// </summary>
    public static IImmutableList<int> range(int start, int count) => Enumerable.Range(start, count).Map();

    /// <summary>
    /// Creates an immutable list by repeating the specified element a given number of times.
    /// </summary>
    public static IImmutableList<T> repeat<T>(T element, int count) => Enumerable.Repeat(element, count).Map();
}