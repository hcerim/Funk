using System;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Funk.Internal;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Type that represents a possible failure.
    /// Can represent successful result, error (in a form of EnumerableException of a specified exception type) or empty value.
    /// </summary>
    public static class Exc
    {
        /// <summary>
        /// Returns Exc of result or error or can be empty. Indicates that the operation can throw specified exception.
        /// It will fail on unhandled exceptions.
        /// </summary>
        public static Exc<T, E> Create<T, E>(Func<Unit, T> operation) where E : Exception => operation.TryCatch<T, E>();

        /// <summary>
        /// Preferably use Create with an explicit exception handling.
        /// Using this method you are handling all exceptions which you should not do.
        /// Returns Exc of result or error or can be empty. Indicates that the operation can throw specified exception.
        /// </summary>
        public static Exc<T, Exception> Create<T>(Func<Unit, T> operation) => operation.TryCatch<T, Exception>();

        /// <summary>
        /// Returns Exc of result or error or can be empty. Indicates that the operation can throw specified exception.
        /// It will fail on unhandled exceptions.
        /// </summary>
        public static Task<Exc<T, E>> CreateAsync<T, E>(Func<Unit, Task<T>> operation) where E : Exception => operation.TryCatchAsync<T, E>();

        /// <summary>
        /// Preferably use Create with an explicit exception handling.
        /// Using this method you are handling all exceptions which you should not do.
        /// Returns Exc of result or error or can be empty. Indicates that the operation can throw specified exception.
        /// </summary>
        public static Task<Exc<T, Exception>> CreateAsync<T>(Func<Unit, Task<T>> operation) => operation.TryCatchAsync<T, Exception>();

        /// <summary>
        /// Creates failed Exc.
        /// </summary>
        [Pure]
        public static Exc<T, E> Failure<T, E>(EnumerableException<E> exception) where E : Exception => new Exc<T, E>(exception);

        /// <summary>
        /// Creates failed Exc.
        /// </summary>
        [Pure]
        public static Exc<T, E> Failure<T, E>(E exception) where E : Exception => new Exc<T, E>(exception);

        /// <summary>
        /// Creates successful Exc.
        /// </summary>
        [Pure]
        public static Exc<T, E> Success<T, E>(T result) where E : Exception => new Exc<T, E>(result);

        /// <summary>
        /// Creates empty Exc.
        /// </summary>
        [Pure]
        public static Exc<T, E> Empty<T, E>() where E : Exception => new Exc<T, E>();
    }

    /// <summary>
    /// Type that represents a possible failure.
    /// Can represent successful result, error (in a form of EnumerableException of a specified exception type) or empty value.
    /// </summary>
    public readonly struct Exc<T, E> : IEquatable<Exc<T, E>> where E : Exception
    {
        internal Exc(T result)
        {
            if (result.IsNull())
            {
                NotEmpty = false;
                Discriminator = 0;
            }
            else
            {
                NotEmpty = true;
                Discriminator = 1;
            }
            Value = result;
        }

        internal Exc(E exception)
        {
            if (exception.IsNull())
            {
                NotEmpty = false;
                Discriminator = 0;
            }
            else
            {
                NotEmpty = true;
                Discriminator = 2;
            }
            Value = exception.ToEnumerableException(exception.Message);
        }

        internal Exc(EnumerableException<E> exception)
        {
            if (exception.IsNull())
            {
                NotEmpty = false;
                Discriminator = 0;
            }
            else
            {
                NotEmpty = true;
                Discriminator = 2;
            }
            Value = exception;
        }
        
        private int Discriminator { get; }
        private object Value { get; }

        public bool IsEmpty => !NotEmpty;
        public bool NotEmpty { get; }
        
        /// <summary>
        /// Maybe of Success. If it is not success, Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T> Success => Discriminator.SafeEquals(1) ? Maybe.Create((T)Value) : empty;

        /// <summary>
        /// Maybe of Failure. If it is not failure, Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<EnumerableException<E>> Failure => Discriminator.SafeEquals(2) ? Maybe.Create((EnumerableException<E>)Value) : empty;

        /// <summary>
        /// If Failure, Maybe contains the root exception inside EnumerableException if there is one. Otherwise, Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<E> RootFailure => Failure.FlatMap(e => e.Root);

        /// <summary>
        /// If Failure, Maybe contains nested exceptions inside EnumerableException if there are any. Otherwise, Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<IImmutableList<E>> NestedFailures => Failure.FlatMap(e => e.Nested);

        public bool IsSuccess => Success.NotEmpty;

        public bool IsFailure => Failure.NotEmpty;

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
        public Task<Exc<R, E>> MapAsync<R>(Func<T, Task<R>> selector) => FlatMapAsync(v => Exc.CreateAsync<R, E>(_ => selector(v)));

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
        public Task<Exc<R, E>> FlatMapAsync<R>(Func<T, Task<Exc<R, E>>> selector) => Match(_ => result(Exc.Empty<R, E>()), selector, e => result(Exc.Failure<R, E>(e)));
        
        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T, R> ifSuccess, Func<EnumerableException<E>, R> ifFailure)
        {
            var value = Value;
            return Discriminator.Match(
                0, _ => ifEmpty(Unit.Value),
                1, _ => ifSuccess((T)value),
                2, _ => ifFailure((EnumerableException<E>)value)
            );
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T, R> ifSuccess, Func<EnumerableException<E>, R> ifFailure, Func<Unit, Exception> otherwiseThrow = null)
        {
            var value = Value;
            return Discriminator.Match(
                1, _ => ifSuccess((T)value),
                2, _ => ifFailure((EnumerableException<E>)value),
                otherwiseThrow: _ => GetException("Every", otherwiseThrow)
            );
        }

        /// <summary>
        /// Executes operation provided with available item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T> ifSuccess = null, Action<EnumerableException<E>> ifFailure = null)
        {
            var value = Value;
            Discriminator.Match(
                0, _ => ifEmpty?.Apply(Unit.Value),
                1, _ => ifSuccess?.Apply((T)value),
                2, _ => ifFailure?.Apply((EnumerableException<E>)value)
            );
        }

        /// <summary>
        /// Returns success or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T UnsafeGetSuccess(Func<Unit, Exception> otherwiseThrow = null)
        {
            var value = Value;
            return Discriminator.Match(
                1, _ => (T)value,
                otherwiseThrow: _ => GetException("First", otherwiseThrow)
            );
        }

        /// <summary>
        /// Returns failure or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public EnumerableException<E> UnsafeGetFailure(Func<Unit, Exception> otherwiseThrow = null)
        {
            var value = Value;
            return Discriminator.Match(
                2, _ => (EnumerableException<E>)value,
                otherwiseThrow: _ => GetException("Second", otherwiseThrow)
            );
        }

        public void Deconstruct(out Maybe<T> success, out Maybe<EnumerableException<E>> failure)
        {
            success = Success;
            failure = Failure;
        }

        public static implicit operator Exc<T, E>(Unit unit) => Exc.Empty<T, E>();

        public static implicit operator Exc<T, E>(T result) => new Exc<T, E>(result);

        public static implicit operator Exc<T, E>(E exception) => new Exc<T, E>(exception);

        public static implicit operator Exc<T, E>(EnumerableException<E> exception) => new Exc<T, E>(exception);

        public static bool operator ==(Exc<T, E> exc, Exc<T, E> other) => exc.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        public static bool operator !=(Exc<T, E> exc, Exc<T, E> other) => !(exc == other);

        public bool Equals(Exc<T, E> other)
        {
            var value = this;
            return other.AsMaybe().FlatMap(o => value.Match(
                _ => o.IsEmpty.AsMaybe(),
                v => o.Success.Map(s => v.SafeEquals(s)),
                e => o.Failure.Map(e.SafeEquals)
            )).GetOrDefault();
        }

        public override bool Equals(object obj) => obj.SafeCast<Exc<T, E>>().Map(Equals).GetOrDefault();

        public override int GetHashCode() => Match(_ => _.GetHashCode(), v => v.GetHashCode(), e => e.GetHashCode());

        public override string ToString() => Match(_ => _.ToString(), v => v.ToString(), e => e.ToString());
        
        private static Exception GetException(string itemName, Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwiseThrow.AsMaybe().Match(
                _ => new EmptyValueException($"{itemName} item is empty."),
                o => o(Unit.Value)
            );
        }
    }
}
