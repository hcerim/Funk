using System.Diagnostics.Contracts;

namespace Funk
{
    public struct Maybe
    {
        [Pure]
        public static Maybe<T> Create<T>(T item) => new Maybe<T>(item);

        [Pure]
        public static Maybe<T> Create<T>(T? item) where T: struct => new Maybe<T>((T)item);
    }
}
