using System;

namespace Funk
{
    /// <summary>
    /// Provides LINQ query syntax support for Maybe and Exc types.
    /// </summary>
    public static class LinqExt
    {
        #region Maybe
        /// <summary>
        /// Projects the value of a Maybe into a new form.
        /// </summary>
        public static Maybe<R> Select<T, R>(this Maybe<T> maybe, Func<T, R> selector) => maybe.Map(selector);

        /// <summary>
        /// Projects the value of a Maybe into a new Maybe and flattens the result.
        /// </summary>
        public static Maybe<R> SelectMany<T, R>(this Maybe<T> maybe, Func<T, Maybe<R>> selector) => maybe.FlatMap(selector);

        /// <summary>
        /// Projects the value of a Maybe into an intermediate Maybe, then applies a result selector.
        /// </summary>
        public static Maybe<R> SelectMany<T, C, R>(this Maybe<T> maybe, Func<T, Maybe<C>> selector, Func<T, C, R> resultSelector) => maybe.FlatMap(t => selector(t).Map(c => resultSelector(t, c)));

        /// <summary>
        /// Projects the value of a Maybe into an intermediate Maybe, then applies a result selector that returns a Maybe and flattens the result.
        /// </summary>
        public static Maybe<R> SelectMany<T, C, R>(this Maybe<T> maybe, Func<T, Maybe<C>> selector, Func<T, C, Maybe<R>> resultSelector) => maybe.FlatMap(t => selector(t).FlatMap(c => resultSelector(t, c)));

        /// <summary>
        /// Filters the Maybe based on a predicate. Returns an empty Maybe if the predicate is not satisfied or the value is empty.
        /// </summary>
        public static Maybe<T> Where<T>(this Maybe<T> maybe, Func<T, bool> predicate) => maybe.FlatMap(v => predicate(v).Match(f => Maybe.Empty<T>(), t => maybe));
        #endregion

        #region Exc
        /// <summary>
        /// Projects the success value of an Exc into a new form.
        /// </summary>
        public static Exc<R, E> Select<T, E, R>(this Exc<T, E> exceptional, Func<T, R> selector) where E : Exception => exceptional.Map(selector);

        /// <summary>
        /// Projects the success value of an Exc into a new Exc and flattens the result.
        /// </summary>
        public static Exc<R, E> SelectMany<T, E, R>(this Exc<T, E> exceptional, Func<T, Exc<R, E>> selector) where E : Exception => exceptional.FlatMap(selector);

        /// <summary>
        /// Projects the success value of an Exc into an intermediate Exc, then applies a result selector.
        /// </summary>
        public static Exc<R, E> SelectMany<T, E, C, R>(this Exc<T, E> exceptional, Func<T, Exc<C, E>> selector, Func<T, C, R> resultSelector) where E : Exception => exceptional.FlatMap(t => selector(t).Map(c => resultSelector(t, c)));

        /// <summary>
        /// Projects the success value of an Exc into an intermediate Exc, then applies a result selector that returns an Exc and flattens the result.
        /// </summary>
        public static Exc<R, E> SelectMany<T, E, C, R>(this Exc<T, E> exceptional, Func<T, Exc<C, E>> selector, Func<T, C, Exc<R, E>> resultSelector) where E : Exception => exceptional.FlatMap(t => selector(t).FlatMap(c => resultSelector(t, c)));

        /// <summary>
        /// Filters the Exc based on a predicate on the success value. Returns an empty Exc if the predicate is not satisfied or the value is not successful.
        /// </summary>
        public static Exc<T, E> Where<T, E>(this Exc<T, E> exceptional, Func<T, bool> successPredicate) where E : Exception => exceptional.FlatMap(v => successPredicate(v).Match(f => Exc.Empty<T, E>(), t => exceptional));
        #endregion
    }
}
