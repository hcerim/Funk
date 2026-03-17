using System;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Funk.Internal;
using static Funk.Prelude;

namespace Funk;

/// <summary>
/// Type that represents the lazy pattern-matching expression.
/// </summary>
/// <typeparam name="R">The result type of the pattern match.</typeparam>
public struct Pattern<R> : IEnumerable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IImmutableList<Record<Func<object, bool>, Func<object, R>>> patterns;

    /// <summary>
    /// Adds a new case-expression.
    /// If either the case or the expression is null, case-expression is ignored.
    /// </summary>
    /// <typeparam name="T">The type of the case value.</typeparam>
    /// <param name="item">A tuple of the case value and the function to execute if matched.</param>
    public void Add<T>((T @case, Func<T, R> function) item)
    {
        patterns ??= ImmutableList<Record<Func<object, bool>, Func<object, R>>>.Empty;
        patterns = patterns.AddRange(item.@case.AsMaybe().FlatMap(c =>
            item.function.AsMaybe().Map(f => rec(func((object o) => o.SafeEquals(c)), func((object o) => f(c))).ToImmutableList())
        ).GetOrEmpty());
    }

    /// <summary>
    /// Adds a new predicate-expression.
    /// If either the predicate or the expression is null, predicate-expression is ignored.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="item">A tuple of the predicate and the function to execute if satisfied.</param>
    public void Add<T>((Func<T, bool> predicate, Func<T, R> function) item)
    {
        patterns ??= ImmutableList<Record<Func<object, bool>, Func<object, R>>>.Empty;
        patterns = patterns.AddRange(item.predicate.AsMaybe().FlatMap(p =>
            item.function.AsMaybe().Map(f => rec(func((object o) => o.SafeCast<T>().Map(p).GetOrDefault()), func((object o) => f((T)o))).ToImmutableList())
        ).GetOrEmpty());
    }

    /// <summary>
    /// Evaluates the underlying expressions, and for the first one that satisfies, returns its result.
    /// Expression is evaluated only if there is a matching case/predicate.
    /// In case none of them satisfy, the result will be empty.
    /// If the specified value is null, the result will be empty.
    /// The default case can be provided using the GetOr or Or functions after this call.
    /// </summary>
    /// <param name="value">The value to match against.</param>
    /// <returns>A Maybe containing the result of the first matching expression, or an empty Maybe.</returns>
    public Maybe<R> Match(object value) => patterns.AsFirstOrDefault(i => i.Item1(value)).Map(r => r.Item2.Apply(value));

    IEnumerator IEnumerable.GetEnumerator() => (patterns ?? ImmutableList<Record<Func<object, bool>, Func<object, R>>>.Empty).GetEnumerator();
}

/// <summary>
/// Type that represents the lazy asynchronous pattern-matching expression.
/// </summary>
/// <typeparam name="R">The result type of the pattern match.</typeparam>
public struct AsyncPattern<R> : IEnumerable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IImmutableList<Record<Func<object, bool>, Func<object, Task<R>>>> patterns;

    /// <summary>
    /// Adds a new case-expression.
    /// If either the case or the expression is null, case-expression is ignored.
    /// </summary>
    /// <typeparam name="T">The type of the case value.</typeparam>
    /// <param name="item">A tuple of the case value and the function to execute if matched.</param>
    public void Add<T>((T @case, Func<T, Task<R>> function) item)
    {
        patterns ??= ImmutableList<Record<Func<object, bool>, Func<object, Task<R>>>>.Empty;
        patterns = patterns.AddRange(item.@case.AsMaybe().FlatMap(c =>
            item.function.AsMaybe().Map(f => rec(func((object o) => o.SafeEquals(c)), func((object o) => f(c))).ToImmutableList())
        ).GetOrEmpty());
    }

    /// <summary>
    /// Adds a new predicate-expression.
    /// If either the predicate or the expression is null, predicate-expression is ignored.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="item">A tuple of the predicate and the function to execute if satisfied.</param>
    public void Add<T>((Func<T, bool> predicate, Func<T, Task<R>> function) item)
    {
        patterns ??= ImmutableList<Record<Func<object, bool>, Func<object, Task<R>>>>.Empty;
        patterns = patterns.AddRange(item.predicate.AsMaybe().FlatMap(p =>
            item.function.AsMaybe().Map(f => rec(func((object o) => o.SafeCast<T>().Map(p).GetOrDefault()), func((object o) => f((T)o))).ToImmutableList())
        ).GetOrEmpty());
    }

    /// <summary>
    /// Evaluates the underlying expressions, and for the first one that satisfies, returns its result.
    /// Expression is evaluated only if there is a matching case/predicate.
    /// In case none of them satisfy, the result will be empty.
    /// If the specified value is null, the result will be empty.
    /// The default case can be provided using the GetOrAsync or OrAsync functions after this call.
    /// </summary>
    /// <param name="value">The value to match against.</param>
    /// <returns>A Maybe containing the result of the first matching expression, or an empty Maybe.</returns>
    public Task<Maybe<R>> Match(object value) => patterns.AsFirstOrDefault(i => i.Item1(value)).MapAsync(r => r.Item2.Apply(value));

    IEnumerator IEnumerable.GetEnumerator() => (patterns ?? ImmutableList<Record<Func<object, bool>, Func<object, Task<R>>>>.Empty).GetEnumerator();
}

/// <summary>
/// Type that represents the lazy type pattern-matching expression.
/// </summary>
/// <typeparam name="R">The result type of the pattern match.</typeparam>
public struct TypePattern<R> : IEnumerable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IImmutableList<Record<Func<object, bool>, Func<object, R>>> patterns;

    /// <summary>
    /// Adds a new expression.
    /// If the expression is null, it is ignored.
    /// </summary>
    /// <typeparam name="T">The type to match against.</typeparam>
    /// <param name="function">The function to execute if the value is of type T.</param>
    public void Add<T>(Func<T, R> function)
    {
        patterns ??= ImmutableList<Record<Func<object, bool>, Func<object, R>>>.Empty;
        patterns = patterns.AddRange(function.AsMaybe().Map(f =>
            rec(func((object o) => o is T), func((object o) => function((T)o))).ToImmutableList()
        ).GetOrEmpty());
    }

    /// <summary>
    /// Evaluates the underlying expressions, and for the first one that satisfies, returns its result.
    /// Expression is evaluated only if there is a matching type.
    /// In case none of them satisfy, the result will be empty.
    /// If the specified value is null, the result will be empty.
    /// The default case can be provided using the GetOr or Or functions after this call.
    /// </summary>
    /// <param name="value">The value to match against.</param>
    /// <returns>A Maybe containing the result of the first matching expression, or an empty Maybe.</returns>
    public Maybe<R> Match(object value) => patterns.AsFirstOrDefault(i => i.Item1(value)).Map(r => r.Item2.Apply(value));

    IEnumerator IEnumerable.GetEnumerator() => (patterns ?? ImmutableList<Record<Func<object, bool>, Func<object, R>>>.Empty).GetEnumerator();
}

/// <summary>
/// Type that represents the lazy asynchronous type pattern-matching expression.
/// </summary>
/// <typeparam name="R">The result type of the pattern match.</typeparam>
public struct AsyncTypePattern<R> : IEnumerable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IImmutableList<Record<Func<object, bool>, Func<object, Task<R>>>> patterns;

    /// <summary>
    /// Adds a new expression.
    /// If the expression is null, it is ignored.
    /// </summary>
    /// <typeparam name="T">The type to match against.</typeparam>
    /// <param name="function">The function to execute if the value is of type T.</param>
    public void Add<T>(Func<T, Task<R>> function)
    {
        patterns ??= ImmutableList<Record<Func<object, bool>, Func<object, Task<R>>>>.Empty;
        patterns = patterns.AddRange(function.AsMaybe().Map(f =>
            rec(func((object o) => o is T), func((object o) => function((T)o))).ToImmutableList()
        ).GetOrEmpty());
    }

    /// <summary>
    /// Evaluates the underlying expressions, and for the first one that satisfies, returns its result.
    /// Expression is evaluated only if there is a matching type.
    /// In case none of them satisfy, the result will be empty.
    /// If the specified value is null, the result will be empty.
    /// The default case can be provided using the GetOrAsync or OrAsync functions after this call.
    /// </summary>
    /// <param name="value">The value to match against.</param>
    /// <returns>A Maybe containing the result of the first matching expression, or an empty Maybe.</returns>
    public Task<Maybe<R>> Match(object value) => patterns.AsFirstOrDefault(i => i.Item1(value)).MapAsync(r => r.Item2.Apply(value));

    IEnumerator IEnumerable.GetEnumerator() => (patterns ?? ImmutableList<Record<Func<object, bool>, Func<object, Task<R>>>>.Empty).GetEnumerator();
}