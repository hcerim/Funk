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
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exception.
        /// </summary>
        public EnumerableException MapWith(Func<Unit, Exception> selector)
        {
            return Create(Message, selector(Unit.Value).MergeRange(Nested));
        }

        /// <summary>
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exceptions.
        /// </summary>
        public EnumerableException MapWith(Func<Unit, IEnumerable<Exception>> selector)
        {
            var list = new List<Exception>(Nested);
            list.AddRange(selector(Unit.Value).Map());
            return Create(Message, list.ExceptNulls());
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
