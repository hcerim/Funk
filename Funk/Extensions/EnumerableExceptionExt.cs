using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            return EnumerableException.Create(exc?.Message, exception.ToImmutableList());
        }

        public static EnumerableException<E> MergeRange<E>(this E exc, IEnumerable<E> exceptions) where E : Exception
        {
            return EnumerableException.Create(exc?.Message, exceptions);
        }

        public static EnumerableException<E> Bind<E>(this EnumerableException<E> first, EnumerableException<E> second) where E : Exception
        {
            var list = new List<E>();
            list.AddRange(first?.Nested.GetOrElse(_ => ImmutableList<E>.Empty.Map()));
            list.AddRange(second?.Nested.GetOrElse(_ => ImmutableList<E>.Empty.Map()));
            return EnumerableException.Create(first?.Message, list);
        }

        public static EnumerableException<E> BindRange<E>(this EnumerableException<E> first, IEnumerable<EnumerableException<E>> exceptions) where E : Exception
        {
            var list = new List<E>();
            list.AddRange(first?.Nested.GetOrElse(_ => ImmutableList<E>.Empty.Map()));
            list.AddRange(exceptions.FlatMap(e => e.Nested.GetOrElse(_ => ImmutableList<E>.Empty.Map())));
            return EnumerableException.Create(first?.Message, list);
        }
    }
}
