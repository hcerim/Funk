using System;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    /// <summary>
    /// Type that holds no value but provides empty value handling.
    /// Base type for any disjoint union implementation.
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
        }

        [Pure]
        protected static Exception GetException(string itemName, Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwiseThrow.IsNull() ? new EmptyValueException($"{itemName} item is empty.") : otherwiseThrow(Unit.Value);
        }

        [Pure]
        public bool IsEmpty => !NotEmpty;
        public bool NotEmpty { get; }
        protected int Discriminator { get; }
    }

    /// <summary>
    /// Type that represents one of the 3 possible values (T1, T2 or Empty).
    /// </summary>
    public class OneOf<T1, T2> : OneOf
    {
        [Pure]
        public static OneOf<T1, T2> Empty => new OneOf<T1, T2>();

        private OneOf()
            : base(default, 0)
        {
        }

        public OneOf(T1 t1)
            : base(t1, 1)
        {
            _first = t1;
        }

        public OneOf(T2 t2)
            : base(t2, 2)
        {
            _second = t2;
        }

        private readonly T1 _first;
        private readonly T2 _second;

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                case 2: return ifSecond(_second);
                default: return ifEmpty(Unit.Value);
            }
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<Unit, Exception> otherwiseThrow = null)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                case 2: return ifSecond(_second);
                default: throw GetException("Every", otherwiseThrow);
            }
        }

        /// <summary>
        /// Executes operation provided with available item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null)
        {
            switch (Discriminator)
            {
                case 1:
                    ifFirst?.Invoke(_first);
                    break;
                case 2:
                    ifSecond?.Invoke(_second);
                    break;
                default:
                    ifEmpty?.Invoke(Unit.Value);
                    break;
            }
        }

        /// <summary>
        /// Returns first item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_first is null | Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return _first;
        }

        /// <summary>
        /// Returns second item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_second is null | Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return _second;
        }
    }

    /// <summary>
    /// Type that represents one of the 4 possible values (T1, T2, T3 or Empty).
    /// </summary>
    public class OneOf<T1, T2, T3> : OneOf
    {
        [Pure]
        public static OneOf<T1, T2, T3> Empty => new OneOf<T1, T2, T3>();

        private OneOf()
            : base(default, 0)
        {
        }

        public OneOf(T1 t1)
            : base(t1, 1)
        {
            _first = t1;
        }

        public OneOf(T2 t2)
            : base(t2, 2)
        {
            _second = t2;
        }

        public OneOf(T3 t3)
            : base(t3, 3)
        {
            _third = t3;
        }

        private readonly T1 _first;
        private readonly T2 _second;
        private readonly T3 _third;

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                case 2: return ifSecond(_second);
                case 3: return ifThird(_third);
                default: return ifEmpty(Unit.Value);
            }
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<Unit, Exception> otherwiseThrow = null)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                case 2: return ifSecond(_second);
                case 3: return ifThird(_third);
                default: throw GetException("Every", otherwiseThrow);
            }
        }

        /// <summary>
        /// Executes operation provided with available item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null, Action<T3> ifThird = null)
        {
            switch (Discriminator)
            {
                case 1:
                    ifFirst?.Invoke(_first);
                    break;
                case 2:
                    ifSecond?.Invoke(_second);
                    break;
                case 3:
                    ifThird?.Invoke(_third);
                    break;
                default:
                    ifEmpty?.Invoke(Unit.Value);
                    break;
            }
        }

        /// <summary>
        /// Returns first item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_first is null | Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return _first;
        }

        /// <summary>
        /// Returns second item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_second is null | Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return _second;
        }

        /// <summary>
        /// Returns third item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_third is null | Discriminator.SafeNotEquals(3))
            {
                throw GetException("Third", otherwiseThrow);
            }

            return _third;
        }
    }

    /// <summary>
    /// Type that represents one of the 5 possible values (T1, T2, T3, T4 or Empty).
    /// </summary>
    public class OneOf<T1, T2, T3, T4> : OneOf
    {
        [Pure]
        public static OneOf<T1, T2, T3, T4> Empty => new OneOf<T1, T2, T3, T4>();

        private OneOf()
            : base(default, 0)
        {
        }

        public OneOf(T1 t1)
            : base(t1, 1)
        {
            _first = t1;
        }

        public OneOf(T2 t2)
            : base(t2, 2)
        {
            _second = t2;
        }

        public OneOf(T3 t3)
            : base(t3, 3)
        {
            _third = t3;
        }

        public OneOf(T4 t4)
            : base(t4, 4)
        {
            _fourth = t4;
        }

        private readonly T1 _first;
        private readonly T2 _second;
        private readonly T3 _third;
        private readonly T4 _fourth;

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                case 2: return ifSecond(_second);
                case 3: return ifThird(_third);
                case 4: return ifFourth(_fourth);
                default: return ifEmpty(Unit.Value);
            }
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth, Func<Unit, Exception> otherwiseThrow = null)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                case 2: return ifSecond(_second);
                case 3: return ifThird(_third);
                case 4: return ifFourth(_fourth);
                default: throw GetException("Every", otherwiseThrow);
            }
        }

        /// <summary>
        /// Executes operation provided with available item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null, Action<T3> ifThird = null, Action<T4> ifFourth = null)
        {
            switch (Discriminator)
            {
                case 1:
                    ifFirst?.Invoke(_first);
                    break;
                case 2:
                    ifSecond?.Invoke(_second);
                    break;
                case 3:
                    ifThird?.Invoke(_third);
                    break;
                case 4:
                    ifFourth?.Invoke(_fourth);
                    break;
                default:
                    ifEmpty?.Invoke(Unit.Value);
                    break;
            }
        }

        /// <summary>
        /// Returns first item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_first is null | Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return _first;
        }

        /// <summary>
        /// Returns second item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_second is null | Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return _second;
        }

        /// <summary>
        /// Returns third item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_third is null | Discriminator.SafeNotEquals(3))
            {
                throw GetException("Third", otherwiseThrow);
            }

            return _third;
        }

        /// <summary>
        /// Returns fourth item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T4 UnsafeGetFourth(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_fourth is null | Discriminator.SafeNotEquals(4))
            {
                throw GetException("Fourth", otherwiseThrow);
            }

            return _fourth;
        }
    }

    /// <summary>
    /// Type that represents one of the 6 possible values (T1, T2, T3, T4, T5 or Empty).
    /// </summary>
    public class OneOf<T1, T2, T3, T4, T5> : OneOf
    {
        [Pure]
        public static OneOf<T1, T2, T3, T4, T5> Empty => new OneOf<T1, T2, T3, T4, T5>();

        private OneOf()
            : base(default, 0)
        {
        }

        public OneOf(T1 t1)
            : base(t1, 1)
        {
            _first = t1;
        }

        public OneOf(T2 t2)
            : base(t2, 2)
        {
            _second = t2;
        }

        public OneOf(T3 t3)
            : base(t3, 3)
        {
            _third = t3;
        }

        public OneOf(T4 t4)
            : base(t4, 4)
        {
            _fourth = t4;
        }

        public OneOf(T5 t5)
            : base(t5, 5)
        {
            _fifth = t5;
        }

        private readonly T1 _first;
        private readonly T2 _second;
        private readonly T3 _third;
        private readonly T4 _fourth;
        private readonly T5 _fifth;

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth, Func<T5, R> ifFifth)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                case 2: return ifSecond(_second);
                case 3: return ifThird(_third);
                case 4: return ifFourth(_fourth);
                case 5: return ifFifth(_fifth);
                default: return ifEmpty(Unit.Value);
            }
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public R Match<R>(Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth, Func<T5, R> ifFifth, Func<Unit, Exception> otherwiseThrow = null)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                case 2: return ifSecond(_second);
                case 3: return ifThird(_third);
                case 4: return ifFourth(_fourth);
                case 5: return ifFifth(_fifth);
                default: throw GetException("Every", otherwiseThrow);
            }
        }

        /// <summary>
        /// Executes operation provided with available item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T1> ifFirst = null, Action<T2> ifSecond = null, Action<T3> ifThird = null, Action<T4> ifFourth = null, Action<T5> ifFifth = null)
        {
            switch (Discriminator)
            {
                case 1:
                    ifFirst?.Invoke(_first);
                    break;
                case 2:
                    ifSecond?.Invoke(_second);
                    break;
                case 3:
                    ifThird?.Invoke(_third);
                    break;
                case 4:
                    ifFourth?.Invoke(_fourth);
                    break;
                case 5:
                    ifFifth?.Invoke(_fifth);
                    break;
                default:
                    ifEmpty?.Invoke(Unit.Value);
                    break;
            }
        }

        /// <summary>
        /// Returns first item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_first is null | Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return _first;
        }

        /// <summary>
        /// Returns second item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_second is null | Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return _second;
        }

        /// <summary>
        /// Returns third item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_third is null | Discriminator.SafeNotEquals(3))
            {
                throw GetException("Third", otherwiseThrow);
            }

            return _third;
        }

        /// <summary>
        /// Returns fourth item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T4 UnsafeGetFourth(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_fourth is null | Discriminator.SafeNotEquals(4))
            {
                throw GetException("Fourth", otherwiseThrow);
            }

            return _fourth;
        }

        /// <summary>
        /// Returns fifth item or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T5 UnsafeGetFifth(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_fifth is null | Discriminator.SafeNotEquals(5))
            {
                throw GetException("Fifth", otherwiseThrow);
            }

            return _fifth;
        }
    }
}
