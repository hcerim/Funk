using System;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public static class EnumerableExceptionExt
    {
        [Pure]
        public static EnumerableException ToException(this Exception exception) => EnumerableException.Create(exception);
    }
}
