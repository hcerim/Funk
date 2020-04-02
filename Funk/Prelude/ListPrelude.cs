using System.Collections.Generic;
using System.Collections.Immutable;

namespace Funk
{
    public static partial class Prelude
    {
        public static IImmutableList<T> list<T>(params T[] items) => items.Map();
        public static IImmutableList<T> list<T>(IEnumerable<T> enumerable) => enumerable.Map();
    }
}
