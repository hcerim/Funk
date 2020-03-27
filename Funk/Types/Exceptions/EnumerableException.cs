using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Funk.Exceptions
{
    public class EnumerableException : FunkException, IEnumerable<Exception>
    {
        [Pure]
        public static EnumerableException Create(string message) => new EnumerableException(message);

        [Pure]
        public static EnumerableException Create(Exception exception) => new EnumerableException(exception);

        [Pure]
        public static EnumerableException Create(string message, IEnumerable<Exception> exceptions) => new EnumerableException(message, exceptions);

        /// <summary>
        /// Creates a new EnumerableException with updated Nested exceptions with the new exception.
        /// </summary>
        public static EnumerableException Create(EnumerableException exc, Exception exception)
        {
            return Create(exc?.Message, exception.MergeRange(exc?.Nested).ExceptNulls());
        }

        /// <summary>
        /// Creates a new EnumerableException with updated Nested exceptions with new exceptions.
        /// </summary>
        public static EnumerableException Create(EnumerableException exc, IEnumerable<Exception> exceptions)
        {
            var list = new List<Exception>(exc?.Nested);
            list.AddRange(exceptions.Map());
            return Create(exc?.Message, list.ExceptNulls());
        }

        public EnumerableException(string message)
            : base(FunkExceptionType.Enumerable, message)
        {
            Nested = ImmutableList<Exception>.Empty;
        }

        public EnumerableException(Exception exception)
            : base(FunkExceptionType.Enumerable, exception?.Message)
        {
            Nested = ImmutableList<Exception>.Empty;
        }

        public EnumerableException(string message, IEnumerable<Exception> nested)
            : base(FunkExceptionType.Enumerable, message)
        {
            Nested = nested.ExceptNulls();
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
