using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using Funk.Internal;
using static Funk.Prelude;

namespace Funk
{
    public static class Pattern
    {
        public static Pattern<T> Match<T>(params (object @case, Func<object, T> function)[] sequence) => new Pattern<T>(sequence.WhereOrDefault(r => r.@case.IsNotNull() && r.function.IsNotNull()).Map(l => l.Map(r => r.ToRecord())).GetOrEmpty());

        public static Pattern<R> Match<T, R>(params (object @case, Func<T, R> function)[] sequence) => new Pattern<R>(sequence.WhereOrDefault(r => r.@case.IsNotNull() && r.function.IsNotNull()).Map(l => l.Map(r => rec<object, Func<object, R>>(r.@case, o => r.function((T)r.@case)))).GetOrEmpty());

        public static Maybe<T> Apply<T>(this Pattern<T> pattern, object value)
        {
            return pattern.Patterns.FlatMap(t => t.AsFirstOrDefault(i => i.Item1.SafeEquals(value))).Map(r => r.Item2.Apply(value));
        }
    }

    public static class AsyncPattern
    {
        public static AsyncPattern<T> Match<T>(params (object @case, Func<object, Task<T>> function)[] sequence) => new AsyncPattern<T>(sequence.WhereOrDefault(r => r.@case.IsNotNull() && r.function.IsNotNull()).Map(l => l.Map(r => r.ToRecord())).GetOrEmpty());

        public static AsyncPattern<R> Match<T, R>(params (object @case, Func<T, Task<R>> function)[] sequence) => new AsyncPattern<R>(sequence.WhereOrDefault(r => r.@case.IsNotNull() && r.function.IsNotNull()).Map(l => l.Map(r => rec<object, Func<object, Task<R>>>(r.@case, o => r.function((T)r.@case)))).GetOrEmpty());

        public static Task<Maybe<T>> Apply<T>(this AsyncPattern<T> pattern, object value)
        {
            return pattern.Patterns.FlatMap(t => t.AsFirstOrDefault(i => i.Item1.SafeEquals(value))).MapAsync(r => r.Item2.Apply(value));
        }
    }

    public static class TypePattern
    {
        public static Maybe<T> Apply<T>(this TypePattern<T> pattern, object value) => pattern.AsMaybe().FlatMap(p => p.patterns.AsFirstOrDefault(i => value.GetType().GetTypeInfo().IsAssignableFrom(i.Item1.GetTypeInfo())).FlatMap(r => value.AsMaybe().Map(v => r.Item2.Apply(v))));
    }

    public static class AsyncTypePattern
    {
        public static Task<Maybe<T>> Apply<T>(this AsyncTypePattern<T> pattern, object value) => pattern.AsMaybe().FlatMapAsync(p => p.patterns.AsFirstOrDefault(i => value.GetType().GetTypeInfo().IsAssignableFrom(i.Item1.GetTypeInfo())).FlatMapAsync(r => value.AsMaybe().MapAsync(v => r.Item2.Apply(v))));
    }

    public readonly struct Pattern<T>
    {
        internal Pattern(IImmutableList<Record<object, Func<object, T>>> patterns)
        {
            Patterns = patterns.AsMaybe();
        }

        internal Maybe<IImmutableList<Record<object, Func<object, T>>>> Patterns { get; }
    }

    public readonly struct AsyncPattern<T>
    {
        internal AsyncPattern(IImmutableList<Record<object, Func<object, Task<T>>>> patterns)
        {
            Patterns = patterns.AsMaybe();
        }

        internal Maybe<IImmutableList<Record<object, Func<object, Task<T>>>>> Patterns { get; }
    }

    public sealed class TypePattern<R> : IEnumerable
    {
        internal readonly List<Record<Type, Func<object, R>>> patterns = new List<Record<Type, Func<object, R>>>();

        public void Add<T>(Func<T, R> function) => patterns.AddRange(function.AsMaybe().Map(f => rec<Type, Func<object, R>>(typeof(T), o => function((T)o)).ToImmutableList()).GetOrEmpty());

        public IEnumerator GetEnumerator() => patterns.GetEnumerator();
    }

    public sealed class AsyncTypePattern<R> : IEnumerable
    {
        internal readonly List<Record<Type, Func<object, Task<R>>>> patterns = new List<Record<Type, Func<object, Task<R>>>>();

        public void Add<T>(Func<T, Task<R>> function) => patterns.AddRange(function.AsMaybe().Map(f => rec<Type, Func<object, Task<R>>>(typeof(T), o => function((T)o)).ToImmutableList()).GetOrEmpty());

        public IEnumerator GetEnumerator() => patterns.GetEnumerator();
    }
}
