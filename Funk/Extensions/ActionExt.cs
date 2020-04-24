using System;

namespace Funk
{
    public static class ActionExt
    {
        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public static Func<Unit> ToFunc(this Action act) => () =>
        {
            act();
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public static Func<R> ToFunc<R>(this Action act, Func<Unit, R> result) => () =>
        {
            act();
            return result(Unit.Value);
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public static Func<T1, Unit> ToFunc<T1>(this Action<T1> act) => t1 =>
        {
            act(t1);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public static Func<T1, R> ToFunc<T1, R>(this Action<T1> act, Func<Unit, R> result) => t1 =>
        {
            act(t1);
            return result(Unit.Value);
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public static Func<T1, T2, Unit> ToFunc<T1, T2>(this Action<T1, T2> act) => (t1, t2) =>
        {
            act(t1, t2);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public static Func<T1, T2, R> ToFunc<T1, T2, R>(this Action<T1, T2> act, Func<Unit, R> result) => (t1, t2) =>
        {
            act(t1, t2);
            return result(Unit.Value);
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public static Func<T1, T2, T3, Unit> ToFunc<T1, T2, T3>(this Action<T1, T2, T3> act) => (t1, t2, t3) =>
        {
            act(t1, t2, t3);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public static Func<T1, T2, T3, R> ToFunc<T1, T2, T3, R>(this Action<T1, T2, T3> act, Func<Unit, R> result) => (t1, t2, t3) =>
        {
            act(t1, t2, t3);
            return result(Unit.Value);
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public static Func<T1, T2, T3, T4, Unit> ToFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> act) => (t1, t2, t3, t4) =>
        {
            act(t1, t2, t3, t4);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public static Func<T1, T2, T3, T4, R> ToFunc<T1, T2, T3, T4, R>(this Action<T1, T2, T3, T4> act, Func<Unit, R> result) => (t1, t2, t3, t4) =>
        {
            act(t1, t2, t3, t4);
            return result(Unit.Value);
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public static Func<T1, T2, T3, T4, T5, Unit> ToFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> act) => (t1, t2, t3, t4, t5) =>
        {
            act(t1, t2, t3, t4, t5);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public static Func<T1, T2, T3, T4, T5, R> ToFunc<T1, T2, T3, T4, T5, R>(this Action<T1, T2, T3, T4, T5> act, Func<Unit, R> result) => (t1, t2, t3, t4, t5) =>
        {
            act(t1, t2, t3, t4, t5);
            return result(Unit.Value);
        };
    }
}
