using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static EnumerableException<E> exception<E>(E exception, string message = null) where E : Exception => exception.ToEnumerableException(message);

        [Pure]
        public static EnumerableException<E> exception<E>(IEnumerable<E> exceptions, string message = null) where E : Exception => exceptions.ToEnumerableException(message);
    }
}
