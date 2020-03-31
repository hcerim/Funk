using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Funk.Exceptions;

namespace Funk
{
    public static class ExceptionExt
    {
        [Pure]
        public static EnumerableException<E> ToEnumerableException<E>(this E exception) where E : Exception => EnumerableException.Create(exception);

        [Pure]
        public static EnumerableException<E> ToEnumerableException<E>(this E exception, string message) where E : Exception => EnumerableException.Create(message, exception);

        [Pure]
        public static EnumerableException<E> ToEnumerableException<E>(this IEnumerable<E> exceptions, string message) where E : Exception => EnumerableException.Create(message, exceptions.Map());

        [Pure]
        public static EnumerableException<E> Merge<E>(this E exc, E exception) where E : Exception
        {
            return exc.MergeRange(exception.ToImmutableList());
        }

        [Pure]
        public static EnumerableException<E> MergeRange<E>(this E exc, IEnumerable<E> exceptions) where E : Exception
        {
            return EnumerableException.Create(exc?.Message, exc.ToImmutableList().Concat(exceptions.Map()));
        }
    }
}
