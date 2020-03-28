using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static EnumerableException<E> exc<E>(E exception) where E : Exception => exception.ToEnumerableException();

        [Pure]
        public static EnumerableException<E> exc<E>(string message, E exception) where E : Exception => exception.ToEnumerableException(message);

        [Pure]
        public static EnumerableException<E> exc<E>(string message, IEnumerable<E> exceptions) where E : Exception => exceptions.ToEnumerableException(message);
    }
}
