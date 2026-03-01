using System;

namespace Funk;

/// <summary>
/// Provides partial application extension methods for Func and Action delegates.
/// </summary>
public static class PartialExt
{
    /// <summary>
    /// Applies the argument to the function, executing it and returning the result.
    /// </summary>
    public static R Apply<T1, R>(this Func<T1, R> function, T1 t1) => function(t1);
        
    /// <summary>
    /// Applies the argument to the action, executing it and returning Unit.
    /// </summary>
    public static Unit Apply<T1>(this Action<T1> function, T1 t1)
    {
        function(t1);
        return Unit.Value;
    }

    /// <summary>
    /// Partially applies the first argument to the function, returning a function with reduced arity.
    /// </summary>
    public static Func<T2, R> Apply<T1, T2, R>(this Func<T1, T2, R> function, T1 t1) => t2 => function(t1, t2);
        
    /// <summary>
    /// Partially applies the first argument to the action, returning an action with reduced arity.
    /// </summary>
    public static Action<T2> Apply<T1, T2>(this Action<T1, T2> function, T1 t1) => t2 => function(t1, t2);

    /// <summary>
    /// Partially applies the first argument to the function, returning a function with reduced arity.
    /// </summary>
    public static Func<T2, T3, R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> function, T1 t1) => (t2, t3) => function(t1, t2, t3);
        
    /// <summary>
    /// Partially applies the first argument to the action, returning an action with reduced arity.
    /// </summary>
    public static Action<T2, T3> Apply<T1, T2, T3>(this Action<T1, T2, T3> function, T1 t1) => (t2, t3) => function(t1, t2, t3);

    /// <summary>
    /// Partially applies the first argument to the function, returning a function with reduced arity.
    /// </summary>
    public static Func<T2, T3, T4, R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> function, T1 t1) => (t2, t3, t4) => function(t1, t2, t3, t4);
        
    /// <summary>
    /// Partially applies the first argument to the action, returning an action with reduced arity.
    /// </summary>
    public static Action<T2, T3, T4> Apply<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> function, T1 t1) => (t2, t3, t4) => function(t1, t2, t3, t4);

    /// <summary>
    /// Partially applies the first argument to the function, returning a function with reduced arity.
    /// </summary>
    public static Func<T2, T3, T4, T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> function, T1 t1) => (t2, t3, t4, t5) => function(t1, t2, t3, t4, t5);
        
    /// <summary>
    /// Partially applies the first argument to the action, returning an action with reduced arity.
    /// </summary>
    public static Action<T2, T3, T4, T5> Apply<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> function, T1 t1) => (t2, t3, t4, t5) => function(t1, t2, t3, t4, t5);
}