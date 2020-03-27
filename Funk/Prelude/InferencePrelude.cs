using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Funk
{
    public static partial class Prelude
    {
        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Func<R> fun<R>(Func<R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Func<T1, R> fun<T1, R>(Func<T1, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Func<T1, T2, R> fun<T1, T2, R>(Func<T1, T2, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, R> fun<T1, T2, T3, R>(Func<T1, T2, T3, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, R> fun<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Func<T1, T2, T3, T4, T5, R> fun<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func) => func;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Action act(Action act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Action<T1> act<T1>(Action<T1> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Action<T1, T2> act<T1, T2>(Action<T1, T2> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Action<T1, T2, T3> act<T1, T2, T3>(Action<T1, T2, T3> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4>(Action<T1, T2, T3, T4> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> act) => act;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Func<R>> exp<R>(Expression<Func<R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Func<T1, R>> exp<T1, R>(Expression<Func<T1, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Func<T1, T2, R>> exp<T1, T2, R>(Expression<Func<T1, T2, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Func<T1, T2, T3, R>> exp<T1, T2, T3, R>(Expression<Func<T1, T2, T3, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Func<T1, T2, T3, T4, R>> exp<T1, T2, T3, T4, R>(Expression<Func<T1, T2, T3, T4, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Func without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Func<T1, T2, T3, T4, T5, R>> exp<T1, T2, T3, T4, T5, R>(Expression<Func<T1, T2, T3, T4, T5, R>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Action> exp(Expression<Action> expr) => expr;
        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Action<T1>> exp<T1>(Expression<Action<T1>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Action<T1, T2>> exp<T1, T2>(Expression<Action<T1, T2>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Action<T1, T2, T3>> exp<T1, T2, T3>(Expression<Action<T1, T2, T3>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Action<T1, T2, T3, T4>> exp<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> expr) => expr;

        /// <summary>
        /// Higher-order function that helps infer Expression with Action without explicitly using the type.
        /// </summary>
        [Pure]
        public static Expression<Action<T1, T2, T3, T4, T5>> exp<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> expr) => expr;
    }
}
