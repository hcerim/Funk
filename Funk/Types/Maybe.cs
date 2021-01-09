using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    public static class Maybe
    {
        /// <summary>
        /// Creates a Maybe of item.
        /// </summary>
        [Pure]
        public static Maybe<T> Create<T>(T item) => new Maybe<T>(item);

        /// <summary>
        /// Creates a Maybe of nullable item.
        /// </summary>
        [Pure]
        public static Maybe<T> Create<T>(T? item) where T : struct => item.IsNull() ? Empty<T>() : new Maybe<T>(item.Value);

        /// <summary>
        /// Creates empty Maybe.
        /// </summary>
        [Pure]
        public static Maybe<T> Empty<T>() => new Maybe<T>();
    }

    /// <summary>
    /// Type that represents the possible absence of data with appropriate handling.
    /// </summary>
    public readonly struct Maybe<T> : IEquatable<Maybe<T>>
    {
        internal Maybe(T item)
        {
            if (item.IsNull())
            {
                NotEmpty = false;
                Discriminator = 0;
            }
            else
            {
                NotEmpty = true;
                Discriminator = 1;
            }
            Value = item;
        }

        private object Value { get; }
        private int Discriminator { get; }

        public bool NotEmpty { get; }
        public bool IsEmpty => !NotEmpty;

        /// <summary>
        /// Maps available Maybe item (value or empty) to the result of the corresponding selector.
        /// </summary>
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T, R> ifNotEmpty)
        {
            var value = Value;
            return Discriminator.Match(
                0, _ => ifEmpty(Unit.Value),
                1, _ => ifNotEmpty((T)value)
            );
        }

        /// <summary>
        /// Maps not empty value to the result of the selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T, R> ifNotEmpty, Func<Unit, Exception> otherwiseThrow = null)
        {
            var value = Value;
            return Discriminator.Match(
                1, _ => ifNotEmpty((T)value),
                otherwiseThrow: _ => GetException(otherwiseThrow)
            );
        }

        /// <summary>
        /// Executes operation provided with available Maybe (value or empty) item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T> ifNotEmpty = null)
        {
            var value = Value;
            Discriminator.Match(
                0, _ => ifEmpty?.Apply(Unit.Value),
                1, _ => ifNotEmpty?.Apply((T)value)
            );
        }

        /// <summary>
        /// Structure-preserving map.
        /// Maps not empty Maybe to the new Maybe of the selector. Otherwise, returns empty Maybe of the selector.
        /// Use FlatMap if you have nested Maybes.
        /// </summary>
        public Maybe<R> Map<R>(Func<T, R> selector) => FlatMap(v => selector(v).AsMaybe());

        /// <summary>
        /// Structure-preserving map.
        /// Maps not empty Maybe to the Task of new Maybe of the selector. Otherwise, returns Task of empty Maybe of the selector.
        /// Use FlatMap if you have nested Maybes.
        /// </summary>
        public Task<Maybe<R>> MapAsync<R>(Func<T, Task<R>> selector) => FlatMapAsync(async v => (await selector(v)).AsMaybe());

        /// <summary>
        /// Structure-preserving map.
        /// Binds not empty Maybe to the new Maybe of the selector. Otherwise, returns empty Maybe of the selector.
        /// </summary>
        public Maybe<R> FlatMap<R>(Func<T, Maybe<R>> selector) => Match(_ => Maybe.Empty<R>(), selector);

        /// <summary>
        /// Structure-preserving map.
        /// Binds not empty Maybe to the Task of new Maybe of the selector. Otherwise, returns Task of empty Maybe of the selector.
        /// </summary>
        public Task<Maybe<R>> FlatMapAsync<R>(Func<T, Task<Maybe<R>>> selector) => Match(_ => result(Maybe.Empty<R>()), selector);

        /// <summary>
        /// Returns not empty value of Maybe or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T UnsafeGet(Func<Unit, Exception> otherwiseThrow = null) => Match(_ => throw GetException(otherwiseThrow), v => v);

        public static implicit operator Maybe<T>(Unit unit) => Maybe.Empty<T>();

        public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);

        public static bool operator ==(Maybe<T> maybe, Maybe<T> other) => maybe.Equals(other);

        public static bool operator !=(Maybe<T> maybe, Maybe<T> other) => !(maybe == other);

        public override string ToString() => Match(_ => _.ToString(), v => v.ToString());

        public bool Equals(Maybe<T> other) => Match(_ => other.IsEmpty, v => other.Match(_ => false, v2 => v.SafeEquals(v2)));

        public override bool Equals(object obj) => Equals(obj.SafeCast<Maybe<T>>().Flatten());

        public override int GetHashCode() => Match(_ => _.GetHashCode(), v => v.GetHashCode());

        private static Exception GetException(Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwiseThrow.AsMaybe().Match(
                _ => new EmptyValueException("Maybe value is empty."),
                o => o(Unit.Value)
            );
        }
    }
}
