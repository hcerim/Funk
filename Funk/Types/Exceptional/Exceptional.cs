using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public sealed class Exceptional<T, E> : OneOf<T, EnumerableException<E>> where E : Exception
    {
        private Exceptional()
        {
        }

        public Exceptional(T result)
            : base(result)
        {
        }

        public Exceptional(E exception)
            : base(exception.ToException())
        {
        }

        public Exceptional(EnumerableException<E> exception)
            : base(exception)
        {
        }

        /// <summary>
        /// Maybe of Success. If it is not success, Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T> Success => First;

        /// <summary>
        /// Maybe of Failure. If it is not failure, Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<EnumerableException<E>> Failure => Second;

        /// <summary>
        /// If Failure, Maybe contains nested exceptions inside EnumerableException if there are any. Otherwise, Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<IReadOnlyCollection<E>> NestedFailures => Failure.FlatMap(e => e.Nested.AsNotEmptyCollection());

        [Pure]
        public bool IsSuccess => IsFirst;

        [Pure] 
        public bool IsFailure => IsSecond;

        public static implicit operator Exceptional<T, E>(Unit unit) => new Exceptional<T, E>();
    }
}
