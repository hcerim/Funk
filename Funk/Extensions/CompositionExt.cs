using System;

namespace Funk;

/// <summary>
/// Provides extension methods for function composition.
/// </summary>
public static class CompositionExt
{
    /// <summary>
    /// Composes 2 functions from right to left.
    /// </summary>
    /// <typeparam name="T1">The input type of the composed function.</typeparam>
    /// <typeparam name="T2">The intermediate type.</typeparam>
    /// <typeparam name="T3">The output type of the composed function.</typeparam>
    /// <param name="f1">The outer function.</param>
    /// <param name="f2">The inner function.</param>
    /// <returns>A composed function.</returns>
    public static Func<T1, T3> ComposeRight<T1, T2, T3>(this Func<T2, T3> f1, Func<T1, T2> f2) => i => f1(f2(i));

    /// <summary>
    /// Composes 2 functions from right to left.
    /// </summary>
    /// <typeparam name="T1">The input type of the composed function.</typeparam>
    /// <typeparam name="T2">The output type of the composed function.</typeparam>
    /// <param name="f1">The outer function.</param>
    /// <param name="f2">The inner action to execute before the function.</param>
    /// <returns>A composed function.</returns>
    public static Func<T1, T2> ComposeRight<T1, T2>(this Func<T1, T2> f1, Action<T1> f2) =>
        i =>
        {
            f2(i);
            return f1(i);
        };
        
    /// <summary>
    /// Composes 2 functions from right to left.
    /// </summary>
    /// <typeparam name="T1">The input type of the composed function.</typeparam>
    /// <typeparam name="T2">The output type of the composed function.</typeparam>
    /// <param name="f1">The outer action to execute after the function.</param>
    /// <param name="f2">The inner function.</param>
    /// <returns>A composed function.</returns>
    public static Func<T1, T2> ComposeRight<T1, T2>(this Action<T1, T2> f1, Func<T1, T2> f2) =>
        i =>
        {
            var result = f2(i);
            f1(i, result);
            return result;
        };

    /// <summary>
    /// Composes 2 functions from left to right.
    /// </summary>
    /// <typeparam name="T1">The input type of the composed function.</typeparam>
    /// <typeparam name="T2">The intermediate type.</typeparam>
    /// <typeparam name="T3">The output type of the composed function.</typeparam>
    /// <param name="f1">The inner function.</param>
    /// <param name="f2">The outer function.</param>
    /// <returns>A composed function.</returns>
    public static Func<T1, T3> ComposeLeft<T1, T2, T3>(this Func<T1, T2> f1, Func<T2, T3> f2) => i => f2(f1(i));
        
    /// <summary>
    /// Composes 2 functions from left to right.
    /// </summary>
    /// <typeparam name="T1">The input type of the composed function.</typeparam>
    /// <typeparam name="T2">The output type of the composed function.</typeparam>
    /// <param name="f1">The inner action to execute before the function.</param>
    /// <param name="f2">The outer function.</param>
    /// <returns>A composed function.</returns>
    public static Func<T1, T2> ComposeLeft<T1, T2>(this Action<T1> f1, Func<T1, T2> f2) =>
        i =>
        {
            f1(i);
            return f2(i);
        };
        
    /// <summary>
    /// Composes 2 functions from left to right.
    /// </summary>
    /// <typeparam name="T1">The input type of the composed function.</typeparam>
    /// <typeparam name="T2">The output type of the composed function.</typeparam>
    /// <param name="f2">The inner function.</param>
    /// <param name="f1">The outer action to execute after the function.</param>
    /// <returns>A composed function.</returns>
    public static Func<T1, T2> ComposeLeft<T1, T2>(this Func<T1, T2> f2, Action<T1, T2> f1) =>
        i =>
        {
            var result = f2(i);
            f1(i, result);
            return result;
        };
}
