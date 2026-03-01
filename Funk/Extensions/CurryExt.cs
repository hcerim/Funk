using System;

namespace Funk
{
    /// <summary>
    /// Provides currying extension methods for Func delegates.
    /// </summary>
    public static class CurryExt
    {
        /// <summary>
        /// Curries a function with 2 parameters into a chain of single-parameter functions.
        /// </summary>
        public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> function) => t1 => t2 => function(t1, t2);

        /// <summary>
        /// Curries a function with 3 parameters into a chain of single-parameter functions.
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(this Func<T1, T2, T3, R> function) => t1 => t2 => t3 => function(t1, t2, t3);

        /// <summary>
        /// Curries a function with 4 parameters into a chain of single-parameter functions.
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, Func<T4, R>>>> Curry<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> function) => t1 => t2 => t3 => t4 => function(t1, t2, t3, t4);

        /// <summary>
        /// Curries a function with 5 parameters into a chain of single-parameter functions.
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, R>>>>> Curry<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> function) => t1 => t2 => t3 => t4 => t5 => function(t1, t2, t3, t4, t5);
    }
}
