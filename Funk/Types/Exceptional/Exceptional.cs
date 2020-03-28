using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    /// <summary>
    /// Exceptional monad.
    /// Can represent successful result, error (in a form of EnumerableException of a specified exception type) or empty value.
    /// </summary>
    public sealed class Exceptional<T, E> : OneOf<T, EnumerableException<E>> where E : Exception
    {
        private Exceptional()
        {
        }

        internal Exceptional(T result)
            : base(result)
        {
        }

        internal Exceptional(E exception)
            : base(exception.ToException())
        {
        }

        internal Exceptional(EnumerableException<E> exception)
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
        public Maybe<IImmutableList<E>> NestedFailures => Failure.FlatMap(e => e.Nested);

        [Pure]
        public bool IsSuccess => IsFirst;

        [Pure] 
        public bool IsFailure => IsSecond;

        public static implicit operator Exceptional<T, E>(Unit unit) => new Exceptional<T, E>();
    }
}
