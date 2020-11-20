using System;

namespace Funk
{
    public static class CompositionExt
    {
        /// <summary>
        /// Composes 2 functions from right to left.
        /// </summary>
        public static Func<T1, T3> ComposeRight<T1, T2, T3>(this Func<T2, T3> f1, Func<T1, T2> f2) => i => f1(f2(i));
        
        /// <summary>
        /// Composes 2 functions from left to right.
        /// </summary>
        public static Func<T1, T3> ComposeLeft<T1, T2, T3>(this Func<T1, T2> f1, Func<T2, T3> f2) => i => f2(f1(i));
    }
}