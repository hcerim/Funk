using System;
using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        /// <summary>
        /// Creates a successful Exc with the specified value.
        /// </summary>
        [Pure]
        public static Exc<T, E> success<T, E>(T item) where E : Exception => Exc.Success<T, E>(item);

        /// <summary>
        /// Creates a failed Exc with the specified exception.
        /// </summary>
        [Pure]
        public static Exc<T, E> failure<T, E>(E exception) where E : Exception => Exc.Failure<T, E>(exception);

        /// <summary>
        /// Creates a failed Exc with the specified enumerable exception.
        /// </summary>
        [Pure]
        public static Exc<T, E> failure<T, E>(EnumerableException<E> exception) where E : Exception => Exc.Failure<T, E>(exception);
    }
}
