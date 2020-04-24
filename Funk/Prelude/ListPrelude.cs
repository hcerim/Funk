using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static IImmutableList<T> list<T>(params T[] items) => items.Map();

        [Pure]
        public static IImmutableList<T> list<T>(IEnumerable<T> enumerable) => enumerable.Map();

        [Pure]
        public static IImmutableList<T> list<T>() => Enumerable.Empty<T>().Map();

        public static IImmutableList<int> range(int start, int count) => Enumerable.Range(start, count).Map();

        public static IImmutableList<T> repeat<T>(T element, int count) => Enumerable.Repeat(element, count).Map();
    }
}
