using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Funk;

public static partial class Prelude
{
    /// <summary>
    /// Infers the type of a Func delegate with a return value.
    /// </summary>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="func">The function to infer.</param>
    /// <returns>The same function with inferred types.</returns>
    [Pure]
    public static Func<R> func<R>(Func<R> func) => func;

    /// <summary>
    /// Infers the type of a Func delegate with 1 parameter and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="func">The function to infer.</param>
    /// <returns>The same function with inferred types.</returns>
    [Pure]
    public static Func<T1, R> func<T1, R>(Func<T1, R> func) => func;

    /// <summary>
    /// Infers the type of a Func delegate with 2 parameters and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="func">The function to infer.</param>
    /// <returns>The same function with inferred types.</returns>
    [Pure]
    public static Func<T1, T2, R> func<T1, T2, R>(Func<T1, T2, R> func) => func;

    /// <summary>
    /// Infers the type of a Func delegate with 3 parameters and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="func">The function to infer.</param>
    /// <returns>The same function with inferred types.</returns>
    [Pure]
    public static Func<T1, T2, T3, R> func<T1, T2, T3, R>(Func<T1, T2, T3, R> func) => func;

    /// <summary>
    /// Infers the type of a Func delegate with 4 parameters and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="func">The function to infer.</param>
    /// <returns>The same function with inferred types.</returns>
    [Pure]
    public static Func<T1, T2, T3, T4, R> func<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func) => func;

    /// <summary>
    /// Infers the type of a Func delegate with 5 parameters and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="func">The function to infer.</param>
    /// <returns>The same function with inferred types.</returns>
    [Pure]
    public static Func<T1, T2, T3, T4, T5, R> func<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func) => func;

    /// <summary>
    /// Infers the type of an Action delegate.
    /// </summary>
    /// <param name="act">The action to infer.</param>
    /// <returns>The same action with inferred types.</returns>
    [Pure]
    public static Action act(Action act) => act;

    /// <summary>
    /// Infers the type of an Action delegate with 1 parameter.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <param name="act">The action to infer.</param>
    /// <returns>The same action with inferred types.</returns>
    [Pure]
    public static Action<T1> act<T1>(Action<T1> act) => act;

    /// <summary>
    /// Infers the type of an Action delegate with 2 parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="act">The action to infer.</param>
    /// <returns>The same action with inferred types.</returns>
    [Pure]
    public static Action<T1, T2> act<T1, T2>(Action<T1, T2> act) => act;

    /// <summary>
    /// Infers the type of an Action delegate with 3 parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="act">The action to infer.</param>
    /// <returns>The same action with inferred types.</returns>
    [Pure]
    public static Action<T1, T2, T3> act<T1, T2, T3>(Action<T1, T2, T3> act) => act;

    /// <summary>
    /// Infers the type of an Action delegate with 4 parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="act">The action to infer.</param>
    /// <returns>The same action with inferred types.</returns>
    [Pure]
    public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4>(Action<T1, T2, T3, T4> act) => act;

    /// <summary>
    /// Infers the type of an Action delegate with 5 parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <param name="act">The action to infer.</param>
    /// <returns>The same action with inferred types.</returns>
    [Pure]
    public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> act) => act;

    /// <summary>
    /// Infers the type of an Expression wrapping a Func delegate with a return value.
    /// </summary>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Func<R>> exp<R>(Expression<Func<R>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping a Func delegate with 1 parameter and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Func<T1, R>> exp<T1, R>(Expression<Func<T1, R>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping a Func delegate with 2 parameters and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Func<T1, T2, R>> exp<T1, T2, R>(Expression<Func<T1, T2, R>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping a Func delegate with 3 parameters and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Func<T1, T2, T3, R>> exp<T1, T2, T3, R>(Expression<Func<T1, T2, T3, R>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping a Func delegate with 4 parameters and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Func<T1, T2, T3, T4, R>> exp<T1, T2, T3, T4, R>(Expression<Func<T1, T2, T3, T4, R>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping a Func delegate with 5 parameters and a return value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <typeparam name="R">The return type.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Func<T1, T2, T3, T4, T5, R>> exp<T1, T2, T3, T4, T5, R>(Expression<Func<T1, T2, T3, T4, T5, R>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping an Action delegate.
    /// </summary>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Action> exp(Expression<Action> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping an Action delegate with 1 parameter.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Action<T1>> exp<T1>(Expression<Action<T1>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping an Action delegate with 2 parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Action<T1, T2>> exp<T1, T2>(Expression<Action<T1, T2>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping an Action delegate with 3 parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Action<T1, T2, T3>> exp<T1, T2, T3>(Expression<Action<T1, T2, T3>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping an Action delegate with 4 parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Action<T1, T2, T3, T4>> exp<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> expr) => expr;

    /// <summary>
    /// Infers the type of an Expression wrapping an Action delegate with 5 parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <typeparam name="T3">The type of the third parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
    /// <param name="expr">The expression to infer.</param>
    /// <returns>The same expression with inferred types.</returns>
    [Pure]
    public static Expression<Action<T1, T2, T3, T4, T5>> exp<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> expr) => expr;
}