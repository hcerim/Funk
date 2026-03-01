using System;

namespace Funk;

/// <summary>
/// Provides LINQ query syntax support for Maybe and Exc types.
/// </summary>
public static class LinqExt
{
    #region Maybe
    extension<T>(Maybe<T> maybe)
    {
        /// <summary>
        /// Projects the value of a Maybe into a new form.
        /// </summary>
        public Maybe<R> Select<R>(Func<T, R> selector) => maybe.Map(selector);

        /// <summary>
        /// Projects the value of a Maybe into a new Maybe and flattens the result.
        /// </summary>
        public Maybe<R> SelectMany<R>(Func<T, Maybe<R>> selector) => maybe.FlatMap(selector);

        /// <summary>
        /// Projects the value of a Maybe into an intermediate Maybe, then applies a result selector.
        /// </summary>
        public Maybe<R> SelectMany<C, R>(Func<T, Maybe<C>> selector, Func<T, C, R> resultSelector) => maybe.FlatMap(t => selector(t).Map(c => resultSelector(t, c)));

        /// <summary>
        /// Projects the value of a Maybe into an intermediate Maybe, then applies a result selector that returns a Maybe and flattens the result.
        /// </summary>
        public Maybe<R> SelectMany<C, R>(Func<T, Maybe<C>> selector, Func<T, C, Maybe<R>> resultSelector) => maybe.FlatMap(t => selector(t).FlatMap(c => resultSelector(t, c)));

        /// <summary>
        /// Filters the Maybe based on a predicate. Returns an empty Maybe if the predicate is not satisfied or the value is empty.
        /// </summary>
        public Maybe<T> Where(Func<T, bool> predicate) => maybe.FlatMap(v => predicate(v).Match(f => Maybe.Empty<T>(), t => maybe));
    }

    #endregion

    #region Exc
    extension<T, E>(Exc<T, E> exceptional) where E : Exception
    {
        /// <summary>
        /// Projects the success value of an Exc into a new form.
        /// </summary>
        public Exc<R, E> Select<R>(Func<T, R> selector) => exceptional.Map(selector);

        /// <summary>
        /// Projects the success value of an Exc into a new Exc and flattens the result.
        /// </summary>
        public Exc<R, E> SelectMany<R>(Func<T, Exc<R, E>> selector) => exceptional.FlatMap(selector);

        /// <summary>
        /// Projects the success value of an Exc into an intermediate Exc, then applies a result selector.
        /// </summary>
        public Exc<R, E> SelectMany<C, R>(Func<T, Exc<C, E>> selector, Func<T, C, R> resultSelector) => exceptional.FlatMap(t => selector(t).Map(c => resultSelector(t, c)));

        /// <summary>
        /// Projects the success value of an Exc into an intermediate Exc, then applies a result selector that returns an Exc and flattens the result.
        /// </summary>
        public Exc<R, E> SelectMany<C, R>(Func<T, Exc<C, E>> selector, Func<T, C, Exc<R, E>> resultSelector) => exceptional.FlatMap(t => selector(t).FlatMap(c => resultSelector(t, c)));

        /// <summary>
        /// Filters the Exc based on a predicate on the success value. Returns an empty Exc if the predicate is not satisfied or the value is not successful.
        /// </summary>
        public Exc<T, E> Where(Func<T, bool> successPredicate) => exceptional.FlatMap(v => successPredicate(v).Match(f => Exc.Empty<T, E>(), t => exceptional));
    }

    #endregion
}