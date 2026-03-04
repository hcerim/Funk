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
        /// <typeparam name="R">The type of the projected value.</typeparam>
        /// <param name="selector">The projection function.</param>
        /// <returns>A Maybe containing the projected value, or an empty Maybe.</returns>
        public Maybe<R> Select<R>(Func<T, R> selector) => maybe.Map(selector);

        /// <summary>
        /// Projects the value of a Maybe into a new Maybe and flattens the result.
        /// </summary>
        /// <typeparam name="R">The type of the projected value.</typeparam>
        /// <param name="selector">The projection function returning a Maybe.</param>
        /// <returns>A Maybe containing the projected value, or an empty Maybe.</returns>
        public Maybe<R> SelectMany<R>(Func<T, Maybe<R>> selector) => maybe.FlatMap(selector);

        /// <summary>
        /// Projects the value of a Maybe into an intermediate Maybe, then applies a result selector.
        /// </summary>
        /// <typeparam name="C">The type of the intermediate value.</typeparam>
        /// <typeparam name="R">The type of the result value.</typeparam>
        /// <param name="selector">The projection function returning an intermediate Maybe.</param>
        /// <param name="resultSelector">The function combining the source and intermediate values.</param>
        /// <returns>A Maybe containing the combined result, or an empty Maybe.</returns>
        public Maybe<R> SelectMany<C, R>(Func<T, Maybe<C>> selector, Func<T, C, R> resultSelector) => maybe.FlatMap(t => selector(t).Map(c => resultSelector(t, c)));

        /// <summary>
        /// Projects the value of a Maybe into an intermediate Maybe, then applies a result selector that returns a Maybe and flattens the result.
        /// </summary>
        /// <typeparam name="C">The type of the intermediate value.</typeparam>
        /// <typeparam name="R">The type of the result value.</typeparam>
        /// <param name="selector">The projection function returning an intermediate Maybe.</param>
        /// <param name="resultSelector">The function combining the source and intermediate values into a Maybe.</param>
        /// <returns>A Maybe containing the combined result, or an empty Maybe.</returns>
        public Maybe<R> SelectMany<C, R>(Func<T, Maybe<C>> selector, Func<T, C, Maybe<R>> resultSelector) => maybe.FlatMap(t => selector(t).FlatMap(c => resultSelector(t, c)));

        /// <summary>
        /// Filters the Maybe based on a predicate. Returns an empty Maybe if the predicate is not satisfied or the value is empty.
        /// </summary>
        /// <param name="predicate">The predicate to test the value against.</param>
        /// <returns>The original Maybe if the predicate is satisfied, or an empty Maybe.</returns>
        public Maybe<T> Where(Func<T, bool> predicate) => maybe.FlatMap(v => predicate(v).Match(f => Maybe.Empty<T>(), t => maybe));
    }

    #endregion

    #region Exc
    extension<T, E>(Exc<T, E> exceptional) where E : Exception
    {
        /// <summary>
        /// Projects the success value of an Exc into a new form.
        /// </summary>
        /// <typeparam name="R">The type of the projected value.</typeparam>
        /// <param name="selector">The projection function.</param>
        /// <returns>An Exc containing the projected value, or the original failure.</returns>
        public Exc<R, E> Select<R>(Func<T, R> selector) => exceptional.Map(selector);

        /// <summary>
        /// Projects the success value of an Exc into a new Exc and flattens the result.
        /// </summary>
        /// <typeparam name="R">The type of the projected value.</typeparam>
        /// <param name="selector">The projection function returning an Exc.</param>
        /// <returns>An Exc containing the projected value, or the original failure.</returns>
        public Exc<R, E> SelectMany<R>(Func<T, Exc<R, E>> selector) => exceptional.FlatMap(selector);

        /// <summary>
        /// Projects the success value of an Exc into an intermediate Exc, then applies a result selector.
        /// </summary>
        /// <typeparam name="C">The type of the intermediate value.</typeparam>
        /// <typeparam name="R">The type of the result value.</typeparam>
        /// <param name="selector">The projection function returning an intermediate Exc.</param>
        /// <param name="resultSelector">The function combining the source and intermediate values.</param>
        /// <returns>An Exc containing the combined result, or the original failure.</returns>
        public Exc<R, E> SelectMany<C, R>(Func<T, Exc<C, E>> selector, Func<T, C, R> resultSelector) => exceptional.FlatMap(t => selector(t).Map(c => resultSelector(t, c)));

        /// <summary>
        /// Projects the success value of an Exc into an intermediate Exc, then applies a result selector that returns an Exc and flattens the result.
        /// </summary>
        /// <typeparam name="C">The type of the intermediate value.</typeparam>
        /// <typeparam name="R">The type of the result value.</typeparam>
        /// <param name="selector">The projection function returning an intermediate Exc.</param>
        /// <param name="resultSelector">The function combining the source and intermediate values into an Exc.</param>
        /// <returns>An Exc containing the combined result, or the original failure.</returns>
        public Exc<R, E> SelectMany<C, R>(Func<T, Exc<C, E>> selector, Func<T, C, Exc<R, E>> resultSelector) => exceptional.FlatMap(t => selector(t).FlatMap(c => resultSelector(t, c)));

        /// <summary>
        /// Filters the Exc based on a predicate on the success value. Returns an empty Exc if the predicate is not satisfied or the value is not successful.
        /// </summary>
        /// <param name="successPredicate">The predicate to test the success value against.</param>
        /// <returns>The original Exc if the predicate is satisfied, or an empty Exc.</returns>
        public Exc<T, E> Where(Func<T, bool> successPredicate) => exceptional.FlatMap(v => successPredicate(v).Match(f => Exc.Empty<T, E>(), t => exceptional));
    }

    #endregion
}