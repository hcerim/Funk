using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static EnumerableException<E> exc<E>(E exception) where E : Exception => exception.ToException();

        [Pure]
        public static EnumerableException<E> exc<E>(string message, E exception) where E : Exception => EnumerableException.Create(message, exception);

        [Pure]
        public static EnumerableException<E> exc<E>(string message, IEnumerable<E> exceptions) where E : Exception => EnumerableException.Create(message, exceptions);
    }
}
