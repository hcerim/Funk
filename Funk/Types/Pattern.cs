using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Funk.Internal;

namespace Funk
{
    public static class Pattern
    {
        public static Pattern<T> Match<T>(params (object @case, Func<object, T> function)[] sequence) => new Pattern<T>(sequence.WhereOrDefault(r => r.@case.IsNotNull() && r.function.IsNotNull()).Map(l => l.Map(r => r.ToRecord())).GetOrEmpty());

        public static Maybe<T> Apply<T>(this Pattern<T> pattern, object value)
        {
            return pattern.Patterns.FlatMap(t => t.AsFirstOrDefault(i => i.Item1.SafeEquals(value))).Map(r => r.Item2.Apply(value));
        }
    }

    public static class AsyncPattern
    {
        public static AsyncPattern<T> Match<T>(params (object @case, Func<object, Task<T>> function)[] sequence) => new AsyncPattern<T>(sequence.WhereOrDefault(r => r.@case.IsNotNull() && r.function.IsNotNull()).Map(l => l.Map(r => r.ToRecord())).GetOrEmpty());

        public static Task<Maybe<T>> Apply<T>(this AsyncPattern<T> pattern, object value)
        {
            return pattern.Patterns.FlatMap(t => t.AsFirstOrDefault(i => i.Item1.SafeEquals(value))).MapAsync(r => r.Item2.Apply(value));
        }
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
}
