using System;

namespace Funk
{
    public static class LinqExt
    {
        #region Maybe
        public static Maybe<R> Select<T, R>(this Maybe<T> maybe, Func<T, R> selector) => maybe.Map(selector);

        public static Maybe<R> SelectMany<T, R>(this Maybe<T> maybe, Func<T, Maybe<R>> selector) => maybe.FlatMap(selector);

        public static Maybe<R> SelectMany<T, C, R>(this Maybe<T> maybe, Func<T, Maybe<C>> selector, Func<T, C, R> resultSelector) => maybe.FlatMap(t => selector(t).Map(c => resultSelector(t, c)));

        public static Maybe<R> SelectMany<T, C, R>(this Maybe<T> maybe, Func<T, Maybe<C>> selector, Func<T, C, Maybe<R>> resultSelector) => maybe.FlatMap(t => selector(t).FlatMap(c => resultSelector(t, c)));

        public static Maybe<T> Where<T>(this Maybe<T> maybe, Func<T, bool> predicate) => maybe.FlatMap(v => predicate(v).Match(f => Maybe.Empty<T>(), t => maybe));
        #endregion

        #region Exc
        public static Exc<R, E> Select<T, E, R>(this Exc<T, E> exceptional, Func<T, R> selector) where E : Exception => exceptional.Map(selector);

        public static Exc<R, E> SelectMany<T, E, R>(this Exc<T, E> exceptional, Func<T, Exc<R, E>> selector) where E : Exception => exceptional.FlatMap(selector);

        public static Exc<R, E> SelectMany<T, E, C, R>(this Exc<T, E> exceptional, Func<T, Exc<C, E>> selector, Func<T, C, R> resultSelector) where E : Exception => exceptional.FlatMap(t => selector(t).Map(c => resultSelector(t, c)));

        public static Exc<R, E> SelectMany<T, E, C, R>(this Exc<T, E> exceptional, Func<T, Exc<C, E>> selector, Func<T, C, Exc<R, E>> resultSelector) where E : Exception => exceptional.FlatMap(t => selector(t).FlatMap(c => resultSelector(t, c)));

        public static Exc<T, E> Where<T, E>(this Exc<T, E> exceptional, Func<T, bool> successPredicate) where E : Exception => exceptional.FlatMap(v => successPredicate(v).Match(f => Exc.Empty<T, E>(), t => exceptional));
        #endregion
    }
}
