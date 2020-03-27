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

        public Maybe<IReadOnlyCollection<E>> NestedFailures => Failure.Map(e => e.Nested);

        [Pure]
        public bool IsSuccess => IsFirst;

        [Pure] 
        public bool IsFailure => IsSecond;

        public static implicit operator Exceptional<T, E>(Unit unit) => new Exceptional<T, E>();
    }
}
