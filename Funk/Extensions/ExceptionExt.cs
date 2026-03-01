using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funk
{
    /// <summary>
    /// Provides extension methods for creating and merging exception collections.
    /// </summary>
    public static class ExceptionExt
    {
        /// <summary>
        /// Wraps a single exception into an EnumerableException with an optional message.
        /// </summary>
        [Pure]
        public static EnumerableException<E> ToEnumerableException<E>(this E exception, string message = null) where E : Exception => EnumerableException.Create(exception, message);

        /// <summary>
        /// Wraps a sequence of exceptions into an EnumerableException with an optional message.
        /// </summary>
        [Pure]
        public static EnumerableException<E> ToEnumerableException<E>(this IEnumerable<E> exceptions, string message = null) where E : Exception => EnumerableException.Create(exceptions, message);

        /// <summary>
        /// Merges two exceptions into an EnumerableException.
        /// </summary>
        [Pure]
        public static EnumerableException<E> Merge<E>(this E exc, E exception) where E : Exception => exc.MergeRange(exception.ToImmutableList());

        /// <summary>
        /// Merges an exception with a sequence of exceptions into an EnumerableException.
        /// </summary>
        [Pure]
        public static EnumerableException<E> MergeRange<E>(this E exc, IEnumerable<E> exceptions) where E : Exception => EnumerableException.Create(exc.ToImmutableList().SafeConcat(exceptions), exc?.Message);
    }
}
