using System;
using System.Diagnostics.Contracts;

namespace Funk;

public static partial class Prelude
{
    /// <summary>
    /// Creates a successful Exc with the specified value.
    /// </summary>
    /// <typeparam name="T">The success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <param name="item">The success value.</param>
    /// <returns>A successful Exc.</returns>
    [Pure]
    public static Exc<T, E> success<T, E>(T item) where E : Exception => Exc.Success<T, E>(item);

    /// <summary>
    /// Creates a failed Exc with the specified exception.
    /// </summary>
    /// <typeparam name="T">The success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <param name="exception">The exception.</param>
    /// <returns>A failed Exc.</returns>
    [Pure]
    public static Exc<T, E> failure<T, E>(E exception) where E : Exception => Exc.Failure<T, E>(exception);

    /// <summary>
    /// Creates a failed Exc with the specified enumerable exception.
    /// </summary>
    /// <typeparam name="T">The success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <param name="exception">The enumerable exception.</param>
    /// <returns>A failed Exc.</returns>
    [Pure]
    public static Exc<T, E> failure<T, E>(EnumerableException<E> exception) where E : Exception => Exc.Failure<T, E>(exception);
}