using System;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk
{
    /// <summary>
    /// Maybe monad.
    /// Type that represents the possible absence of data with appropriate handling.
    /// </summary>
    public struct Maybe<T>
    {
        public Maybe(T item)
        {
            if (item is null)
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

        private T Value { get; }

        private int Discriminator { get; }
        public bool NotEmpty { get; }

        [Pure]
        public bool IsEmpty => !NotEmpty;

        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T, R> ifFirst)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(Value);
                default: return ifEmpty(Unit.Value);
            }
        }

        public R Match<R>(Func<T, R> ifFirst, Func<Unit, Exception> otherwiseThrow = null)
        {
            switch (Discriminator)
            {
                case 1: return ifFirst(Value);
                default: throw GetException(otherwiseThrow);
            }
        }

        public void Match(Action<Unit> ifEmpty, Action<T> ifFirst)
        {
            switch (Discriminator)
            {
                case 1:
                    ifFirst(Value);
                    break;
                default:
                    ifEmpty(Unit.Value);
                    break;
            }
        }

        [Pure]
        public Maybe<R> Map<R>(Func<T, R> selector) => FlatMap(t => selector(t).AsMaybe());

        [Pure]
        public Maybe<R> FlatMap<R>(Func<T, Maybe<R>> selector) => Match(_ => Maybe.Empty<R>(), selector);

        public T UnsafeGet(Func<Unit, Exception> otherwiseThrow = null)
        {
            switch (Discriminator)
            {
                case 1: return Value;
                default: throw GetException(otherwiseThrow);
            }
        }

        [Pure]
        private static Exception GetException(Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwiseThrow is null ? new EmptyValueException("Maybe value is empty.") : otherwiseThrow(Unit.Value);
        }
    }
}
