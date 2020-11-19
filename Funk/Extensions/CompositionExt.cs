using System;

namespace Funk
{
    public static class CompositionExt
    {
        public static Func<T1, T3> Compose<T1, T2, T3>(this Func<T2, T3> f1, Func<T1, T2> f2) => i => f1(f2(i));
        
        public static Func<T1, T3> ComposeInvert<T1, T2, T3>(this Func<T1, T2> f1, Func<T2, T3> f2) => i => f2(f1(i));
    }
}