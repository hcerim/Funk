using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public static class EnumerableExceptionExt
    {
        [Pure]
        public static EnumerableException<E> ToException<E>(this E exception) where E : Exception => EnumerableException.Create(exception);

        public static EnumerableException<E> Merge<E>(this E exc, E exception) where E : Exception
        {
            return EnumerableException.Create(exc?.Message, exception.ToReadOnlyCollection());
        }

        public static EnumerableException<E> MergeRange<E>(this E exc, IEnumerable<E> exceptions) where E : Exception
        {
            return EnumerableException.Create(exc?.Message, exceptions.ExceptNulls());
        }

        public static EnumerableException<E> Bind<E>(this EnumerableException<E> first, EnumerableException<E> second) where E : Exception
        {
            var list = new List<E>();
            list.AddRange(first?.Nested);
            list.AddRange(second?.Nested);
            return EnumerableException.Create(first?.Message, list.ExceptNulls());
        }

        public static EnumerableException<E> BindRange<E>(this EnumerableException<E> first, IEnumerable<EnumerableException<E>> exceptions) where E : Exception
        {
            var list = new List<E>();
            list.AddRange(first?.Nested);
            list.AddRange(exceptions.FlatMap(e => e.Nested));
            return EnumerableException.Create(first?.Message, list.ExceptNulls());
        }
    }
}
