using System;

namespace Funk
{
    public static class PartialExt
    {
        public static R Apply<T1, R>(this Func<T1, R> function, T1 t1) => function(t1);
        
        public static Unit Apply<T1>(this Action<T1> function, T1 t1)
        {
            function(t1);
            return Unit.Value;
        }

        public static Func<T2, R> Apply<T1, T2, R>(this Func<T1, T2, R> function, T1 t1) => t2 => function(t1, t2);
        
        public static Action<T2> Apply<T1, T2>(this Action<T1, T2> function, T1 t1) => t2 => function(t1, t2);

        public static Func<T2, T3, R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> function, T1 t1) => (t2, t3) => function(t1, t2, t3);
        
        public static Action<T2, T3> Apply<T1, T2, T3>(this Action<T1, T2, T3> function, T1 t1) => (t2, t3) => function(t1, t2, t3);

        public static Func<T2, T3, T4, R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> function, T1 t1) => (t2, t3, t4) => function(t1, t2, t3, t4);
        
        public static Action<T2, T3, T4> Apply<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> function, T1 t1) => (t2, t3, t4) => function(t1, t2, t3, t4);

        public static Func<T2, T3, T4, T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> function, T1 t1) => (t2, t3, t4, t5) => function(t1, t2, t3, t4, t5);
        
        public static Action<T2, T3, T4, T5> Apply<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> function, T1 t1) => (t2, t3, t4, t5) => function(t1, t2, t3, t4, t5);
    }
}
