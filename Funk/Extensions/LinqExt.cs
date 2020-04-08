using System;

namespace Funk
{
    public static class LinqExt
    {
        public static Maybe<R> Select<T, R>(this Maybe<T> maybe, Func<T, R> selector) => maybe.Map(selector);

        public static Maybe<R> SelectMany<T, R>(this Maybe<T> maybe, Func<T, Maybe<R>> selector) => maybe.FlatMap(selector);

        public static Maybe<T> Where<T>(this Maybe<T> maybe, Func<T, bool> predicate) => maybe.FlatMap(v => predicate(v).Match(f => Maybe.Empty<T>(), t => maybe));

        public static Record<R1> Select<T1, R1>(this Record<T1> record, Func<T1, R1> selector) => record.Map(selector);

        public static Record<R1> SelectMany<T1, R1>(this Record<T1> record, Func<T1, Record<R1>> selector) => record.Map(selector);

        public static Record<R1, R2> Select<T1, T2, R1, R2>(this Record<T1, T2> record, Func<T1, T2, (R1, R2)> selector) => record.Map(selector);

        public static Record<R1, R2> SelectMany<T1, T2, R1, R2>(this Record<T1, T2> record, Func<T1, T2, Record<R1, R2>> selector) => record.Map(selector);

        public static Record<R1, R2, R3> Select<T1, T2, T3, R1, R2, R3>(this Record<T1, T2, T3> record, Func<T1, T2, T3, (R1, R2, R3)> selector) => record.Map(selector);

        public static Record<R1, R2, R3> SelectMany<T1, T2, T3, R1, R2, R3>(this Record<T1, T2, T3> record, Func<T1, T2, T3, Record<R1, R2, R3>> selector) => record.Map(selector);

        public static Record<R1, R2, R3, R4> Select<T1, T2, T3, T4, R1, R2, R3, R4>(this Record<T1, T2, T3, T4> record, Func<T1, T2, T3, T4, (R1, R2, R3, R4)> selector) => record.Map(selector);

        public static Record<R1, R2, R3, R4> SelectMany<T1, T2, T3, T4, R1, R2, R3, R4>(this Record<T1, T2, T3, T4> record, Func<T1, T2, T3, T4, Record<R1, R2, R3, R4>> selector) => record.Map(selector);

        public static Record<R1, R2, R3, R4, R5> Select<T1, T2, T3, T4, T5, R1, R2, R3, R4, R5>(this Record<T1, T2, T3, T4, T5> record, Func<T1, T2, T3, T4, T5, (R1, R2, R3, R4, R5)> selector) => record.Map(selector);

        public static Record<R1, R2, R3, R4, R5> SelectMany<T1, T2, T3, T4, T5, R1, R2, R3, R4, R5>(this Record<T1, T2, T3, T4, T5> record, Func<T1, T2, T3, T4, T5, Record<R1, R2, R3, R4, R5>> selector) => record.Map(selector);

        public static Exc<R, E> Select<T, E, R>(this Exc<T, E> exceptional, Func<T, R> selector) where E : Exception => exceptional.Map(selector);

        public static Exc<R, E> SelectMany<T, E, R>(this Exc<T, E> exceptional, Func<T, Exc<R, E>> selector) where E : Exception => exceptional.FlatMap(selector);

        public static Maybe<Exc<T, E>> Where<T, E>(this Exc<T, E> exceptional, Func<T, bool> predicate) where E : Exception => exceptional.AsSuccess().FlatMap(e => predicate(e.UnsafeGetFirst()).Match(f => Maybe.Empty<Exc<T, E>>(), t => Maybe.Create(e)));
    }
}
