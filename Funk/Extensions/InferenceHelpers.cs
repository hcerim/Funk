using System;
using System.Linq.Expressions;

namespace Funk
{
    public static partial class Operators
    {
        public static Func<R> func<R>(Func<R> func) => func;
        public static Func<T1, R> func<T1, R>(Func<T1, R> func) => func;
        public static Func<T1, T2, R> func<T1, T2, R>(Func<T1, T2, R> func) => func;
        public static Func<T1, T2, T3, R> func<T1, T2, T3, R>(Func<T1, T2, T3, R> func) => func;
        public static Func<T1, T2, T3, T4, R> func<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func) => func;
        public static Func<T1, T2, T3, T4, T5, R> func<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func) => func;
        public static Func<T1, T2, T3, T4, T5, T6, R> func<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func) => func;
        public static Func<T1, T2, T3, T4, T5, T6, T7, R> func<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func) => func;

        public static Action act(Action act) => act;
        public static Action<T1> act<T1>(Action<T1> act) => act;
        public static Action<T1, T2> act<T1, T2>(Action<T1, T2> act) => act;
        public static Action<T1, T2, T3> act<T1, T2, T3>(Action<T1, T2, T3> act) => act;
        public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4>(Action<T1, T2, T3, T4> act) => act;
        public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> act) => act;
        public static Action<T1, T2, T3, T4, T5, T6> act<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> act) => act;
        public static Action<T1, T2, T3, T4, T5, T6, T7> act<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> act) => act;

        public static Expression<Func<R>> expr<R>(Expression<Func<R>> expr) => expr;
        public static Expression<Func<T1, R>> expr<T1, R>(Expression<Func<T1, R>> expr) => expr;
        public static Expression<Func<T1, T2, R>> expr<T1, T2, R>(Expression<Func<T1, T2, R>> expr) => expr;
        public static Expression<Func<T1, T2, T3, R>> expr<T1, T2, T3, R>(Expression<Func<T1, T2, T3, R>> expr) => expr;
        public static Expression<Func<T1, T2, T3, T4, R>> expr<T1, T2, T3, T4, R>(Expression<Func<T1, T2, T3, T4, R>> expr) => expr;
        public static Expression<Func<T1, T2, T3, T4, T5, R>> expr<T1, T2, T3, T4, T5, R>(Expression<Func<T1, T2, T3, T4, T5, R>> expr) => expr;
        public static Expression<Func<T1, T2, T3, T4, T5, T6, R>> expr<T1, T2, T3, T4, T5, T6, R>(Expression<Func<T1, T2, T3, T4, T5, T6, R>> expr) => expr;
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, R>> expr<T1, T2, T3, T4, T5, T6, T7, R>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, R>> expr) => expr;

        public static Expression<Action> expr(Expression<Action> expr) => expr;
        public static Expression<Action<T1>> expr<T1>(Expression<Action<T1>> expr) => expr;
        public static Expression<Action<T1, T2>> expr<T1, T2>(Expression<Action<T1, T2>> expr) => expr;
        public static Expression<Action<T1, T2, T3>> expr<T1, T2, T3>(Expression<Action<T1, T2, T3>> expr) => expr;
        public static Expression<Action<T1, T2, T3, T4>> expr<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> expr) => expr;
        public static Expression<Action<T1, T2, T3, T4, T5>> expr<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> expr) => expr;
        public static Expression<Action<T1, T2, T3, T4, T5, T6>> expr<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> expr) => expr;
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7>> expr<T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T1, T2, T3, T4, T5, T6, T7>> expr) => expr;
    }
}
