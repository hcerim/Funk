using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public sealed class Exceptional<T, E> : OneOf<T, E> where E : EnumerableException
    {
        private Exceptional()
        {
        }

        public Exceptional(T result)
            : base(result)
        {
        }

        public Exceptional(E exception)
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
        public Maybe<E> Failure => Second;

        [Pure]
        public bool IsSuccess => Discriminator.SafeEquals(1);

        [Pure]
        public bool IsFailure => Discriminator.SafeEquals(2);

        public static implicit operator Exceptional<T, E>(Unit unit) => new Exceptional<T, E>();
    }
}
