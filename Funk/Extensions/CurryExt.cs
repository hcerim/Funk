using System;

namespace Funk
{
    public static class CurryExt
    {
        public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> function) => t1 => t2 => function(t1, t2);

        public static Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(this Func<T1, T2, T3, R> function) => t1 => t2 => t3 => function(t1, t2, t3);

        public static Func<T1, Func<T2, Func<T3, Func<T4, R>>>> Curry<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> function) => t1 => t2 => t3 => t4 => function(t1, t2, t3, t4);

        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, R>>>>> Curry<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> function) => t1 => t2 => t3 => t4 => t5 => function(t1, t2, t3, t4, t5);
    }
}
