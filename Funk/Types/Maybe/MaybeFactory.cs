using System.Diagnostics.Contracts;
using static Funk.Prelude;

namespace Funk
{
    public struct Maybe
    {
        /// <summary>
        /// Creates a Maybe of item.
        /// </summary>
        [Pure]
        public static Maybe<T> Create<T>(T item) => new Maybe<T>(item);

        /// <summary>
        /// Creates a Maybe of nullable item.
        /// </summary>
        [Pure]
        public static Maybe<T> Create<T>(T? item) where T: struct => item.IsNotNull() ? new Maybe<T>((T)item) : empty;
    }
}
