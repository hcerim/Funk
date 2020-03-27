using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static EnumerableException exc(string message) => EnumerableException.Create(message);

        [Pure]
        public static EnumerableException exc(Exception exception) => exception.ToException();

        [Pure]
        public static EnumerableException exc(string message, IEnumerable<Exception> exceptions) => EnumerableException.Create(message, exceptions);
    }
}
