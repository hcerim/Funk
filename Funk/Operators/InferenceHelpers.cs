using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Funk
{
    public static partial class Operators
    {
        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Func<R> func<R>(Func<R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Func<T1, R> func<T1, R>(Func<T1, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Func<T1, T2, R> func<T1, T2, R>(Func<T1, T2, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Func<T1, T2, T3, R> func<T1, T2, T3, R>(Func<T1, T2, T3, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Func<T1, T2, T3, T4, R> func<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, R> func<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Action act(Action act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Action<T1> act<T1>(Action<T1> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Action<T1, T2> act<T1, T2>(Action<T1, T2> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Action<T1, T2, T3> act<T1, T2, T3>(Action<T1, T2, T3> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4>(Action<T1, T2, T3, T4> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Func<R>> expr<R>(Expression<Func<R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Func<T1, R>> expr<T1, R>(Expression<Func<T1, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Func<T1, T2, R>> expr<T1, T2, R>(Expression<Func<T1, T2, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Func<T1, T2, T3, R>> expr<T1, T2, T3, R>(Expression<Func<T1, T2, T3, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Func<T1, T2, T3, T4, R>> expr<T1, T2, T3, T4, R>(Expression<Func<T1, T2, T3, T4, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Func<T1, T2, T3, T4, T5, R>> expr<T1, T2, T3, T4, T5, R>(Expression<Func<T1, T2, T3, T4, T5, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Action> expr(Expression<Action> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Action<T1>> expr<T1>(Expression<Action<T1>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Action<T1, T2>> expr<T1, T2>(Expression<Action<T1, T2>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Action<T1, T2, T3>> expr<T1, T2, T3>(Expression<Action<T1, T2, T3>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Action<T1, T2, T3, T4>> expr<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Expression<Action<T1, T2, T3, T4, T5>> expr<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> expr) => expr;
    }
}
