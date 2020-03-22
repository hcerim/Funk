using System;
using System.Diagnostics.Contracts;

namespace Funk
{
    public static class ActionExt
    {
        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        [Pure]
        public static Func<Unit> ToFunc(this Action act) => () =>
        {
            act();
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        [Pure]
        public static Func<T1, Unit> ToFunc<T1>(this Action<T1> act) => t1 =>
        {
            act(t1);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        [Pure]
        public static Func<T1, T2, Unit> ToFunc<T1, T2>(this Action<T1, T2> act) => (t1, t2) =>
        {
            act(t1, t2);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, Unit> ToFunc<T1, T2, T3>(this Action<T1, T2, T3> act) => (t1, t2, t3) =>
        {
            act(t1, t2, t3);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, Unit> ToFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> act) => (t1, t2, t3, t4) =>
        {
            act(t1, t2, t3, t4);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, Unit> ToFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> act) => (t1, t2, t3, t4, t5) =>
        {
            act(t1, t2, t3, t4, t5);
            return Unit.Value;
        };
    }
}
