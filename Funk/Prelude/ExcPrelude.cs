using System;
using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static Exc<T, E> success<T, E>(T item) where E : Exception => Exc.Success<T, E>(item);

        [Pure]
        public static Exc<T, E> failure<T, E>(E exception) where E : Exception => Exc.Failure<T, E>(exception);

        [Pure]
        public static Exc<T, E> failure<T, E>(EnumerableException<E> exception) where E : Exception => Exc.Failure<T, E>(exception);
    }
}
