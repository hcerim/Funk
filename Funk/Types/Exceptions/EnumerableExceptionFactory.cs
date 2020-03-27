using System;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public static class EnumerableExceptionFactory
    {
        [Pure]
        public static EnumerableException ToException(this Exception exception) => EnumerableException.FromException(exception);
    }
}
