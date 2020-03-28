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
            nested = exception.ToImmutableList();
            Root = exception.AsMaybe();
        }

        public EnumerableException(string message, E exception)
            : base(FunkExceptionType.Enumerable, message)
        {
            nested = exception.ToImmutableList();
            Root = exception.AsMaybe();
        }

        public EnumerableException(string message, IEnumerable<E> exceptions)
            : base(FunkExceptionType.Enumerable, message)
        {
            var list = exceptions.ExceptNulls();
            nested = list;
            Root = list.FirstOrDefault().AsMaybe();
        }

        /// <summary>
        /// Collection of aggregated exceptions including the root one.
        /// </summary>
        public Maybe<IImmutableList<E>> Nested => nested.AsNotEmptyList();

        private IImmutableList<E> nested { get; }

        /// <summary>
        /// Root error cause.
        /// If you pass a collection of exceptions, it will be the first one.
        /// </summary>
        public Maybe<E> Root { get; }

        /// <summary>
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exception.
        /// </summary>
        public EnumerableException<E> MapWith(Func<Unit, E> selector)
        {
            return MapWithMany(_ => selector(Unit.Value).ToImmutableList());
        }

        /// <summary>
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exceptions.
        /// </summary>
        public EnumerableException<E> MapWithMany(Func<Unit, IEnumerable<E>> selector)
        {
            var exceptions = selector(Unit.Value).Map();
            return EnumerableException.Create(Message, Nested.Match(
                _ => exceptions,
                c => c.Concat(exceptions)
            ));
        }

        /// <summary>
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exception and its nested ones.
        /// </summary>
        public EnumerableException<E> Bind(EnumerableException<E> exception)
        {
            var list = new List<E>();
            list.AddRange(Nested.GetOrElse(_ => ImmutableList<E>.Empty.Map()));
            list.AddRange(exception?.Nested.GetOrElse(_ => ImmutableList<E>.Empty.Map()));
            return EnumerableException.Create(Message, list);
        }

        /// <summary>
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exceptions and their nested ones.
        /// </summary>
        public EnumerableException<E> BindRange(IEnumerable<EnumerableException<E>> exceptions)
        {
            var list = new List<E>();
            list.AddRange(Nested.GetOrElse(_ => ImmutableList<E>.Empty.Map()));
            list.AddRange(exceptions.FlatMap(e => e.Nested.GetOrElse(_ => ImmutableList<E>.Empty.Map())));
            return EnumerableException.Create(Message, list);
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
