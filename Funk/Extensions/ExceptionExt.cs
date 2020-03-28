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
        public static EnumerableException<E> ToException<E>(this E exception) where E : Exception => EnumerableException.Create(exception);

        public static EnumerableException<E> Merge<E>(this E exc, E exception) where E : Exception
        {
            return exc.MergeRange(exception.ToImmutableList());
        }

        public static EnumerableException<E> MergeRange<E>(this E exc, IEnumerable<E> exceptions) where E : Exception
        {
            return EnumerableException.Create(exc?.Message, exc.ToImmutableList().Concat(exceptions));
        }
    }
}
