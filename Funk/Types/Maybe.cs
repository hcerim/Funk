using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Provides lifting (create) functions for the Maybe type.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Creates a Maybe of object.
        /// </summary>
        [Pure]
        public static Maybe<T> Create<T>(T item) => new Maybe<T>(item);

        /// <summary>
        /// Creates a Maybe of nullable value object.
        /// </summary>
        [Pure]
        public static Maybe<T> Create<T>(T? item) where T : struct => item.IsNull() ? Empty<T>() : new Maybe<T>(item.Value);

        /// <summary>
        /// Creates an empty Maybe.
        /// </summary>
        [Pure]
        public static Maybe<T> Empty<T>() => new Maybe<T>();
    }

    /// <summary>
    /// Type that represents the possible absence of data.
    /// It is a coproduct of 2 values: Unit (if it is empty) and underlying value (if it is not empty).
    /// It defaults to an empty value.
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        private object Value { get; }
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Pure]
        private int Discriminator { get; }

        /// <summary>
        /// Returns true if the underlying value is not empty.
        /// </summary>
        [Pure]
        public bool NotEmpty { get; }
        
        /// <summary>
        /// Returns true if the underlying value is empty.
        /// </summary>
        [Pure]
        public bool IsEmpty => !NotEmpty;

        /// <summary>
        /// Pattern-matches on the underlying values.
        /// In case the underlying value is empty, the first function is executed and its result is returned.
        /// Otherwise, the second function that is provided with the underlying non-empty value is executed and its result is returned.
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
        /// Pattern-matches on the underlying values.
        /// In case the underlying value is not empty, the first function is executed and its result is returned.
        /// Otherwise, the second function is executed which returns the specified exception that is then thrown.
        /// In case the second function is not specified, EmptyValueException is thrown.
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
        /// Pattern-matches on the underlying values.
        /// In case the underlying value is empty, the first function (if specified) is executed.
        /// Otherwise, the second function (if specified) that is provided with the underlying non-empty value is executed.
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
        /// Maps this Maybe to the new Maybe.
        /// If the underlying value is empty, the specified function is not executed and the operation results in an empty Maybe.
        /// Otherwise, the specified function is executed and its result is wrapped in the Maybe indicating the possible absence of returned data.
        /// In case the specified function already returns the Maybe, use FlatMap instead.
        /// </summary>
        public Maybe<R> Map<R>(Func<T, R> selector) => FlatMap(v => selector(v).AsMaybe());

        /// <summary>
        /// Structure-preserving map.
        /// Maps this Maybe to the new Maybe.
        /// If the underlying value is empty, the specified function is not executed and the operation results in an empty Maybe.
        /// Otherwise, the specified function is executed and its result is wrapped in the Maybe indicating the possible absence of returned data.
        /// In case the specified function already returns the Maybe, use FlatMapAsync instead.
        /// </summary>
        public Task<Maybe<R>> MapAsync<R>(Func<T, Task<R>> selector) => FlatMapAsync(async v => (await selector(v)).AsMaybe());

        /// <summary>
        /// Structure-preserving map.
        /// Maps this Maybe to the new Maybe.
        /// If the underlying value is empty, the specified function is not executed and the operation results in an empty Maybe.
        /// Otherwise, the specified function is executed and its result is wrapped in the Maybe indicating the possible absence of returned data.
        /// </summary>
        public Maybe<R> FlatMap<R>(Func<T, Maybe<R>> selector) => Match(_ => Maybe.Empty<R>(), selector);

        /// <summary>
        /// Structure-preserving map.
        /// Maps this Maybe to the new Maybe.
        /// If the underlying value is empty, the specified function is not executed and the operation results in an empty Maybe.
        /// Otherwise, the specified function is executed and its result is wrapped in the Maybe indicating the possible absence of returned data.
        /// </summary>
        public Task<Maybe<R>> FlatMapAsync<R>(Func<T, Task<Maybe<R>>> selector) => Match(_ => result(Maybe.Empty<R>()), selector);

        /// <summary>
        /// USE GetOr PREFERABLY!
        /// Returns the underlying value if not empty or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T UnsafeGet(Func<Unit, Exception> otherwiseThrow = null) => Match(v => v, _ => GetException(otherwiseThrow));

        /// <summary>
        /// Lifts the Unit to the Maybe. 
        /// </summary>
        [Pure]
        public static implicit operator Maybe<T>(Unit unit) => Maybe.Empty<T>();

        /// <summary>
        /// Lifts the object to the Maybe. 
        /// </summary>
        [Pure]
        public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator ==(Maybe<T> maybe, Maybe<T> other) => maybe.Equals(other);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator !=(Maybe<T> maybe, Maybe<T> other) => !(maybe == other);

        /// <summary>
        /// Underlying types' based ToString method.
        /// </summary>
        public override string ToString() => Match(_ => _.ToString(), v => v.ToString());

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public bool Equals(Maybe<T> other) => Match(_ => other.IsEmpty, v => other.Match(_ => false, v2 => v.SafeEquals(v2)));

        /// <summary>
        /// Underlying types' based equality comparison.
        /// If the other object is not Maybe of the same type, returns false.
        /// </summary>
        [Pure]
        public override bool Equals(object obj) => Equals(obj.SafeCast<Maybe<T>>().Flatten());

        /// <summary>
        /// Underlying types' based GetHashCode method.
        /// </summary>
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
