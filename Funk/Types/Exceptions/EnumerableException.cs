using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Funk.Exceptions
{
    public class EnumerableException : FunkException, IEnumerable<Exception>
    {
        private EnumerableException(Exception exception)
            : base(FunkExceptionType.Enumerable, exception?.Message)
        {
            Nested = ImmutableList<Exception>.Empty;
        }

        public EnumerableException(string message)
            : base(FunkExceptionType.Enumerable, message)
        {
            Nested = ImmutableList<Exception>.Empty;
        }

        public EnumerableException(string message, IEnumerable<Exception> nested)
            : base(FunkExceptionType.Enumerable, message)
        {
            Nested = nested.ExceptNulls();
        }

        public static EnumerableException FromException(Exception exception)
        {
            return new EnumerableException(exception);
        }

        public IReadOnlyCollection<Exception> Nested { get; }

        /// <summary>
        /// Returns an immutable dictionary of key as a discriminator and collection of corresponding exceptions. Handles null enumerable.
        /// </summary>
        [Pure]
        public IReadOnlyDictionary<TKey, IReadOnlyCollection<Exception>> ToDictionary<TKey>(Func<Exception, TKey> keySelector) => Nested.ToDictionary(keySelector);

        public virtual IEnumerator<Exception> GetEnumerator()
        {
            return Nested.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
