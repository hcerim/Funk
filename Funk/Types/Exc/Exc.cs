using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Funk.Exceptions;
using Funk.Internal;

namespace Funk
{
    /// <summary>
    /// Exceptional monad.
    /// Can represent successful result, error (in a form of EnumerableException of a specified exception type) or empty value.
    /// </summary>
    public static class Exc
    {
        /// <summary>
        /// Returns Exceptional of result or error or can be empty. Indicates that the operation can throw specified exception.
        /// It will fail on unhandled exceptions.
        /// </summary>
        public static Exc<T, E> Create<T, E>(Func<Unit, T> operation) where E : Exception
        {
            return operation.TryCatch<T, E>();
        }

        /// <summary>
        /// Preferably use Create with an explicit exception handling.
        /// Using this method you are handling all exceptions which you should not do.
        /// Returns Exceptional of result or error or can be empty. Indicates that the operation can throw specified exception.
        /// </summary>
        public static Exc<T, Exception> Create<T>(Func<Unit, T> operation)
        {
            return operation.TryCatch<T, Exception>();
        }

        /// <summary>
        /// Returns Exceptional of result or error or can be empty. Indicates that the operation can throw specified exception.
        /// It will fail on unhandled exceptions.
        /// </summary>
        public static async Task<Exc<T, E>> CreateAsync<T, E>(Func<Unit, Task<T>> operation) where E : Exception
        {
            return await operation.TryCatchAsync<T, E>().ConfigureAwait(false);
        }

        /// <summary>
        /// Preferably use Create with an explicit exception handling.
        /// Using this method you are handling all exceptions which you should not do.
        /// Returns Exceptional of result or error or can be empty. Indicates that the operation can throw specified exception.
        /// </summary>
        public static async Task<Exc<T, Exception>> CreateAsync<T>(Func<Unit, Task<T>> operation)
        {
            return await operation.TryCatchAsync<T, Exception>().ConfigureAwait(false);
        }

        /// <summary>
        /// Creates failed Exc.
        /// </summary>
        public static Exc<T, E> Failure<T, E>(EnumerableException<E> exception) where E : Exception
        {
            return new Exc<T, E>(exception);
        }

        /// <summary>
        /// Creates successful Exc.
        /// </summary>
        public static Exc<T, E> Success<T, E>(T result) where E : Exception
        {
            return new Exc<T, E>(result);
        }

        /// <summary>
        /// Creates empty Exc.
        /// </summary>
        public static Exc<T, E> Empty<T, E>() where E : Exception
        {
            return new Exc<T, E>();
        }
    }

    /// <summary>
    /// Exceptional monad.
    /// Can represent successful result, error (in a form of EnumerableException of a specified exception type) or empty value.
    /// </summary>
    public sealed class Exc<T, E> : OneOf<T, EnumerableException<E>> where E : Exception
    {
        internal Exc()
        {
        }

        internal Exc(T result)
            : base(result)
        {
        }

        internal Exc(E exception)
            : base(exception.ToEnumerableException(exception?.Message))
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

        /// <summary>
        /// Structure-preserving map.
        /// Continuation on successful result.
        /// Maps successful Exc to the new Exc specified by the selector. Otherwise returns failed Exc.
        /// Use FlatMap if you have nested Exc. 
        /// </summary>
        public Exc<R, E> Map<R>(Func<T, R> selector) => FlatMap(v => Exc.Create<R, E>(_ => selector(v)));

        /// <summary>
        /// Structure-preserving map.
        /// Continuation on successful result.
        /// Maps Task of successful Exc to the new Exc specified by the selector. Otherwise returns failed Exc.
        /// Use FlatMap if you have nested Exc. 
        /// </summary>
        public async Task<Exc<R, E>> MapAsync<R>(Func<T, Task<R>> selector) => await FlatMapAsync(v => Exc.CreateAsync<R, E>(_ => selector(v))).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map.
        /// Continuation on successful result.
        /// Maps successful Exc to the new Exc specified by the selector. Otherwise returns failed Exc.
        /// </summary>
        public Exc<R, E> FlatMap<R>(Func<T, Exc<R, E>> selector) => Match(_ => Exc.Empty<R, E>(), selector, Exc.Failure<R, E>);

        /// <summary>
        /// Structure-preserving map.
        /// Continuation on successful result.
        /// Maps Task of successful Exc to the new Exc specified by the selector. Otherwise returns failed Exc.
        /// </summary>
        public async Task<Exc<R, E>> FlatMapAsync<R>(Func<T, Task<Exc<R, E>>> selector)
        {
            switch (Discriminator)
            {
                case 1: return await selector((T)Value).ConfigureAwait(false);
                case 2: return Exc.Failure<R, E>((EnumerableException<E>)Value);
                default: return Exc.Empty<R, E>();
            }
        }

        [Pure]
        public Maybe<E> RootFailure => Failure.FlatMap(e => e.Root);

        [Pure]
        public bool IsSuccess => IsFirst;

        [Pure]
        public bool IsFailure => IsSecond;

        public static implicit operator Exc<T, E>(Unit unit) => Exc.Empty<T, E>();

        public static implicit operator Exc<T, E>(T result) => new Exc<T, E>(result);

        public static implicit operator Exc<T, E>(E exception) => new Exc<T, E>(exception);

        public static implicit operator Exc<T, E>(EnumerableException<E> exception) => new Exc<T, E>(exception);
    }
}
