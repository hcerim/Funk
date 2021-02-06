using System;
using System.Diagnostics.Contracts;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Base type for any coproduct implementation.
    /// </summary>
    public abstract class OneOf
    {
        /// <summary>
        /// Creates a new OneOf object.
        /// In case the specified object is empty, discriminator will default to 0.
        /// </summary>
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
        /// Represents the state of the underlying object.
        /// In case the object is empty, it will be 0.
        /// </summary>
        [Pure]
        protected int Discriminator { get; }
        
        /// <summary>
        /// Underlying value of the coproduct.
        /// It should never be accessed directly without first checking the Discriminator value!
        /// </summary>
        [Pure]
        protected object Value { get; }
        
        internal static Exception GetException(string itemName, Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwiseThrow.AsMaybe().Match(
                _ => new EmptyValueException($"{itemName} item is empty."),
                o => o(Unit.Value)
            );
        }
    }

    /// <summary>
    /// OneOf with arity of 2.
    /// Type that represents a coproduct of three values (including an empty value -> Unit).
    /// Defaults to an empty value.
    /// </summary>
    public class OneOf<T1, T2> : OneOf, IEquatable<OneOf<T1, T2>>
    {
        /// <summary>
        /// Creates the OneOf in the default state.
        /// </summary>
        public OneOf()
            : base(null, 0)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the first state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T1 first)
            : base(first, 1)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the second state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T2 second)
            : base(second, 2)
        {
        }

        /// <summary>
        /// First underlying value.
        /// In case the OneOf object is not in the first state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T1> First => Discriminator.SafeEquals(1) ? Maybe.Create((T1)Value) : empty;

        /// <summary>
        /// Second underlying value.
        /// In case the OneOf object is not in the second state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T2> Second => Discriminator.SafeEquals(2) ? Maybe.Create((T2)Value) : empty;

        /// <summary>
        /// Returns true if the underlying value is in the first state.
        /// </summary>
        [Pure]
        public bool IsFirst => First.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the second state.
        /// </summary>
        [Pure]
        public bool IsSecond => Second.NotEmpty;

        /// <summary>
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function provided with the underlying value will be executed and its result will be returned.
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
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function provided with the underlying value will be executed and its result will be returned.
        /// In case the underlying value is empty, the last function (if specified) returns the exception that is then thrown.
        /// In case the last function is not specified, EmptyValueException is thrown.
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
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function (if specified) provided with the underlying value will be executed.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null)
        {
            Discriminator.Match(
                0, _ => ifEmpty?.Apply(Unit.Value),
                1, _ => ifFirst?.Apply((T1)Value),
                2, _ => ifSecond?.Apply((T2)Value)
            );
        }

        /// <summary>
        /// USE First and then GetOr PREFERABLY!
        /// Returns the underlying value in the first state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                1, _ => (T1)Value,
                otherwiseThrow: _ => GetException("First", otherwiseThrow)
            );

        /// <summary>
        /// USE Second and then GetOr PREFERABLY!
        /// Returns the underlying value in the second state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                2, _ => (T2)Value,
                otherwiseThrow: _ => GetException("Second", otherwiseThrow)
            );

        /// <summary>
        /// Deconstructs the OneOf to underlying values.
        /// In case the underlying value is empty, deconstructed values will be empty.
        /// Otherwise, one of the deconstructed values will be in a non-empty state.
        /// </summary>
        [Pure]
        public void Deconstruct(out Maybe<T1> first, out Maybe<T2> second)
        {
            first = First;
            second = Second;
        }

        /// <summary>
        /// Lifts the Unit to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2>(Unit unit) => new OneOf<T1, T2>();
        
        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2>(T1 t1) => new OneOf<T1, T2>(t1);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2>(T2 t2) => new OneOf<T1, T2>(t2);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator ==(OneOf<T1, T2> oneOf, OneOf<T1, T2> other) => oneOf.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator !=(OneOf<T1, T2> oneOf, OneOf<T1, T2> other) => !(oneOf == other);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public bool Equals(OneOf<T1, T2> other)
        {
            return other.AsMaybe().FlatMap(o => Match(
                _ => o.IsEmpty.AsMaybe(),
                f => o.First.Map(t1 => f.SafeEquals(t1)),
                s => o.Second.Map(t2 => s.SafeEquals(t2))
            )).GetOrDefault();
        }

        /// <summary>
        /// Underlying types' based equality comparison.
        /// If the other object is not OneOf of same types, returns false.
        /// </summary>
        [Pure]
        public override bool Equals(object obj) => obj.SafeCast<OneOf<T1, T2>>().Map(Equals).GetOrDefault();

        /// <summary>
        /// Underlying types' based GetHashCode method.
        /// </summary>
        public override int GetHashCode() => Match(_ => _.GetHashCode(), f => f.GetHashCode(), s => s.GetHashCode());

        /// <summary>
        /// Underlying types' based ToString method.
        /// </summary>
        public override string ToString() => Match(_ => _.ToString(), f => f.ToString(), s => s.ToString());
    }

    /// <summary>
    /// OneOf with arity of 3.
    /// Type that represents a coproduct of four values (including an empty value -> Unit).
    /// Defaults to an empty value.
    /// </summary>
    public class OneOf<T1, T2, T3> : OneOf, IEquatable<OneOf<T1, T2, T3>>
    {
        /// <summary>
        /// Creates the OneOf in the default state.
        /// </summary>
        public OneOf()
            : base(null, 0)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the first state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T1 first)
            : base(first, 1)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the second state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T2 second)
            : base(second, 2)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the third state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T3 third)
            : base(third, 3)
        {
        }

        /// <summary>
        /// First underlying value.
        /// In case the OneOf object is not in the first state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T1> First => Discriminator.SafeEquals(1) ? Maybe.Create((T1)Value) : empty;

        /// <summary>
        /// Second underlying value.
        /// In case the OneOf object is not in the second state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T2> Second => Discriminator.SafeEquals(2) ? Maybe.Create((T2)Value) : empty;

        /// <summary>
        /// Third underlying value.
        /// In case the OneOf object is not in the third state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T3> Third => Discriminator.SafeEquals(3) ? Maybe.Create((T3)Value) : empty;

        /// <summary>
        /// Returns true if the underlying value is in the first state.
        /// </summary>
        [Pure]
        public bool IsFirst => First.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the second state.
        /// </summary>
        [Pure]
        public bool IsSecond => Second.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the third state.
        /// </summary>
        [Pure]
        public bool IsThird => Third.NotEmpty;

        /// <summary>
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function provided with the underlying value will be executed and its result will be returned.
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
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function provided with the underlying value will be executed and its result will be returned.
        /// In case the underlying value is empty, the last function (if specified) returns the exception that is then thrown.
        /// In case the last function is not specified, EmptyValueException is thrown.
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
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function (if specified) provided with the underlying value will be executed.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null, Action<T3> ifThird = null)
        {
            Discriminator.Match(
                0, _ => ifEmpty?.Apply(Unit.Value),
                1, _ => ifFirst?.Apply((T1)Value),
                2, _ => ifSecond?.Apply((T2)Value),
                3, _ => ifThird?.Apply((T3)Value)
            );
        }

        /// <summary>
        /// USE First and then GetOr PREFERABLY!
        /// Returns the underlying value in the first state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                1, _ => (T1)Value,
                otherwiseThrow: _ => GetException("First", otherwiseThrow)
            );

        /// <summary>
        /// USE Second and then GetOr PREFERABLY!
        /// Returns the underlying value in the second state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                2, _ => (T2)Value,
                otherwiseThrow: _ => GetException("Second", otherwiseThrow)
            );

        /// <summary>
        /// USE Third and then GetOr PREFERABLY!
        /// Returns the underlying value in the third state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                3, _ => (T3)Value,
                otherwiseThrow: _ => GetException("Third", otherwiseThrow)
            );
        
        /// <summary>
        /// Deconstructs the OneOf to underlying values.
        /// In case the underlying value is empty, deconstructed values will be empty.
        /// Otherwise, one of the deconstructed values will be in a non-empty state.
        /// </summary>
        [Pure]
        public void Deconstruct(out Maybe<T1> first, out Maybe<T2> second, out Maybe<T3> third)
        {
            first = First;
            second = Second;
            third = Third;
        }
        
        /// <summary>
        /// Lifts the Unit to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3>(Unit unit) => new OneOf<T1, T2, T3>();

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3>(T1 t1) => new OneOf<T1, T2, T3>(t1);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3>(T2 t2) => new OneOf<T1, T2, T3>(t2);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3>(T3 t3) => new OneOf<T1, T2, T3>(t3);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator ==(OneOf<T1, T2, T3> oneOf, OneOf<T1, T2, T3> other) => oneOf.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator !=(OneOf<T1, T2, T3> oneOf, OneOf<T1, T2, T3> other) => !(oneOf == other);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public bool Equals(OneOf<T1, T2, T3> other)
        {
            return other.AsMaybe().FlatMap(o => Match(
                _ => o.IsEmpty.AsMaybe(),
                f => o.First.Map(t1 => f.SafeEquals(t1)),
                s => o.Second.Map(t2 => s.SafeEquals(t2)),
                t => o.Third.Map(t3 => t.SafeEquals(t3))
            )).GetOrDefault();
        }

        /// <summary>
        /// Underlying types' based equality comparison.
        /// If the other object is not OneOf of same types, returns false.
        /// </summary>
        [Pure]
        public override bool Equals(object obj) => obj.SafeCast<OneOf<T1, T2, T3>>().Map(Equals).GetOrDefault();

        /// <summary>
        /// Underlying types' based GetHashCode method.
        /// </summary>
        public override int GetHashCode() => Match(_ => _.GetHashCode(), f => f.GetHashCode(), s => s.GetHashCode(), t => t.GetHashCode());

        /// <summary>
        /// Underlying types' based ToString method.
        /// </summary>
        public override string ToString() => Match(_ => _.ToString(), f => f.ToString(), s => s.ToString(), t => t.ToString());
    }

    /// <summary>
    /// OneOf with arity of 4.
    /// Type that represents a coproduct of five values (including an empty value -> Unit).
    /// Defaults to an empty value.
    /// </summary>
    public class OneOf<T1, T2, T3, T4> : OneOf, IEquatable<OneOf<T1, T2, T3, T4>>
    {
        /// <summary>
        /// Creates the OneOf in the default state.
        /// </summary>
        public OneOf()
            : base(null, 0)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the first state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T1 first)
            : base(first, 1)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the second state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T2 second)
            : base(second, 2)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the third state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T3 third)
            : base(third, 3)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the fourth state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T4 fourth)
            : base(fourth, 4)
        {
        }

        /// <summary>
        /// First underlying value.
        /// In case the OneOf object is not in the first state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T1> First => Discriminator.SafeEquals(1) ? Maybe.Create((T1)Value) : empty;

        /// <summary>
        /// Second underlying value.
        /// In case the OneOf object is not in the second state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T2> Second => Discriminator.SafeEquals(2) ? Maybe.Create((T2)Value) : empty;

        /// <summary>
        /// Third underlying value.
        /// In case the OneOf object is not in the third state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T3> Third => Discriminator.SafeEquals(3) ? Maybe.Create((T3)Value) : empty;

        /// <summary>
        /// Fourth underlying value.
        /// In case the OneOf object is not in the fourth state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T4> Fourth => Discriminator.SafeEquals(4) ? Maybe.Create((T4)Value) : empty;

        /// <summary>
        /// Returns true if the underlying value is in the first state.
        /// </summary>
        [Pure]
        public bool IsFirst => First.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the second state.
        /// </summary>
        [Pure]
        public bool IsSecond => Second.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the third state.
        /// </summary>
        [Pure]
        public bool IsThird => Third.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the fourth state.
        /// </summary>
        [Pure]
        public bool IsFourth => Fourth.NotEmpty;

        /// <summary>
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function provided with the underlying value will be executed and its result will be returned.
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
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function provided with the underlying value will be executed and its result will be returned.
        /// In case the underlying value is empty, the last function (if specified) returns the exception that is then thrown.
        /// In case the last function is not specified, EmptyValueException is thrown.
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
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function (if specified) provided with the underlying value will be executed.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null, Action<T3> ifThird = null, Action<T4> ifFourth = null)
        {
            Discriminator.Match(
                0, _ => ifEmpty?.Apply(Unit.Value),
                1, _ => ifFirst?.Apply((T1)Value),
                2, _ => ifSecond?.Apply((T2)Value),
                3, _ => ifThird?.Apply((T3)Value),
                4, _ => ifFourth?.Apply((T4)Value)
            );
        }

        /// <summary>
        /// USE First and then GetOr PREFERABLY!
        /// Returns the underlying value in the first state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                1, _ => (T1)Value,
                otherwiseThrow: _ => GetException("First", otherwiseThrow)
            );

        /// <summary>
        /// USE Second and then GetOr PREFERABLY!
        /// Returns the underlying value in the second state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                2, _ => (T2)Value,
                otherwiseThrow: _ => GetException("Second", otherwiseThrow)
            );

        /// <summary>
        /// USE Third and then GetOr PREFERABLY!
        /// Returns the underlying value in the third state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                3, _ => (T3)Value,
                otherwiseThrow: _ => GetException("Third", otherwiseThrow)
            );

        /// <summary>
        /// USE Fourth and then GetOr PREFERABLY!
        /// Returns the underlying value in the fourth state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T4 UnsafeGetFourth(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                4, _ => (T4)Value,
                otherwiseThrow: _ => GetException("Fourth", otherwiseThrow)
            );
        
        /// <summary>
        /// Deconstructs the OneOf to underlying values.
        /// In case the underlying value is empty, deconstructed values will be empty.
        /// Otherwise, one of the deconstructed values will be in a non-empty state.
        /// </summary>
        [Pure]
        public void Deconstruct(out Maybe<T1> first, out Maybe<T2> second, out Maybe<T3> third, out Maybe<T4> fourth)
        {
            first = First;
            second = Second;
            third = Third;
            fourth = Fourth;
        }
        
        /// <summary>
        /// Lifts the Unit to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4>(Unit unit) => new OneOf<T1, T2, T3, T4>();

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4>(T1 t1) => new OneOf<T1, T2, T3, T4>(t1);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4>(T2 t2) => new OneOf<T1, T2, T3, T4>(t2);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4>(T3 t3) => new OneOf<T1, T2, T3, T4>(t3);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4>(T4 t4) => new OneOf<T1, T2, T3, T4>(t4);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator ==(OneOf<T1, T2, T3, T4> oneOf, OneOf<T1, T2, T3, T4> other) => oneOf.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator !=(OneOf<T1, T2, T3, T4> oneOf, OneOf<T1, T2, T3, T4> other) => !(oneOf == other);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
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

        /// <summary>
        /// Underlying types' based equality comparison.
        /// If the other object is not OneOf of same types, returns false.
        /// </summary>
        [Pure]
        public override bool Equals(object obj) => obj.SafeCast<OneOf<T1, T2, T3, T4>>().Map(Equals).GetOrDefault();

        /// <summary>
        /// Underlying types' based GetHashCode method.
        /// </summary>
        public override int GetHashCode() => Match(_ => _.GetHashCode(), f => f.GetHashCode(), s => s.GetHashCode(), t => t.GetHashCode(), f => f.GetHashCode());

        /// <summary>
        /// Underlying types' based ToString method.
        /// </summary>
        public override string ToString() => Match(_ => _.ToString(), f => f.ToString(), s => s.ToString(), t => t.ToString(), f => f.ToString());
    }

    /// <summary>
    /// OneOf with arity of 5.
    /// Type that represents a coproduct of six values (including an empty value -> Unit).
    /// Defaults to an empty value.
    /// </summary>
    public class OneOf<T1, T2, T3, T4, T5> : OneOf, IEquatable<OneOf<T1, T2, T3, T4, T5>>
    {
        /// <summary>
        /// Creates the OneOf in the default state.
        /// </summary>
        public OneOf()
            : base(null, 0)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the first state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T1 first)
            : base(first, 1)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the second state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T2 second)
            : base(second, 2)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the third state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T3 third)
            : base(third, 3)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the fourth state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T4 fourth)
            : base(fourth, 4)
        {
        }

        /// <summary>
        /// Creates the OneOf object in the fifth state.
        /// In case the specified value is empty, defaults to an empty OneOf (default state).
        /// </summary>
        public OneOf(T5 fifth)
            : base(fifth, 5)
        {
        }

        /// <summary>
        /// First underlying value.
        /// In case the OneOf object is not in the first state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T1> First => Discriminator.SafeEquals(1) ? Maybe.Create((T1)Value) : empty;

        /// <summary>
        /// Second underlying value.
        /// In case the OneOf object is not in the second state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T2> Second => Discriminator.SafeEquals(2) ? Maybe.Create((T2)Value) : empty;

        /// <summary>
        /// Third underlying value.
        /// In case the OneOf object is not in the third state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T3> Third => Discriminator.SafeEquals(3) ? Maybe.Create((T3)Value) : empty;

        /// <summary>
        /// Fourth underlying value.
        /// In case the OneOf object is not in the fourth state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T4> Fourth => Discriminator.SafeEquals(4) ? Maybe.Create((T4)Value) : empty;

        /// <summary>
        /// Fifth underlying value.
        /// In case the OneOf object is not in the fifth state, returns an empty Maybe.
        /// </summary>
        [Pure]
        public Maybe<T5> Fifth => Discriminator.SafeEquals(5) ? Maybe.Create((T5)Value) : empty;

        /// <summary>
        /// Returns true if the underlying value is in the first state.
        /// </summary>
        [Pure]
        public bool IsFirst => First.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the second state.
        /// </summary>
        [Pure]
        public bool IsSecond => Second.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the third state.
        /// </summary>
        [Pure]
        public bool IsThird => Third.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the fourth state.
        /// </summary>
        [Pure]
        public bool IsFourth => Fourth.NotEmpty;

        /// <summary>
        /// Returns true if the underlying value is in the fifth state.
        /// </summary>
        [Pure]
        public bool IsFifth => Fifth.NotEmpty;

        /// <summary>
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function provided with the underlying value will be executed and its result will be returned.
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
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function provided with the underlying value will be executed and its result will be returned.
        /// In case the underlying value is empty, the last function (if specified) returns the exception that is then thrown.
        /// In case the last function is not specified, EmptyValueException is thrown.
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
        /// Pattern-matches on the underlying values.
        /// Depending on the state of the underlying object,
        /// corresponding function (if specified) provided with the underlying value will be executed.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null, Action<T3> ifThird = null, Action<T4> ifFourth = null, Action<T5> ifFifth = null)
        {
            Discriminator.Match(
                0, _ => ifEmpty?.Apply(Unit.Value),
                1, _ => ifFirst?.Apply((T1)Value),
                2, _ => ifSecond?.Apply((T2)Value),
                3, _ => ifThird?.Apply((T3)Value),
                4, _ => ifFourth?.Apply((T4)Value),
                5, _ => ifFifth?.Apply((T5)Value)
            );
        }

        /// <summary>
        /// USE First and then GetOr PREFERABLY!
        /// Returns the underlying value in the first state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                1, _ => (T1)Value,
                otherwiseThrow: _ => GetException("First", otherwiseThrow)
            );

        /// <summary>
        /// USE Second and then GetOr PREFERABLY!
        /// Returns the underlying value in the second state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                2, _ => (T2)Value,
                otherwiseThrow: _ => GetException("Second", otherwiseThrow)
            );

        /// <summary>
        /// USE Third and then GetOr PREFERABLY!
        /// Returns the underlying value in the third state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                3, _ => (T3)Value,
                otherwiseThrow: _ => GetException("Third", otherwiseThrow)
            );

        /// <summary>
        /// USE Fourth and then GetOr PREFERABLY!
        /// Returns the underlying value in the fourth state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T4 UnsafeGetFourth(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                4, _ => (T4)Value,
                otherwiseThrow: _ => GetException("Fourth", otherwiseThrow)
            );

        /// <summary>
        /// USE Fifth and then GetOr PREFERABLY!
        /// Returns the underlying value in the fifth state or throws exception returned in the specified function.
        /// In case the function is not specified, EmptyValueException is thrown.
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T5 UnsafeGetFifth(Func<Unit, Exception> otherwiseThrow = null) =>
            Discriminator.Match(
                5, _ => (T5)Value,
                otherwiseThrow: _ => GetException("Fifth", otherwiseThrow)
            );
        
        /// <summary>
        /// Deconstructs the OneOf to underlying values.
        /// In case the underlying value is empty, deconstructed values will be empty.
        /// Otherwise, one of the deconstructed values will be in a non-empty state.
        /// </summary>
        [Pure]
        public void Deconstruct(out Maybe<T1> first, out Maybe<T2> second, out Maybe<T3> third, out Maybe<T4> fourth, out Maybe<T5> fifth)
        {
            first = First;
            second = Second;
            third = Third;
            fourth = Fourth;
            fifth = Fifth;
        }
        
        /// <summary>
        /// Lifts the Unit to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(Unit unit) => new OneOf<T1, T2, T3, T4, T5>();

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T1 t1) => new OneOf<T1, T2, T3, T4, T5>(t1);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T2 t2) => new OneOf<T1, T2, T3, T4, T5>(t2);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T3 t3) => new OneOf<T1, T2, T3, T4, T5>(t3);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T4 t4) => new OneOf<T1, T2, T3, T4, T5>(t4);

        /// <summary>
        /// Lifts the object to the OneOf.
        /// </summary>
        [Pure]
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T5 t5) => new OneOf<T1, T2, T3, T4, T5>(t5);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator ==(OneOf<T1, T2, T3, T4, T5> oneOf, OneOf<T1, T2, T3, T4, T5> other) => oneOf.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
        public static bool operator !=(OneOf<T1, T2, T3, T4, T5> oneOf, OneOf<T1, T2, T3, T4, T5> other) => !(oneOf == other);

        /// <summary>
        /// Underlying types' based equality comparison.
        /// </summary>
        [Pure]
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

        /// <summary>
        /// Underlying types' based equality comparison.
        /// If the other object is not OneOf of same types, returns false.
        /// </summary>
        [Pure]
        public override bool Equals(object obj) => obj.SafeCast<OneOf<T1, T2, T3, T4, T5>>().Map(Equals).GetOrDefault();

        /// <summary>
        /// Underlying types' based GetHashCode method.
        /// </summary>
        public override int GetHashCode() => Match(_ => _.GetHashCode(), f => f.GetHashCode(), s => s.GetHashCode(), t => t.GetHashCode(), f => f.GetHashCode(), f => f.GetHashCode());

        /// <summary>
        /// Underlying types' based ToString method.
        /// </summary>
        public override string ToString() => Match(_ => _.ToString(), f => f.ToString(), s => s.ToString(), t => t.ToString(), f => f.ToString(), f => f.ToString());
    }
}
