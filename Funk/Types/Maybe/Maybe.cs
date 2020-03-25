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

        private object Value { get; }
        private int Discriminator { get; }
        public bool NotEmpty { get; }

        [Pure]
        public bool IsEmpty => !NotEmpty;

        /// <summary>
        /// Maps available Maybe item (value or empty) to the result of the corresponding selector.
        /// </summary>
        public R Match<R>(Func<Unit, R> ifEmpty, Func<T, R> ifNotEmpty)
        {
            switch (Discriminator)
            {
                case 1: return ifNotEmpty((T)Value);
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
                case 1: return ifNotEmpty((T)Value);
                default: throw GetException(otherwiseThrow);
            }
        }

        /// <summary>
        /// Executes operation provided with available Maybe (value or empty) item.
        /// </summary>
        public void Match(Action<Unit> ifEmpty = null, Action<T> ifNotEmpty = null)
        {
            switch (Discriminator)
            {
                case 1:
                    ifNotEmpty?.Invoke((T)Value);
                    break;
                default:
                    ifEmpty?.Invoke(Unit.Value);
                    break;
            }
        }

        /// <summary>
        /// Maps not empty Maybe to the new Maybe of the selector. Otherwise, returns empty Maybe of the selector.
        /// Use FlatMap if you have nested Maybes.
        /// </summary>
        public Maybe<R> Map<R>(Func<T, R> selector) => FlatMap(t => selector(t).AsMaybe());

        /// <summary>
        /// Maps not empty Maybe to the new Maybe of the selector. Otherwise, returns empty Maybe of the selector.
        /// </summary>
        public Maybe<R> FlatMap<R>(Func<T, Maybe<R>> selector) => Match(_ => Maybe.Empty, selector);

        /// <summary>
        /// Returns not empty value of Maybe or throws EmptyValueException (unless specified explicitly).
        /// </summary>
        /// <exception cref="EmptyValueException"></exception>
        public T UnsafeGet(Func<Unit, Exception> otherwiseThrow = null)
        {
            return Match(
                _ => throw GetException(otherwiseThrow),
                v => v
            );
        }

        public static implicit operator Maybe<T>(Unit unit) => new Maybe<T>();

        [Pure]
        private static Exception GetException(Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwiseThrow.IsNull() ? new EmptyValueException("Maybe value is empty.") : otherwiseThrow(Unit.Value);
        }
    }
}
