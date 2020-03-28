using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funk.Exceptions
{
    public static class EnumerableException
    {
        /// <summary>
        /// Creates a new EnumerableException with no nested exceptions.
        /// </summary>
        [Pure]
        public static EnumerableException<E> Create<E>(string message) where E : Exception => new EnumerableException<E>(message);

        /// <summary>
        /// Creates a new EnumerableException from existing exception with no nested exceptions.
        /// </summary>
        [Pure]
        public static EnumerableException<E> Create<E>(E exception) where E : Exception => new EnumerableException<E>(exception);

        /// <summary>
        /// Creates a new EnumerableException with one nested exception if not null.
        /// </summary>
        [Pure]
        public static EnumerableException<E> Create<E>(string message, E exception) where E : Exception => new EnumerableException<E>(message, exception);

        /// <summary>
        /// Creates a new EnumerableException with nested exceptions if not null or empty (removes null exceptions from enumerable).
        /// </summary>
        [Pure]
        public static EnumerableException<E> Create<E>(string message, IEnumerable<E> exceptions) where E : Exception => new EnumerableException<E>(message, exceptions);
    }

    public sealed class EnumerableException<E> : FunkException, IEnumerable<E> where E : Exception
    {
        public EnumerableException(string message)
            : base(FunkExceptionType.Enumerable, message)
        {
            nested = ImmutableList<E>.Empty;
        }

        public EnumerableException(E exception)
            : base(FunkExceptionType.Enumerable, exception?.Message)
        {
            nested = ImmutableList<E>.Empty;
        }

        public EnumerableException(string message, E exception)
            : base(FunkExceptionType.Enumerable, message)
        {
            nested = exception.ToImmutableList();
        }

        public EnumerableException(string message, IEnumerable<E> exceptions)
            : base(FunkExceptionType.Enumerable, message)
        {
            nested = exceptions.ExceptNulls();
        }

        public Maybe<IImmutableList<E>> Nested => nested.AsNotEmptyList();

        private IImmutableList<E> nested { get; }

        /// <summary>
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exception.
        /// </summary>
        public EnumerableException<E> MapWith(Func<Unit, E> selector)
        {
            return MapWith(_ => selector(Unit.Value).ToImmutableList());
        }

        /// <summary>
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exceptions.
        /// </summary>
        public EnumerableException<E> MapWith(Func<Unit, IEnumerable<E>> selector)
        {
            var exceptions = selector(Unit.Value).Map();
            return EnumerableException.Create(Message, Nested.Match(
                _ => exceptions,
                c => c.Concat(exceptions)
            ));
        }

        /// <summary>
        /// Returns an immutable dictionary of key as a discriminator and collection of corresponding exceptions.
        /// </summary>
        public Maybe<IImmutableDictionary<TKey, IImmutableList<E>>> ToDictionary<TKey>(Func<E, TKey> keySelector) => Nested.Map(c => c.ToDictionary(keySelector));

        public IEnumerator<E> GetEnumerator()
        {
            return nested.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
