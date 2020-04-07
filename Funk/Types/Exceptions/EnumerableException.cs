using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Funk.Exceptions
{
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
    public sealed class EnumerableException<E> : FunkException, IImmutableList<E> where E : Exception
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

        private IImmutableList<E> nested { get; }

        /// <summary>
        /// Root error cause.
        /// If you pass a collection of exceptions, it will be the first one.
        /// </summary>
        public Maybe<E> Root { get; }

        /// <summary>
        /// Structure-preserving map.
        /// Use Bind if you are binding with another EnumerableException.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exception.
        /// </summary>
        public EnumerableException<E> MapWith(Func<Unit, E> selector)
        {
            return MapWithMany(_ => selector(Unit.Value).ToImmutableList());
        }

        /// <summary>
        /// Structure-preserving map.
        /// Use BindRange if you are binding with other EnumerableExceptions.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exceptions.
        /// </summary>
        public EnumerableException<E> MapWithMany(Func<Unit, IEnumerable<E>> selector)
        {
            var exceptions = selector(Unit.Value).Map();
            return EnumerableException.Create(Nested.Match(
                _ => exceptions,
                c => c.SafeConcat(exceptions)
            ), Message);
        }

        /// <summary>
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exception and its nested ones.
        /// </summary>
        public EnumerableException<E> Bind(EnumerableException<E> exception)
        {
            return BindRange(exception.ToImmutableList());
        }

        /// <summary>
        /// Structure-preserving map.
        /// Maps EnumerableException to the new one with aggregated nested exceptions with new exceptions and their nested ones.
        /// </summary>
        public EnumerableException<E> BindRange(IEnumerable<EnumerableException<E>> exceptions)
        {
            var list = new List<E>();
            list.AddRange(Nested.GetOr(_ => ImmutableList<E>.Empty.Map()));
            list.AddRange(exceptions.FlatMap(e => e.Nested.GetOr(_ => ImmutableList<E>.Empty.Map())));
            return EnumerableException.Create(list, Message);
        }

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

        public override string ToString() => Nested.FlatMap(n => n.MapReduce(e => e.ToString(), (a, b) => $"{a}, {b}")).GetOr(_ => "EnumerableException is empty.");

        #region Obsolete methods
        /// <summary>
        /// Ignores null value.
        /// </summary>
        [Obsolete("Use MapWith or Bind instead.")]
        public IImmutableList<E> Add(E value)
        {
            return MapWithMany(_ => value.ToImmutableList());
        }

        /// <summary>
        /// Handled null enumerable and ignores null values.
        /// </summary>
        [Obsolete("Use MapWithMany or BindRange instead.")]
        public IImmutableList<E> AddRange(IEnumerable<E> items)
        {
            return MapWithMany(_ => items);
        }

        /// <summary>
        /// Creates empty EnumerableException.
        /// </summary>
        [Obsolete("Resetting can cause unexpected issues.")]
        public IImmutableList<E> Clear()
        {
            return new EnumerableException<E>(null);
        }

        [Obsolete("Changing the order of exceptions can cause unexpected issues. Use MapWith or Bind instead.")]
        public IImmutableList<E> Insert(int index, E element)
        {
            return EnumerableException.Create(nested.Insert(index, element), Message);
        }

        [Obsolete("Changing the order of exceptions can cause unexpected issues. Use MapWithMany or BindRange instead.")]
        public IImmutableList<E> InsertRange(int index, IEnumerable<E> items)
        {
            return EnumerableException.Create(nested.InsertRange(index, items), Message);
        }

        [Obsolete("Removing exceptions can cause unexpected issues.")]
        public IImmutableList<E> Remove(E value, IEqualityComparer<E> equalityComparer)
        {
            return EnumerableException.Create(nested.Remove(value, equalityComparer), Message);
        }

        [Obsolete("Removing exceptions can cause unexpected issues.")]
        public IImmutableList<E> RemoveAll(Predicate<E> match)
        {
            return EnumerableException.Create(nested.RemoveAll(match), Message);
        }

        [Obsolete("Removing exceptions can cause unexpected issues.")]
        public IImmutableList<E> RemoveAt(int index)
        {
            return EnumerableException.Create(nested.RemoveAt(index), Message);
        }

        [Obsolete("Removing exceptions can cause unexpected issues.")]
        public IImmutableList<E> RemoveRange(IEnumerable<E> items, IEqualityComparer<E> equalityComparer)
        {
            return EnumerableException.Create(nested.RemoveRange(items, equalityComparer), Message);
        }

        [Obsolete("Removing exceptions can cause unexpected issues.")]
        public IImmutableList<E> RemoveRange(int index, int count)
        {
            return EnumerableException.Create(nested.RemoveRange(index, count), Message);
        }

        [Obsolete("Modifying exceptions can cause unexpected issues.")]
        public IImmutableList<E> Replace(E oldValue, E newValue, IEqualityComparer<E> equalityComparer)
        {
            return EnumerableException.Create(nested.Replace(oldValue, newValue, equalityComparer), Message);
        }

        [Obsolete("Modifying exceptions can cause unexpected issues.")]
        public IImmutableList<E> SetItem(int index, E value)
        {
            return EnumerableException.Create(nested.SetItem(index, value), Message);
        }
        #endregion
    }
}
