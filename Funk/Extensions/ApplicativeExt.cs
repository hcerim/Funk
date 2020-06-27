using System;

namespace Funk
{
    public static class ApplicativeExt
    {
        public static Maybe<R> Apply<T1, R>(this Maybe<Func<T1, R>> function, Maybe<T1> maybe) => function.FlatMap(maybe.Map);

        public static Maybe<Func<T2, R>> Apply<T1, T2, R>(this Maybe<Func<T1, T2, R>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));

        public static Maybe<Func<T2, T3, R>> Apply<T1, T2, T3, R>(this Maybe<Func<T1, T2, T3, R>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));

        public static Maybe<Func<T2, T3, T4, R>> Apply<T1, T2, T3, T4, R>(this Maybe<Func<T1, T2, T3, T4, R>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));

        public static Maybe<Func<T2, T3, T4, T5, R>> Apply<T1, T2, T3, T4, T5, R>(this Maybe<Func<T1, T2, T3, T4, T5, R>> function, Maybe<T1> maybe) => function.FlatMap(f => maybe.Map(f.Apply));

        public static Exc<R, E> Apply<T1, R, E>(this Exc<Func<T1, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(exceptional.Map);

        public static Exc<Func<T2, R>, E> Apply<T1, T2, R, E>(this Exc<Func<T1, T2, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));

        public static Exc<Func<T2, T3, R>, E> Apply<T1, T2, T3, R, E>(this Exc<Func<T1, T2, T3, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));

        public static Exc<Func<T2, T3, T4, R>, E> Apply<T1, T2, T3, T4, R, E>(this Exc<Func<T1, T2, T3, T4, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));

        public static Exc<Func<T2, T3, T4, T5, R>, E> Apply<T1, T2, T3, T4, T5, R, E>(this Exc<Func<T1, T2, T3, T4, T5, R>, E> function, Exc<T1, E> exceptional) where E : Exception => function.FlatMap(f => exceptional.Map(f.Apply));
    }
}