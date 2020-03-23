﻿using System;
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

        /// <summary>
        /// Maps available Maybe item (value or empty) to the result of the corresponding selector.
        /// </summary>
        [Pure]
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T, R> ifNotEmpty)
        {
            switch (Discriminator)
            {
                case 1: return ifNotEmpty(Value);
                default: return ifEmpty(Unit.Value);
            }
        }

        /// <summary>
        /// Maps not empty value to the result of the selector or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        public R Match<R>(Func<T, R> ifNotEmpty, Func<Unit, Exception> otherwiseThrow = null)
        {
            switch (Discriminator)
            {
                case 1: return ifNotEmpty(Value);
                default: throw GetException(otherwiseThrow);
            }
        }

        /// <summary>
        /// Executes operation provided with available Maybe (value or empty) item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty, Action<T> ifNotEmpty)
        {
            switch (Discriminator)
            {
                case 1:
                    ifNotEmpty(Value);
                    break;
                default:
                    ifEmpty(Unit.Value);
                    break;
            }
        }

        /// <summary>
        /// Maps not empty Maybe to the new Maybe of the selector. Otherwise, returns empty Maybe of the selector.
        /// Use FlatMap if you have nested Maybes.
        /// </summary>
        [Pure]
        public Maybe<R> Map<R>(Func<T, R> selector) => FlatMap(t => selector(t).AsMaybe());

        /// <summary>
        /// Maps not empty Maybe to the new Maybe of the selector. Otherwise, returns empty Maybe of the selector.
        /// </summary>
        [Pure]
        public Maybe<R> FlatMap<R>(Func<T, Maybe<R>> selector) => Match(_ => Maybe.Empty<R>(), selector);

        /// <summary>
        /// Returns not empty value of Maybe or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
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
