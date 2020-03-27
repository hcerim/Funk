using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public static class EnumerableExceptionExt
    {
        [Pure]
        public static EnumerableException ToException(this Exception exception) => EnumerableException.Create(exception);

        public static EnumerableException Merge(this Exception exc, Exception exception)
        {
            return EnumerableException.Create(exc?.Message, exception.ToReadOnlyCollection());
        }

        public static EnumerableException MergeRange(this Exception exc, IEnumerable<Exception> exceptions)
        {
            return EnumerableException.Create(exc?.Message, exceptions.ExceptNulls());
        }

        public static EnumerableException Bind(this EnumerableException first, EnumerableException second)
        {
            var list = new List<Exception>();
            list.AddRange(first?.Nested);
            list.AddRange(second?.Nested);
            return EnumerableException.Create(first?.Message, list.ExceptNulls());
        }

        public static EnumerableException BindRange(this EnumerableException first, IEnumerable<EnumerableException> exceptions)
        {
            var list = new List<Exception>();
            list.AddRange(first?.Nested);
            list.AddRange(exceptions.FlatMap(e => e.Nested));
            return EnumerableException.Create(first?.Message, list.ExceptNulls());
        }
    }
}
