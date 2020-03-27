using System.Diagnostics.Contracts;
using static Funk.Prelude;

namespace Funk
{
    public struct Maybe
    {
        [Pure]
        public static Maybe<T> Create<T>(T item) => new Maybe<T>(item);

        [Pure]
        public static Maybe<T> Create<T>(T? item) where T: struct => item.IsNotNull() ? new Maybe<T>((T)item) : empty;
    }
}
