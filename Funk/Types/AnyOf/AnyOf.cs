using System;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    public abstract class AnyOf
    {
        protected AnyOf(object item, int discriminator)
        {
            if (item is null)
            {
                IsEmpty = true;
                NotEmpty = false;
                Discriminator = 0;
            }
            else
            {
                IsEmpty = false;
                NotEmpty = true;
                Discriminator = discriminator;
            }
        }

        protected static Exception GetException(string itemName, Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwiseThrow is null ? new EmptyValueException($"{itemName} item is empty.") : otherwiseThrow(Unit.Value);
        }

        public bool IsEmpty { get; }
        public bool NotEmpty { get; }
        protected int Discriminator { get; }
    }

    public class AnyOf<T1> : AnyOf
    {
        public AnyOf(T1 t1)
            : base(t1, 1)
        {
            _first = t1;
        }

        private readonly T1 _first;

        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T1, R> ifFirst)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                default: return ifEmpty(Unit.Value);
            }
        }

        public R Match<R>(Func<T1, R> ifFirst, Func<Unit, Exception> otherwiseThrow = null)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                default: throw GetException("Every", otherwiseThrow);
            }
        }

        public void Match(Action<Unit> ifEmpty, Action<T1> ifFirst)
        {
            switch (Discriminator)
            {
                case 1: ifFirst(_first);
                    break;
                default: ifEmpty(Unit.Value);
                    break;
            }
        }

        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_first is null | Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return _first;
        }
    }

    public class AnyOf<T1, T2> : AnyOf
    {
        public AnyOf(T1 t1)
            : base(t1, 1)
        {
            _first = t1;
        }

        public AnyOf(T2 t2)
            : base(t2, 2)
        {
            _second = t2;
        }

        private readonly T1 _first;
        private readonly T2 _second;

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

        public R Match<R>(Func<T1, R> ifFirst, Func<T2, R> ifSecond, Func<Unit, Exception> otherwiseThrow = null)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(_first);
                case 2: return ifSecond(_second);
                default: throw GetException("Every", otherwiseThrow);
            }
        }

        public void Match(Action<Unit> ifEmpty, Action<T1> ifFirst, Action<T2> ifSecond)
        {
            switch (Discriminator)
            {
                case 1:
                    ifFirst(_first);
                    break;
                case 2:
                    ifSecond(_second);
                    break;
                default:
                    ifEmpty(Unit.Value);
                    break;
            }
        }

        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_first is null | Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return _first;
        }

        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_second is null | Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return _second;
        }
    }

    public class AnyOf<T1, T2, T3> : AnyOf
    {
        public AnyOf(T1 t1)
            : base(t1, 1)
        {
            _first = t1;
        }

        public AnyOf(T2 t2)
            : base(t2, 2)
        {
            _second = t2;
        }

        public AnyOf(T3 t3)
            : base(t3, 3)
        {
            _third = t3;
        }

        private readonly T1 _first;
        private readonly T2 _second;
        private readonly T3 _third;

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

        public void Match(Action<Unit> ifEmpty, Action<T1> ifFirst, Action<T2> ifSecond, Action<T3> ifThird)
        {
            switch (Discriminator)
            {
                case 1:
                    ifFirst(_first);
                    break;
                case 2:
                    ifSecond(_second);
                    break;
                case 3:
                    ifThird(_third);
                    break;
                default:
                    ifEmpty(Unit.Value);
                    break;
            }
        }

        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_first is null | Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return _first;
        }

        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_second is null | Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return _second;
        }

        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_third is null | Discriminator.SafeNotEquals(3))
            {
                throw GetException("Third", otherwiseThrow);
            }

            return _third;
        }
    }

    public class AnyOf<T1, T2, T3, T4> : AnyOf
    {
        public AnyOf(T1 t1)
            : base(t1, 1)
        {
            _first = t1;
        }

        public AnyOf(T2 t2)
            : base(t2, 2)
        {
            _second = t2;
        }

        public AnyOf(T3 t3)
            : base(t3, 3)
        {
            _third = t3;
        }

        public AnyOf(T4 t4)
            : base(t4, 4)
        {
            _fourth = t4;
        }

        private readonly T1 _first;
        private readonly T2 _second;
        private readonly T3 _third;
        private readonly T4 _fourth;

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

        public void Match(Action<Unit> ifEmpty, Action<T1> ifFirst, Action<T2> ifSecond, Action<T3> ifThird, Action<T4> ifFourth)
        {
            switch (Discriminator)
            {
                case 1:
                    ifFirst(_first);
                    break;
                case 2:
                    ifSecond(_second);
                    break;
                case 3:
                    ifThird(_third);
                    break;
                case 4:
                    ifFourth(_fourth);
                    break;
                default:
                    ifEmpty(Unit.Value);
                    break;
            }
        }

        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_first is null | Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return _first;
        }

        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_second is null | Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return _second;
        }

        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_third is null | Discriminator.SafeNotEquals(3))
            {
                throw GetException("Third", otherwiseThrow);
            }

            return _third;
        }

        public T4 UnsafeGetFourth(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_fourth is null | Discriminator.SafeNotEquals(4))
            {
                throw GetException("Fourth", otherwiseThrow);
            }

            return _fourth;
        }
    }

    public class AnyOf<T1, T2, T3, T4, T5> : AnyOf
    {
        public AnyOf(T1 t1)
            : base(t1, 1)
        {
            _first = t1;
        }

        public AnyOf(T2 t2)
            : base(t2, 2)
        {
            _second = t2;
        }

        public AnyOf(T3 t3)
            : base(t3, 3)
        {
            _third = t3;
        }

        public AnyOf(T4 t4)
            : base(t4, 4)
        {
            _fourth = t4;
        }

        public AnyOf(T5 t5)
            : base(t5, 5)
        {
            _fifth = t5;
        }

        private readonly T1 _first;
        private readonly T2 _second;
        private readonly T3 _third;
        private readonly T4 _fourth;
        private readonly T5 _fifth;

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

        public void Match(Action<Unit> ifEmpty, Action<T1> ifFirst, Action<T2> ifSecond, Action<T3> ifThird, Action<T4> ifFourth, Action<T5> ifFifth)
        {
            switch (Discriminator)
            {
                case 1:
                    ifFirst(_first);
                    break;
                case 2:
                    ifSecond(_second);
                    break;
                case 3:
                    ifThird(_third);
                    break;
                case 4:
                    ifFourth(_fourth);
                    break;
                case 5:
                    ifFifth(_fifth);
                    break;
                default:
                    ifEmpty(Unit.Value);
                    break;
            }
        }

        public T1 UnsafeGetFirst(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_first is null | Discriminator.SafeNotEquals(1))
            {
                throw GetException("First", otherwiseThrow);
            }

            return _first;
        }

        public T2 UnsafeGetSecond(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_second is null | Discriminator.SafeNotEquals(2))
            {
                throw GetException("Second", otherwiseThrow);
            }

            return _second;
        }

        public T3 UnsafeGetThird(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_third is null | Discriminator.SafeNotEquals(3))
            {
                throw GetException("Third", otherwiseThrow);
            }

            return _third;
        }

        public T4 UnsafeGetFourth(Func<Unit, Exception> otherwiseThrow = null)
        {
            if (_fourth is null | Discriminator.SafeNotEquals(4))
            {
                throw GetException("Fourth", otherwiseThrow);
            }

            return _fourth;
        }

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
