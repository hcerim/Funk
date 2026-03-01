using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funk;

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

    extension<E>(E exc) where E : Exception
    {
        /// <summary>
        /// Merges two exceptions into an EnumerableException.
        /// </summary>
        [Pure]
        public EnumerableException<E> Merge(E exception) => exc.MergeRange(exception.ToImmutableList());

        /// <summary>
        /// Merges an exception with a sequence of exceptions into an EnumerableException.
        /// </summary>
        [Pure]
        public EnumerableException<E> MergeRange(IEnumerable<E> exceptions) => EnumerableException.Create(exc.ToImmutableList().SafeConcat(exceptions), exc?.Message);
    }
}