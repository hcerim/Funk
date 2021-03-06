﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using static Funk.Prelude;

namespace Funk
{
    public enum FunkExceptionType
    {
        Undefined = 0,
        EmptyValue = 1,
        UnhandledValue = 2,
        Enumerable = 3,
        Serialization = 4,
    }

    /// <summary>
    /// Base Funk exception.
    /// </summary>
    public class FunkException : Exception
    {
        public FunkException(string message)
            : base(message)
        {
            Type = FunkExceptionType.Undefined;
        }

        public FunkException(FunkExceptionType type, string message)
            : base(message)
        {
            Type = type;
        }

        public FunkExceptionType Type { get; }
    }

    /// <summary>
    /// Indicates empty value error upon retrieval.
    /// </summary>
    public sealed class EmptyValueException : FunkException
    {
        public EmptyValueException(string message)
            : base(FunkExceptionType.EmptyValue, message)
        {
        }
    }
    
    /// <summary>
    /// Indicates error upon serialization.
    /// </summary>
    public sealed class SerializationException : FunkException
    {
        public SerializationException(string message)
            : base(FunkExceptionType.Serialization, message)
        {
        }
    }

    /// <summary>
    /// Indicates unhandled value upon pattern matching.
    /// </summary>
    public sealed class UnhandledValueException : FunkException
    {
        public UnhandledValueException(string message)
            : base(FunkExceptionType.UnhandledValue, message)
        {
        }
    }

    public static class EnumerableException
    {
        /// <summary>
        /// Creates a new EnumerableException with one nested exception if not null.
        /// </summary>
        [Pure]
        public static EnumerableException<E> Create<E>(E exception, string message = null) where E : Exception => new EnumerableException<E>(exception, message);

        /// <summary>
        /// Creates a new EnumerableException with nested exceptions if not null or empty (removes null exceptions from enumerable).
        /// </summary>
        [Pure]
        public static EnumerableException<E> Create<E>(IEnumerable<E> exceptions, string message = null) where E : Exception => new EnumerableException<E>(exceptions, message);
    }

    /// <summary>
    /// Immutable exception collection.
    /// </summary>
    public sealed class EnumerableException<E> : FunkException, IImmutableList<E>, IEquatable<EnumerableException<E>> where E : Exception
    {
        private EnumerableException(string message)
            : base(FunkExceptionType.Enumerable, message)
        {
            nested = ImmutableList<E>.Empty;
            Root = Maybe.Empty<E>();
        }

        public EnumerableException(E exception, string message = null)
            : base(FunkExceptionType.Enumerable, message)
        {
            nested = exception.ToImmutableList();
            Root = exception.AsMaybe();
        }

        public EnumerableException(IEnumerable<E> exceptions, string message = null)
            : base(FunkExceptionType.Enumerable, message)
        {
            var list = exceptions.ExceptNulls();
            nested = list;
            Root = list.AsFirstOrDefault(i => i.IsNotNull());
        }

        /// <summary>
        /// Collection of aggregated exceptions including the root one.
        /// </summary>
        public Maybe<IImmutableList<E>> Nested => nested.AsNotEmptyList();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IImmutableList<E> nested;

        /// <summary>
        /// Root error cause.
        /// If you pass a collection of exceptions, it will be the first one.
        /// </summary>
        public Maybe<E> Root { get; }

        /// <summary>
        /// Use Bind if you are binding with another EnumerableException.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exception.
        /// </summary>
        public EnumerableException<E> Add(E exception) => AddRange(exception.ToImmutableList());

        /// <summary>
        /// Use BindRange if you are binding with other EnumerableExceptions.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exceptions.
        /// </summary>
        public EnumerableException<E> AddRange(IEnumerable<E> sequence)
        {
            return EnumerableException.Create(Nested.Match(
                _ => sequence.Map(),
                c => c.SafeConcat(sequence.Map())
            ), Message);
        }

        /// <summary>
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exception and its nested ones.
        /// </summary>
        public EnumerableException<E> Merge(EnumerableException<E> exception) => MergeRange(exception.ToImmutableList());

        /// <summary>
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exceptions and their nested ones.
        /// </summary>
        public EnumerableException<E> MergeRange(IEnumerable<EnumerableException<E>> exceptions) => EnumerableException.Create(list<E>().AddRange(nested).AddRange(exceptions.FlatMap(e => e.nested)), Message);

        /// <summary>
        /// Returns a Maybe of an immutable dictionary of key as a discriminator and collection of corresponding exceptions if there are any nested exception.
        /// Handles duplicate key.
        /// </summary>
        public Maybe<IImmutableDictionary<TKey, IImmutableList<E>>> ToDictionary<TKey>(Func<E, TKey> keySelector) => Nested.Map(c => c.ToDictionary(keySelector));

        public IEnumerator<E> GetEnumerator()
        {
            return nested.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => nested.Count;

        public E this[int index] => nested[index];

        public int IndexOf(E item, int index, int count, IEqualityComparer<E> equalityComparer)
        {
            return nested.IndexOf(item, index, count, equalityComparer);
        }

        public int LastIndexOf(E item, int index, int count, IEqualityComparer<E> equalityComparer)
        {
            return nested.LastIndexOf(item, index, count, equalityComparer);
        }

        public static bool operator ==(EnumerableException<E> exception, EnumerableException<E> other) => exception.AsMaybe().Map(e => e.Equals(other)).GetOrDefault();

        public static bool operator !=(EnumerableException<E> exception, EnumerableException<E> other) => !(exception == other);

        public override int GetHashCode() => nested.GetHashCode();

        public bool Equals(EnumerableException<E> other) => other.AsMaybe().Map(e => nested.SafeEquals(e.nested)).GetOrDefault();

        public override bool Equals(object obj) => obj.SafeCast<EnumerableException<E>>().Map(Equals).GetOrDefault();

        public override string ToString() => Nested.FlatMap(n => n.MapReduce(e => e.ToString(), (a, b) => $"{a}, {b}")).GetOr(_ => "EnumerableException is empty.");

        IImmutableList<E> IImmutableList<E>.Add(E value) => Add(value);

        IImmutableList<E> IImmutableList<E>.AddRange(IEnumerable<E> items) => AddRange(items);

        IImmutableList<E> IImmutableList<E>.Clear() => new EnumerableException<E>(null);

        IImmutableList<E> IImmutableList<E>.Insert(int index, E element) => EnumerableException.Create(nested.Insert(index, element), Message);

        IImmutableList<E> IImmutableList<E>.InsertRange(int index, IEnumerable<E> items) => EnumerableException.Create(nested.InsertRange(index, items), Message);

        IImmutableList<E> IImmutableList<E>.Remove(E value, IEqualityComparer<E> equalityComparer) => EnumerableException.Create(nested.Remove(value, equalityComparer), Message);

        IImmutableList<E> IImmutableList<E>.RemoveAll(Predicate<E> match) => EnumerableException.Create(nested.RemoveAll(match), Message);

        IImmutableList<E> IImmutableList<E>.RemoveAt(int index) => EnumerableException.Create(nested.RemoveAt(index), Message);

        IImmutableList<E> IImmutableList<E>.RemoveRange(IEnumerable<E> items, IEqualityComparer<E> equalityComparer) => EnumerableException.Create(nested.RemoveRange(items, equalityComparer), Message);

        IImmutableList<E> IImmutableList<E>.RemoveRange(int index, int count) => EnumerableException.Create(nested.RemoveRange(index, count), Message);

        IImmutableList<E> IImmutableList<E>.Replace(E oldValue, E newValue, IEqualityComparer<E> equalityComparer) => EnumerableException.Create(nested.Replace(oldValue, newValue, equalityComparer), Message);

        IImmutableList<E> IImmutableList<E>.SetItem(int index, E value) => EnumerableException.Create(nested.SetItem(index, value), Message);
    }
}
