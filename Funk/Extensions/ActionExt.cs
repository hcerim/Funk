using System;

namespace Funk;

/// <summary>
/// Provides extension methods for converting Action delegates to Func delegates.
/// </summary>
public static class ActionExt
{
    extension(Action act)
    {
        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public Func<Unit> ToFunc() => () =>
        {
            act();
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public Func<R> ToFunc<R>(Func<Unit, R> result) => () =>
        {
            act();
            return result(Unit.Value);
        };
    }

    extension<T1>(Action<T1> act)
    {
        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public Func<T1, Unit> ToFunc() => t1 =>
        {
            act(t1);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public Func<T1, R> ToFunc<R>(Func<Unit, R> result) => t1 =>
        {
            act(t1);
            return result(Unit.Value);
        };
    }

    extension<T1, T2>(Action<T1, T2> act)
    {
        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public Func<T1, T2, Unit> ToFunc() => (t1, t2) =>
        {
            act(t1, t2);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public Func<T1, T2, R> ToFunc<R>(Func<Unit, R> result) => (t1, t2) =>
        {
            act(t1, t2);
            return result(Unit.Value);
        };
    }

    extension<T1, T2, T3>(Action<T1, T2, T3> act)
    {
        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public Func<T1, T2, T3, Unit> ToFunc() => (t1, t2, t3) =>
        {
            act(t1, t2, t3);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public Func<T1, T2, T3, R> ToFunc<R>(Func<Unit, R> result) => (t1, t2, t3) =>
        {
            act(t1, t2, t3);
            return result(Unit.Value);
        };
    }

    extension<T1, T2, T3, T4>(Action<T1, T2, T3, T4> act)
    {
        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public Func<T1, T2, T3, T4, Unit> ToFunc() => (t1, t2, t3, t4) =>
        {
            act(t1, t2, t3, t4);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public Func<T1, T2, T3, T4, R> ToFunc<R>(Func<Unit, R> result) => (t1, t2, t3, t4) =>
        {
            act(t1, t2, t3, t4);
            return result(Unit.Value);
        };
    }

    extension<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> act)
    {
        /// <summary>
        /// Higher-order function that converts Action to Func with the return value of Unit.
        /// </summary>
        public Func<T1, T2, T3, T4, T5, Unit> ToFunc() => (t1, t2, t3, t4, t5) =>
        {
            act(t1, t2, t3, t4, t5);
            return Unit.Value;
        };

        /// <summary>
        /// Higher-order function that converts Action to Func with the specified result.
        /// </summary>
        public Func<T1, T2, T3, T4, T5, R> ToFunc<R>(Func<Unit, R> result) => (t1, t2, t3, t4, t5) =>
        {
            act(t1, t2, t3, t4, t5);
            return result(Unit.Value);
        };
    }
}