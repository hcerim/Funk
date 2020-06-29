using System;

namespace Funk
{
    public static class PartialExt
    {
        public static R Apply<T1, R>(this Func<T1, R> function, T1 t1) => function(t1);

        public static Func<T2, R> Apply<T1, T2, R>(this Func<T1, T2, R> function, T1 t1) => t2 => function(t1, t2);

        public static Func<T2, T3, R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> function, T1 t1) => (t2, t3) => function(t1, t2, t3);

        public static Func<T2, T3, T4, R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> function, T1 t1) => (t2, t3, t4) => function(t1, t2, t3, t4);

        public static Func<T2, T3, T4, T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> function, T1 t1) => (t2, t3, t4, t5) => function(t1, t2, t3, t4, t5);
    }
}
