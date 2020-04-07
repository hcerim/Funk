using System;

namespace Funk
{
    public static partial class Prelude
    {
        public static Exc<T, E> success<T, E>(T item) where E : Exception => Exc.Success<T, E>(item);

        public static Exc<T, E> failure<T, E>(EnumerableException<E> exception) where E : Exception => Exc.Failure<T, E>(exception);
    }
}
