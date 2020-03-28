using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Funk.Exceptions;

namespace Funk
{
    /// <summary>
    /// Exceptional monad.
    /// Can represent successful result, error (in a form of EnumerableException of a specified exception type) or empty value.
    /// </summary>
    public static class Exc
    {
        /// <summary>
        /// Returns Exceptional of possible result and exception. Indicates that the operation can throw specified exception.
        /// </summary>
        public static Exc<T, E> Create<T, E>(Func<Unit, T> operation) where E : Exception
        {
            try
            {
                return new Exc<T, E>(operation(Unit.Value));
            }
            catch (E e)
            {
                return new Exc<T, E>(e);
            }
        }

        /// <summary>
        /// Preferably use Create with an explicit exception type definition.
        /// Returns Exceptional of possible result and exception. Indicates that the operation can throw any exception.
        /// </summary>
        public static Exc<T, Exception> Create<T>(Func<Unit, T> operation)
        {
            try
            {
                return new Exc<T, Exception>(operation(Unit.Value));
            }
            catch (Exception e)
            {
                return new Exc<T, Exception>(e);
            }
        }

        /// <summary>
        /// Returns Task of Exceptional of possible result and exception. Indicates that the operation can throw specified exception.
        /// </summary>
        public static async Task<Exc<T, E>> Create<T, E>(Func<Unit, Task<T>> operation) where E : Exception
        {
            try
            {
                return new Exc<T, E>(await operation(Unit.Value));
            }
            catch (E e)
            {
                return await Task.Run(() => new Exc<T, E>(e));
            }
        }

        /// <summary>
        /// Preferably use Create with an explicit exception type definition.
        /// Returns Task of Exceptional of possible result and exception. Indicates that the operation can throw any exception.
        /// </summary>
        public static async Task<Exc<T, Exception>> Create<T>(Func<Unit, Task<T>> operation)
        {
            try
            {
                return new Exc<T, Exception>(await operation(Unit.Value));
            }
            catch (Exception e)
            {
                return await Task.Run(() => new Exc<T, Exception>(e));
            }
        }
    }

    /// <summary>
    /// Exceptional monad.
    /// Can represent successful result, error (in a form of EnumerableException of a specified exception type) or empty value.
    /// </summary>
    public sealed class Exc<T, E> : OneOf<T, EnumerableException<E>> where E : Exception
    {
        private Exc()
        {
        }

        internal Exc(T result)
            : base(result)
        {
        }

        internal Exc(E exception)
            : base(exception.ToException())
        {
        }

        internal Exc(EnumerableException<E> exception)
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

        public static implicit operator Exc<T, E>(Unit unit) => new Exc<T, E>();
    }
}
