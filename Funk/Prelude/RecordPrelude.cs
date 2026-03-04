using System.Diagnostics.Contracts;

namespace Funk;

public static partial class Prelude
{
    /// <summary>
    /// Creates a Record of 1.
    /// </summary>
    /// <typeparam name="T1">The type of the value.</typeparam>
    /// <param name="t1">The value.</param>
    /// <returns>A new Record of 1.</returns>
    [Pure]
    public static Record<T1> rec<T1>(T1 t1) => t1.ToRecord();
        
    /// <summary>
    /// Creates a Record of 2.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="t1">The first value.</param>
    /// <param name="t2">The second value.</param>
    /// <returns>A new Record of 2.</returns>
    [Pure]
    public static Record<T1, T2> rec<T1, T2>(T1 t1, T2 t2) => (t1, t2).ToRecord();

    /// <summary>
    /// Creates a Record of 2 from ValueTuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="tuple">The value tuple.</param>
    /// <returns>A new Record of 2.</returns>
    [Pure]
    public static Record<T1, T2> rec<T1, T2>((T1 t1, T2 t2) tuple) => tuple.ToRecord();

    /// <summary>
    /// Creates a Record of 3.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="t1">The first value.</param>
    /// <param name="t2">The second value.</param>
    /// <param name="t3">The third value.</param>
    /// <returns>A new Record of 3.</returns>
    [Pure]
    public static Record<T1, T2, T3> rec<T1, T2, T3>(T1 t1, T2 t2, T3 t3) => (t1, t2, t3).ToRecord();

    /// <summary>
    /// Creates a Record of 3 from ValueTuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="tuple">The value tuple.</param>
    /// <returns>A new Record of 3.</returns>
    [Pure]
    public static Record<T1, T2, T3> rec<T1, T2, T3>((T1 t1, T2 t2, T3 t3) tuple) => tuple.ToRecord();

    /// <summary>
    /// Creates a Record of 4.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <param name="t1">The first value.</param>
    /// <param name="t2">The second value.</param>
    /// <param name="t3">The third value.</param>
    /// <param name="t4">The fourth value.</param>
    /// <returns>A new Record of 4.</returns>
    [Pure]
    public static Record<T1, T2, T3, T4> rec<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4) => (t1, t2, t3, t4).ToRecord();

    /// <summary>
    /// Creates a Record of 4 from ValueTuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <param name="tuple">The value tuple.</param>
    /// <returns>A new Record of 4.</returns>
    [Pure]
    public static Record<T1, T2, T3, T4> rec<T1, T2, T3, T4>((T1 t1, T2 t2, T3 t3, T4 t4) tuple) => tuple.ToRecord();

    /// <summary>
    /// Creates a Record of 5.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <param name="t1">The first value.</param>
    /// <param name="t2">The second value.</param>
    /// <param name="t3">The third value.</param>
    /// <param name="t4">The fourth value.</param>
    /// <param name="t5">The fifth value.</param>
    /// <returns>A new Record of 5.</returns>
    [Pure]
    public static Record<T1, T2, T3, T4, T5> rec<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => (t1, t2, t3, t4, t5).ToRecord();

    /// <summary>
    /// Creates a Record of 5 from ValueTuple.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <param name="tuple">The value tuple.</param>
    /// <returns>A new Record of 5.</returns>
    [Pure]
    public static Record<T1, T2, T3, T4, T5> rec<T1, T2, T3, T4, T5>((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) tuple) => tuple.ToRecord();
}