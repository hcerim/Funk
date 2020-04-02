﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public static class ExceptionExt
    {
        [Pure]
        public static EnumerableException<E> ToEnumerableException<E>(this E exception, string message = null) where E : Exception => EnumerableException.Create(exception, message);

        [Pure]
        public static EnumerableException<E> ToEnumerableException<E>(this IEnumerable<E> exceptions, string message = null) where E : Exception => EnumerableException.Create(exceptions, message);

        [Pure]
        public static EnumerableException<E> Merge<E>(this E exc, E exception) where E : Exception
        {
            return exc.MergeRange(exception.ToImmutableList());
        }

        [Pure]
        public static EnumerableException<E> MergeRange<E>(this E exc, IEnumerable<E> exceptions) where E : Exception
        {
            return EnumerableException.Create(exc.ToImmutableList().SafeConcat(exceptions), exc?.Message);
        }
    }
}
