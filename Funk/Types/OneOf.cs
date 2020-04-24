using System;
using System.Diagnostics.Contracts;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Base type for any disjoint union implementation. Contains the value and empty value handling.
    /// </summary>
    public abstract class OneOf
    {
        protected OneOf(object item, int discriminator)
        {
            if (item.IsNull())
            {
                Discriminator = 0;
            }
            else
            {
                NotEmpty = true;
                Discriminator = discriminator;
            }
            Value = item;
        }

        protected static Exception GetException(string itemName, Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwiseThrow.AsMaybe().Match(
                _ => new EmptyValueException($"{itemName} item is empty."),
                o => o(Unit.Value)
            );
        }

        protected int Discriminator { get; }
        protected object Value { get; }

        public bool IsEmpty => !NotEmpty;
        public bool NotEmpty { get; }
    }

    /// <summary>
    /// Type that represents one of the 3 possible values (T1, T2 or Empty).
    /// </summary>
    public class OneOf<T1, T2> : OneOf, IEquatable<OneOf<T1, T2>>
    {
        public OneOf(T1 first)
            : base(first, 1)
        {
        }

        public OneOf(T2 second)
            : base(second, 2)
        {
        }

        /// <summary>
        /// First value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T1> First => Discriminator.SafeEquals(1) ? Maybe.Create((T1)Value) : empty;

        /// <summary>
        /// Second value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T2> Second => Discriminator.SafeEquals(2) ? Maybe.Create((T2)Value) : empty;

        public bool IsFirst => First.NotEmpty;

        public bool IsSecond => Second.NotEmpty;

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond)
        {
            return Discriminator.Match(
                0, _ => ifEmpty(Unit.Value),
                1, _ => ifFirst((T1)Value),
                2, _ => ifSecond((T2)Value)
            );
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<Unit, Exception> otherwiseThrow = null)
        {
            return Discriminator.Match(
                1, _ => ifFirst((T1)Value),
                2, _ => ifSecond((T2)Value),
                otherwiseThrow: _ => GetException("Every", otherwiseThrow)
            );
        }

        /// <summary>
        /// Executes operation provided with available item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null)
        {
            Discriminator.Match(
                0, _ => ifEmpty?.Invoke(Unit.Value),
                1, _ => ifFirst?.Invoke((T1)Value),
                2, _ => ifSecond?.Invoke((T2)Value)
            );
        }

        /// <summary>
        /// Returns first item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return (T1)Value;
        }

        /// <summary>
        /// Returns second item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return (T2)Value;
        }

        public static implicit operator OneOf<T1, T2>(T1 t1) => new OneOf<T1, T2>(t1);

        public static implicit operator OneOf<T1, T2>(T2 t2) => new OneOf<T1, T2>(t2);

        public static bool operator ==(OneOf<T1, T2> oneOf, OneOf<T1, T2> other) => oneOf.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        public static bool operator !=(OneOf<T1, T2> oneOf, OneOf<T1, T2> other) => !(oneOf == other);

        public bool Equals(OneOf<T1, T2> other)
        {
            return other.AsMaybe().FlatMap(o => Match(
                _ => o.IsEmpty.AsMaybe(),
                f => o.First.Map(t1 => f.SafeEquals(t1)),
                s => o.Second.Map(t2 => s.SafeEquals(t2))
            )).GetOrDefault();
        }

        public override bool Equals(object obj) => obj.SafeCast<OneOf<T1, T2>>().Map(Equals).GetOrDefault();

        public override int GetHashCode() => Match(_ => _.GetHashCode(), f => f.GetHashCode(), s => s.GetHashCode());

        public override string ToString() => Match(_ => _.ToString(), f => f.ToString(), s => s.ToString());
    }

    /// <summary>
    /// Type that represents one of the 4 possible values (T1, T2, T3 or Empty).
    /// </summary>
    public class OneOf<T1, T2, T3> : OneOf, IEquatable<OneOf<T1, T2, T3>>
    {
        public OneOf(T1 first)
            : base(first, 1)
        {
        }

        public OneOf(T2 second)
            : base(second, 2)
        {
        }

        public OneOf(T3 third)
            : base(third, 3)
        {
        }

        /// <summary>
        /// First value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T1> First => Discriminator.SafeEquals(1) ? Maybe.Create((T1)Value) : empty;

        /// <summary>
        /// Second value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T2> Second => Discriminator.SafeEquals(2) ? Maybe.Create((T2)Value) : empty;

        /// <summary>
        /// Third value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T3> Third => Discriminator.SafeEquals(3) ? Maybe.Create((T3)Value) : empty;

        public bool IsFirst => First.NotEmpty;

        public bool IsSecond => Second.NotEmpty;

        public bool IsThird => Third.NotEmpty;

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird)
        {
            return Discriminator.Match(
                0, _ => ifEmpty(Unit.Value),
                1, _ => ifFirst((T1)Value),
                2, _ => ifSecond((T2)Value),
                3, _ => ifThird((T3)Value)
            );
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<Unit, Exception> otherwiseThrow = null)
        {
            return Discriminator.Match(
                1, _ => ifFirst((T1)Value),
                2, _ => ifSecond((T2)Value),
                3, _ => ifThird((T3)Value),
                otherwiseThrow: _ => GetException("Every", otherwiseThrow)
            );
        }

        /// <summary>
        /// Executes operation provided with available item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null, Action<T3> ifThird = null)
        {
            Discriminator.Match(
                0, _ => ifEmpty?.Invoke(Unit.Value),
                1, _ => ifFirst?.Invoke((T1)Value),
                2, _ => ifSecond?.Invoke((T2)Value),
                3, _ => ifThird?.Invoke((T3)Value)
            );
        }

        /// <summary>
        /// Returns first item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return (T1)Value;
        }

        /// <summary>
        /// Returns second item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return (T2)Value;
        }

        /// <summary>
        /// Returns third item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(3))
            {
                throw GetException("Third", otherwiseThrow);
            }

            return (T3)Value;
        }

        public static implicit operator OneOf<T1, T2, T3>(T1 t1) => new OneOf<T1, T2, T3>(t1);

        public static implicit operator OneOf<T1, T2, T3>(T2 t2) => new OneOf<T1, T2, T3>(t2);

        public static implicit operator OneOf<T1, T2, T3>(T3 t3) => new OneOf<T1, T2, T3>(t3);

        public static bool operator ==(OneOf<T1, T2, T3> oneOf, OneOf<T1, T2, T3> other) => oneOf.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        public static bool operator !=(OneOf<T1, T2, T3> oneOf, OneOf<T1, T2, T3> other) => !(oneOf == other);

        public bool Equals(OneOf<T1, T2, T3> other)
        {
            return other.AsMaybe().FlatMap(o => Match(
                _ => o.IsEmpty.AsMaybe(),
                f => o.First.Map(t1 => f.SafeEquals(t1)),
                s => o.Second.Map(t2 => s.SafeEquals(t2)),
                t => o.Third.Map(t3 => t.SafeEquals(t3))
            )).GetOrDefault();
        }

        public override bool Equals(object obj) => obj.SafeCast<OneOf<T1, T2, T3>>().Map(Equals).GetOrDefault();

        public override int GetHashCode() => Match(_ => _.GetHashCode(), f => f.GetHashCode(), s => s.GetHashCode(), t => t.GetHashCode());

        public override string ToString() => Match(_ => _.ToString(), f => f.ToString(), s => s.ToString(), t => t.ToString());
    }

    /// <summary>
    /// Type that represents one of the 5 possible values (T1, T2, T3, T4 or Empty).
    /// </summary>
    public class OneOf<T1, T2, T3, T4> : OneOf, IEquatable<OneOf<T1, T2, T3, T4>>
    {
        public OneOf(T1 first)
            : base(first, 1)
        {
        }

        public OneOf(T2 second)
            : base(second, 2)
        {
        }

        public OneOf(T3 third)
            : base(third, 3)
        {
        }

        public OneOf(T4 fourth)
            : base(fourth, 4)
        {
        }

        /// <summary>
        /// First value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T1> First => Discriminator.SafeEquals(1) ? Maybe.Create((T1)Value) : empty;

        /// <summary>
        /// Second value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T2> Second => Discriminator.SafeEquals(2) ? Maybe.Create((T2)Value) : empty;

        /// <summary>
        /// Third value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T3> Third => Discriminator.SafeEquals(3) ? Maybe.Create((T3)Value) : empty;

        /// <summary>
        /// Fourth value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T4> Fourth => Discriminator.SafeEquals(4) ? Maybe.Create((T4)Value) : empty;

        public bool IsFirst => First.NotEmpty;

        public bool IsSecond => Second.NotEmpty;

        public bool IsThird => Third.NotEmpty;

        public bool IsFourth => Fourth.NotEmpty;

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth)
        {
            return Discriminator.Match(
                0, _ => ifEmpty(Unit.Value),
                1, _ => ifFirst((T1)Value),
                2, _ => ifSecond((T2)Value),
                3, _ => ifThird((T3)Value),
                4, _ => ifFourth((T4)Value)
            );
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth, Func<Unit, Exception> otherwiseThrow = null)
        {
            return Discriminator.Match(
                1, _ => ifFirst((T1)Value),
                2, _ => ifSecond((T2)Value),
                3, _ => ifThird((T3)Value),
                4, _ => ifFourth((T4)Value),
                otherwiseThrow: _ => GetException("Every", otherwiseThrow)
            );
        }

        /// <summary>
        /// Executes operation provided with available item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null, Action<T3> ifThird = null, Action<T4> ifFourth = null)
        {
            Discriminator.Match(
                0, _ => ifEmpty?.Invoke(Unit.Value),
                1, _ => ifFirst?.Invoke((T1)Value),
                2, _ => ifSecond?.Invoke((T2)Value),
                3, _ => ifThird?.Invoke((T3)Value),
                4, _ => ifFourth?.Invoke((T4)Value)
            );
        }

        /// <summary>
        /// Returns first item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return (T1)Value;
        }

        /// <summary>
        /// Returns second item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return (T2)Value;
        }

        /// <summary>
        /// Returns third item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(3))
            {
                throw GetException("Third", otherwiseThrow);
            }

            return (T3)Value;
        }

        /// <summary>
        /// Returns fourth item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T4 UnsafeGetFourth(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(4))
            {
                throw GetException("Fourth", otherwiseThrow);
            }

            return (T4)Value;
        }

        public static implicit operator OneOf<T1, T2, T3, T4>(T1 t1) => new OneOf<T1, T2, T3, T4>(t1);

        public static implicit operator OneOf<T1, T2, T3, T4>(T2 t2) => new OneOf<T1, T2, T3, T4>(t2);

        public static implicit operator OneOf<T1, T2, T3, T4>(T3 t3) => new OneOf<T1, T2, T3, T4>(t3);

        public static implicit operator OneOf<T1, T2, T3, T4>(T4 t4) => new OneOf<T1, T2, T3, T4>(t4);

        public static bool operator ==(OneOf<T1, T2, T3, T4> oneOf, OneOf<T1, T2, T3, T4> other) => oneOf.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        public static bool operator !=(OneOf<T1, T2, T3, T4> oneOf, OneOf<T1, T2, T3, T4> other) => !(oneOf == other);

        public bool Equals(OneOf<T1, T2, T3, T4> other)
        {
            return other.AsMaybe().FlatMap(o => Match(
                _ => o.IsEmpty.AsMaybe(),
                f => o.First.Map(t1 => f.SafeEquals(t1)),
                s => o.Second.Map(t2 => s.SafeEquals(t2)),
                t => o.Third.Map(t3 => t.SafeEquals(t3)),
                f => o.Fourth.Map(t4 => f.SafeEquals(t4))
            )).GetOrDefault();
        }

        public override bool Equals(object obj) => obj.SafeCast<OneOf<T1, T2, T3, T4>>().Map(Equals).GetOrDefault();

        public override int GetHashCode() => Match(_ => _.GetHashCode(), f => f.GetHashCode(), s => s.GetHashCode(), t => t.GetHashCode(), f => f.GetHashCode());

        public override string ToString() => Match(_ => _.ToString(), f => f.ToString(), s => s.ToString(), t => t.ToString(), f => f.ToString());
    }

    /// <summary>
    /// Type that represents one of the 6 possible values (T1, T2, T3, T4, T5 or Empty).
    /// </summary>
    public class OneOf<T1, T2, T3, T4, T5> : OneOf, IEquatable<OneOf<T1, T2, T3, T4, T5>>
    {
        public OneOf(T1 first)
            : base(first, 1)
        {
        }

        public OneOf(T2 second)
            : base(second, 2)
        {
        }

        public OneOf(T3 third)
            : base(third, 3)
        {
        }

        public OneOf(T4 fourth)
            : base(fourth, 4)
        {
        }

        public OneOf(T5 fifth)
            : base(fifth, 5)
        {
        }

        /// <summary>
        /// First value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T1> First => Discriminator.SafeEquals(1) ? Maybe.Create((T1)Value) : empty;

        /// <summary>
        /// Second value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T2> Second => Discriminator.SafeEquals(2) ? Maybe.Create((T2)Value) : empty;

        /// <summary>
        /// Third value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T3> Third => Discriminator.SafeEquals(3) ? Maybe.Create((T3)Value) : empty;

        /// <summary>
        /// Fourth value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T4> Fourth => Discriminator.SafeEquals(4) ? Maybe.Create((T4)Value) : empty;

        /// <summary>
        /// Fifth value of the OneOf as Maybe. If it is empty Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<T5> Fifth => Discriminator.SafeEquals(5) ? Maybe.Create((T5)Value) : empty;

        public bool IsFirst => First.NotEmpty;

        public bool IsSecond => Second.NotEmpty;

        public bool IsThird => Third.NotEmpty;

        public bool IsFourth => Fourth.NotEmpty;

        public bool IsFifth => Fifth.NotEmpty;

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth, Func<T5, R> ifFifth)
        {
            return Discriminator.Match(
                0, _ => ifEmpty(Unit.Value),
                1, _ => ifFirst((T1)Value),
                2, _ => ifSecond((T2)Value),
                3, _ => ifThird((T3)Value),
                4, _ => ifFourth((T4)Value),
                5, _ => ifFifth((T5)Value)
            );
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth, Func<T5, R> ifFifth, Func<Unit, Exception> otherwiseThrow = null)
        {
            return Discriminator.Match(
                1, _ => ifFirst((T1)Value),
                2, _ => ifSecond((T2)Value),
                3, _ => ifThird((T3)Value),
                4, _ => ifFourth((T4)Value),
                5, _ => ifFifth((T5)Value),
                otherwiseThrow: _ => GetException("Every", otherwiseThrow)
            );
        }

        /// <summary>
        /// Executes operation provided with available item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null, Action<T3> ifThird = null, Action<T4> ifFourth = null, Action<T5> ifFifth = null)
        {
            Discriminator.Match(
                0, _ => ifEmpty?.Invoke(Unit.Value),
                1, _ => ifFirst?.Invoke((T1)Value),
                2, _ => ifSecond?.Invoke((T2)Value),
                3, _ => ifThird?.Invoke((T3)Value),
                4, _ => ifFourth?.Invoke((T4)Value),
                5, _ => ifFifth?.Invoke((T5)Value)
            );
        }

        /// <summary>
        /// Returns first item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return (T1)Value;
        }

        /// <summary>
        /// Returns second item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return (T2)Value;
        }

        /// <summary>
        /// Returns third item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(3))
            {
                throw GetException("Third", otherwiseThrow);
            }

            return (T3)Value;
        }

        /// <summary>
        /// Returns fourth item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T4 UnsafeGetFourth(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(4))
            {
                throw GetException("Fourth", otherwiseThrow);
            }

            return (T4)Value;
        }

        /// <summary>
        /// Returns fifth item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T5 UnsafeGetFifth(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (Discriminator.SafeNotEquals(5))
            {
                throw GetException("Fifth", otherwiseThrow);
            }

            return (T5)Value;
        }

        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T1 t1) => new OneOf<T1, T2, T3, T4, T5>(t1);

        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T2 t2) => new OneOf<T1, T2, T3, T4, T5>(t2);

        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T3 t3) => new OneOf<T1, T2, T3, T4, T5>(t3);

        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T4 t4) => new OneOf<T1, T2, T3, T4, T5>(t4);

        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T5 t5) => new OneOf<T1, T2, T3, T4, T5>(t5);

        public static bool operator ==(OneOf<T1, T2, T3, T4, T5> oneOf, OneOf<T1, T2, T3, T4, T5> other) => oneOf.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        public static bool operator !=(OneOf<T1, T2, T3, T4, T5> oneOf, OneOf<T1, T2, T3, T4, T5> other) => !(oneOf == other);

        public bool Equals(OneOf<T1, T2, T3, T4, T5> other)
        {
            return other.AsMaybe().FlatMap(o => Match(
                _ => o.IsEmpty.AsMaybe(),
                f => o.First.Map(t1 => f.SafeEquals(t1)),
                s => o.Second.Map(t2 => s.SafeEquals(t2)),
                t => o.Third.Map(t3 => t.SafeEquals(t3)),
                f => o.Fourth.Map(t4 => f.SafeEquals(t4)),
                f => o.Fifth.Map(t5 => f.SafeEquals(t5))
            )).GetOrDefault();
        }

        public override bool Equals(object obj) => obj.SafeCast<OneOf<T1, T2, T3, T4, T5>>().Map(Equals).GetOrDefault();

        public override int GetHashCode() => Match(_ => _.GetHashCode(), f => f.GetHashCode(), s => s.GetHashCode(), t => t.GetHashCode(), f => f.GetHashCode(), f => f.GetHashCode());

        public override string ToString() => Match(_ => _.ToString(), f => f.ToString(), s => s.ToString(), t => t.ToString(), f => f.ToString(), f => f.ToString());
    }
}
