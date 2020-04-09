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

        #region Record
        public static Record<R1> Select<T1, R1>(this Record<T1> record, Func<T1, R1> selector) => record.Map(selector);

        public static Record<R1> SelectMany<T1, R1>(this Record<T1> record, Func<T1, Record<R1>> selector) => record.FlatMap(selector);

        public static Record<R1> SelectMany<T1, C1, R1>(this Record<T1> record, Func<T1, Record<C1>> selector, Func<T1, C1, R1> resultSelector) => record.FlatMap(t1 => selector(t1).Map(c1 => resultSelector(t1, c1)));

        public static Record<R1> SelectMany<T1, C1, R1>(this Record<T1> record, Func<T1, Record<C1>> selector, Func<T1, C1, Record<R1>> resultSelector) => record.FlatMap(t1 => selector(t1).FlatMap(c1 => resultSelector(t1, c1)));

        public static Record<R1, R2> Select<T1, T2, R1, R2>(this Record<T1, T2> record, Func<T1, T2, (R1, R2)> selector) => record.Map(selector);

        public static Record<R1, R2> SelectMany<T1, T2, R1, R2>(this Record<T1, T2> record, Func<T1, T2, Record<R1, R2>> selector) => record.FlatMap(selector);

        public static Record<R1, R2> SelectMany<T1, T2, C1, C2, R1, R2>(this Record<T1, T2> record, Func<T1, T2, Record<C1, C2>> selector, Func<T1, T2, C1, C2, (R1, R2)> resultSelector) => record.FlatMap((t1, t2) => selector(t1, t2).Map((c1, c2) => resultSelector(t1, t2, c1, c2)));

        public static Record<R1, R2> SelectMany<T1, T2, C1, C2, R1, R2>(this Record<T1, T2> record, Func<T1, T2, Record<C1, C2>> selector, Func<T1, T2, C1, C2, Record<R1, R2>> resultSelector) => record.FlatMap((t1, t2) => selector(t1, t2).FlatMap((c1, c2) => resultSelector(t1, t2, c1, c2)));

        public static Record<R1, R2, R3> Select<T1, T2, T3, R1, R2, R3>(this Record<T1, T2, T3> record, Func<T1, T2, T3, (R1, R2, R3)> selector) => record.Map(selector);

        public static Record<R1, R2, R3> SelectMany<T1, T2, T3, R1, R2, R3>(this Record<T1, T2, T3> record, Func<T1, T2, T3, Record<R1, R2, R3>> selector) => record.FlatMap(selector);

        public static Record<R1, R2, R3> SelectMany<T1, T2, T3, C1, C2, C3, R1, R2, R3>(this Record<T1, T2, T3> record, Func<T1, T2, T3, Record<C1, C2, C3>> selector, Func<T1, T2, T3, C1, C2, C3, (R1, R2, R3)> resultSelector) => record.FlatMap((t1, t2, t3) => selector(t1, t2, t3).Map((c1, c2, c3) => resultSelector(t1, t2, t3, c1, c2, c3)));

        public static Record<R1, R2, R3> SelectMany<T1, T2, T3, C1, C2, C3, R1, R2, R3>(this Record<T1, T2, T3> record, Func<T1, T2, T3, Record<C1, C2, C3>> selector, Func<T1, T2, T3, C1, C2, C3, Record<R1, R2, R3>> resultSelector) => record.FlatMap((t1, t2, t3) => selector(t1, t2, t3).FlatMap((c1, c2, c3) => resultSelector(t1, t2, t3, c1, c2, c3)));

        public static Record<R1, R2, R3, R4> Select<T1, T2, T3, T4, R1, R2, R3, R4>(this Record<T1, T2, T3, T4> record, Func<T1, T2, T3, T4, (R1, R2, R3, R4)> selector) => record.Map(selector);

        public static Record<R1, R2, R3, R4> SelectMany<T1, T2, T3, T4, R1, R2, R3, R4>(this Record<T1, T2, T3, T4> record, Func<T1, T2, T3, T4, Record<R1, R2, R3, R4>> selector) => record.FlatMap(selector);

        public static Record<R1, R2, R3, R4> SelectMany<T1, T2, T3, T4, C1, C2, C3, C4, R1, R2, R3, R4>(this Record<T1, T2, T3, T4> record, Func<T1, T2, T3, T4, Record<C1, C2, C3, C4>> selector, Func<T1, T2, T3, T4, C1, C2, C3, C4, (R1, R2, R3, R4)> resultSelector) => record.FlatMap((t1, t2, t3, t4) => selector(t1, t2, t3, t4).Map((c1, c2, c3, c4) => resultSelector(t1, t2, t3, t4, c1, c2, c3, c4)));

        public static Record<R1, R2, R3, R4> SelectMany<T1, T2, T3, T4, C1, C2, C3, C4, R1, R2, R3, R4>(this Record<T1, T2, T3, T4> record, Func<T1, T2, T3, T4, Record<C1, C2, C3, C4>> selector, Func<T1, T2, T3, T4, C1, C2, C3, C4, Record<R1, R2, R3, R4>> resultSelector) => record.FlatMap((t1, t2, t3, t4) => selector(t1, t2, t3, t4).FlatMap((c1, c2, c3, c4) => resultSelector(t1, t2, t3, t4, c1, c2, c3, c4)));

        public static Record<R1, R2, R3, R4, R5> Select<T1, T2, T3, T4, T5, R1, R2, R3, R4, R5>(this Record<T1, T2, T3, T4, T5> record, Func<T1, T2, T3, T4, T5, (R1, R2, R3, R4, R5)> selector) => record.Map(selector);

        public static Record<R1, R2, R3, R4, R5> SelectMany<T1, T2, T3, T4, T5, R1, R2, R3, R4, R5>(this Record<T1, T2, T3, T4, T5> record, Func<T1, T2, T3, T4, T5, Record<R1, R2, R3, R4, R5>> selector) => record.FlatMap(selector);

        public static Record<R1, R2, R3, R4, R5> SelectMany<T1, T2, T3, T4, T5, C1, C2, C3, C4, C5, R1, R2, R3, R4, R5>(this Record<T1, T2, T3, T4, T5> record, Func<T1, T2, T3, T4, T5, Record<C1, C2, C3, C4, C5>> selector, Func<T1, T2, T3, T4, T5, C1, C2, C3, C4, C5, (R1, R2, R3, R4, R5)> resultSelector) => record.FlatMap((t1, t2, t3, t4, t5) => selector(t1, t2, t3, t4, t5).Map((c1, c2, c3, c4, c5) => resultSelector(t1, t2, t3, t4, t5, c1, c2, c3, c4, c5)));

        public static Record<R1, R2, R3, R4, R5> SelectMany<T1, T2, T3, T4, T5, C1, C2, C3, C4, C5, R1, R2, R3, R4, R5>(this Record<T1, T2, T3, T4, T5> record, Func<T1, T2, T3, T4, T5, Record<C1, C2, C3, C4, C5>> selector, Func<T1, T2, T3, T4, T5, C1, C2, C3, C4, C5, Record<R1, R2, R3, R4, R5>> resultSelector) => record.FlatMap((t1, t2, t3, t4, t5) => selector(t1, t2, t3, t4, t5).FlatMap((c1, c2, c3, c4, c5) => resultSelector(t1, t2, t3, t4, t5, c1, c2, c3, c4, c5)));
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
