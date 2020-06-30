using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    public static class Pattern
    {
        public static Pattern<T> Match<T>(params (object, Func<object, T>)[] sequence) => new Pattern<T>(sequence.Map(r => rec(r.Item1.AsMaybe(), r.Item2.AsMaybe())));

        public static Maybe<T> Apply<T>(this Pattern<T> pattern, object value)
        {
            return pattern.Patterns.FlatMap(t => t.AsFirstOrDefault(i => i.Item1.SafeEquals(value))).Map(r => r.Item2.Apply(value));
        }
    }

    public static class AsyncPattern
    {
        public static AsyncPattern<T> Match<T>(params (object, Func<object, Task<T>>)[] sequence) => new AsyncPattern<T>(sequence.Map(r => rec(r.Item1.AsMaybe(), r.Item2.AsMaybe())));

        public static Task<Maybe<T>> Apply<T>(this AsyncPattern<T> pattern, object value)
        {
            return pattern.Patterns.FlatMap(t => t.AsFirstOrDefault(i => i.Item1.SafeEquals(value))).MapAsync(r => r.Item2.Apply(value));
        }
    }

    public readonly struct Pattern<T>
    {
        internal Pattern(IImmutableList<Record<Maybe<object>, Maybe<Func<object, T>>>> patterns)
        {
            Patterns = patterns.WhereOrDefault(r => r.Item1.NotEmpty && r.Item2.NotEmpty).Map(l => l.Map(r => r.Map((a, b) => (a.UnsafeGet(), b.UnsafeGet()))));
        }

        internal Maybe<IImmutableList<Record<object, Func<object, T>>>> Patterns { get; }
    }

    public readonly struct AsyncPattern<T>
    {
        internal AsyncPattern(IImmutableList<Record<Maybe<object>, Maybe<Func<object, Task<T>>>>> patterns)
        {
            Patterns = patterns.WhereOrDefault(r => r.Item1.NotEmpty && r.Item2.NotEmpty).Map(l => l.Map(r => r.Map((a, b) => (a.UnsafeGet(), b.UnsafeGet()))));
        }

        internal Maybe<IImmutableList<Record<object, Func<object, Task<T>>>>> Patterns { get; }
    }
}
