using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static EnumerableException<E> exception<E>(E exception) where E : Exception => exception.ToEnumerableException();

        [Pure]
        public static EnumerableException<E> exception<E>(string message, E exception) where E : Exception => exception.ToEnumerableException(message);

        [Pure]
        public static EnumerableException<E> exception<E>(string message, IEnumerable<E> exceptions) where E : Exception => exceptions.ToEnumerableException(message);
    }
}
