﻿using System;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    /// <summary>
    /// Base type for any disjoint union implementation. Contains the value and empty value handling.
    /// </summary>
    public abstract class OneOf
    {
        [Pure]
        public static Unit Empty => Unit.Value;

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

        [Pure]
        protected static Exception GetException(string itemName, Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwiseThrow.IsNull() ? new EmptyValueException($"{itemName} item is empty.") : otherwiseThrow(Unit.Value);
        }

        [Pure]
        public bool IsEmpty => !NotEmpty;
        public bool NotEmpty { get; }
        protected int Discriminator { get; }
        protected object Value { get; }
    }

    /// <summary>
    /// Type that represents one of the 3 possible values (T1, T2 or Empty).
    /// </summary>
    public class OneOf<T1, T2> : OneOf
    {
        private OneOf()
            : base(default, 0)
        {
        }

        public OneOf(T1 t1)
            : base(t1, 1)
        {
        }

        public OneOf(T2 t2)
            : base(t2, 2)
        {
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst((T1)Value);
                case 2: return ifSecond((T2)Value);
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
                case 1: return ifFirst((T1)Value);
                case 2: return ifSecond((T2)Value);
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
                    ifFirst?.Invoke((T1)Value);
                    break;
                case 2:
                    ifSecond?.Invoke((T2)Value);
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

        public static implicit operator OneOf<T1, T2>(Unit unit) => new OneOf<T1, T2>();
    }

    /// <summary>
    /// Type that represents one of the 4 possible values (T1, T2, T3 or Empty).
    /// </summary>
    public class OneOf<T1, T2, T3> : OneOf
    {
        private OneOf()
            : base(default, 0)
        {
        }

        public OneOf(T1 t1)
            : base(t1, 1)
        {
        }

        public OneOf(T2 t2)
            : base(t2, 2)
        {
        }

        public OneOf(T3 t3)
            : base(t3, 3)
        {
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst((T1)Value);
                case 2: return ifSecond((T2)Value);
                case 3: return ifThird((T3)Value);
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
                case 1: return ifFirst((T1)Value);
                case 2: return ifSecond((T2)Value);
                case 3: return ifThird((T3)Value);
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
                    ifFirst?.Invoke((T1)Value);
                    break;
                case 2:
                    ifSecond?.Invoke((T2)Value);
                    break;
                case 3:
                    ifThird?.Invoke((T3)Value);
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

        public static implicit operator OneOf<T1, T2, T3>(Unit unit) => new OneOf<T1, T2, T3>();
    }

    /// <summary>
    /// Type that represents one of the 5 possible values (T1, T2, T3, T4 or Empty).
    /// </summary>
    public class OneOf<T1, T2, T3, T4> : OneOf
    {
        private OneOf()
            : base(default, 0)
        {
        }

        public OneOf(T1 t1)
            : base(t1, 1)
        {
        }

        public OneOf(T2 t2)
            : base(t2, 2)
        {
        }

        public OneOf(T3 t3)
            : base(t3, 3)
        {
        }

        public OneOf(T4 t4)
            : base(t4, 4)
        {
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst((T1)Value);
                case 2: return ifSecond((T2)Value);
                case 3: return ifThird((T3)Value);
                case 4: return ifFourth((T4)Value);
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
                case 1: return ifFirst((T1)Value);
                case 2: return ifSecond((T2)Value);
                case 3: return ifThird((T3)Value);
                case 4: return ifFourth((T4)Value);
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
                    ifFirst?.Invoke((T1)Value);
                    break;
                case 2:
                    ifSecond?.Invoke((T2)Value);
                    break;
                case 3:
                    ifThird?.Invoke((T3)Value);
                    break;
                case 4:
                    ifFourth?.Invoke((T4)Value);
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

        public static implicit operator OneOf<T1, T2, T3, T4>(Unit unit) => new OneOf<T1, T2, T3, T4>();
    }

    /// <summary>
    /// Type that represents one of the 6 possible values (T1, T2, T3, T4, T5 or Empty).
    /// </summary>
    public class OneOf<T1, T2, T3, T4, T5> : OneOf
    {
        private OneOf()
            : base(default, 0)
        {
        }

        public OneOf(T1 t1)
            : base(t1, 1)
        {
        }

        public OneOf(T2 t2)
            : base(t2, 2)
        {
        }

        public OneOf(T3 t3)
            : base(t3, 3)
        {
        }

        public OneOf(T4 t4)
            : base(t4, 4)
        {
        }

        public OneOf(T5 t5)
            : base(t5, 5)
        {
        }

        /// <summary>
        /// Maps available item to the result of the corresponding selector.
        /// </summary>
        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<T3, R> ifThird, Func<T4, R> ifFourth, Func<T5, R> ifFifth)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst((T1)Value);
                case 2: return ifSecond((T2)Value);
                case 3: return ifThird((T3)Value);
                case 4: return ifFourth((T4)Value);
                case 5: return ifFifth((T5)Value);
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
                case 1: return ifFirst((T1)Value);
                case 2: return ifSecond((T2)Value);
                case 3: return ifThird((T3)Value);
                case 4: return ifFourth((T4)Value);
                case 5: return ifFifth((T5)Value);
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
                    ifFirst?.Invoke((T1)Value);
                    break;
                case 2:
                    ifSecond?.Invoke((T2)Value);
                    break;
                case 3:
                    ifThird?.Invoke((T3)Value);
                    break;
                case 4:
                    ifFourth?.Invoke((T4)Value);
                    break;
                case 5:
                    ifFifth?.Invoke((T5)Value);
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

        public static implicit operator OneOf<T1, T2, T3, T4, T5>(Unit unit) => new OneOf<T1, T2, T3, T4, T5>();
    }
}
