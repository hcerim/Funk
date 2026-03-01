using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        /// <summary>
        /// Creates an EnumerableException from a single exception with an optional message.
        /// </summary>
        [Pure]
        public static EnumerableException<E> exception<E>(E exception, string message = null) where E : Exception => exception.ToEnumerableException(message);

        /// <summary>
        /// Creates an EnumerableException from a sequence of exceptions with an optional message.
        /// </summary>
        [Pure]
        public static EnumerableException<E> exception<E>(IEnumerable<E> exceptions, string message = null) where E : Exception => exceptions.ToEnumerableException(message);
    }
}
