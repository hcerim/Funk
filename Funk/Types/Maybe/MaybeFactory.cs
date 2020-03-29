using System.Diagnostics.Contracts;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Maybe monad.
    /// Type that represents the possible absence of data with appropriate handling.
    /// </summary>
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

        /// <summary>
        /// Creates empty Maybe.
        /// </summary>
        [Pure]
        public static Maybe<T> Empty<T>() => new Maybe<T>();
    }
}
