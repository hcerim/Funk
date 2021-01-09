using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Funk.Internal;
using static Funk.Prelude;

namespace Funk
{
    public struct Pattern<R> : IEnumerable
    {
        private List<Record<Func<object, bool>, Func<object, R>>> patterns;

        public void Add<T>((T @case, Func<T, R> function) item)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Func<object, bool>, Func<object, R>>>();
            }
            patterns.AddRange(item.@case.AsMaybe().FlatMap(c =>
                item.function.AsMaybe().Map(f => rec(func((object o) => o.SafeEquals(c)), func((object o) => f(c))).ToImmutableList())
            ).GetOrEmpty());
        }

        public void Add<T>((Func<T, bool> predicate, Func<T, R> function) item)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Func<object, bool>, Func<object, R>>>();
            }
            patterns.AddRange(item.predicate.AsMaybe().FlatMap(p =>
                item.function.AsMaybe().Map(f => rec(func((object o) => o.SafeCast<T>().Map(p).GetOrDefault()), func((object o) => f((T)o))).ToImmutableList())
            ).GetOrEmpty());
        }

        public Maybe<R> Match(object value) => patterns.AsFirstOrDefault(i => i.Item1(value)).Map(r => r.Item2.Apply(value));

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }

    public struct AsyncPattern<R> : IEnumerable
    {
        private List<Record<Func<object, bool>, Func<object, Task<R>>>> patterns;

        public void Add<T>((T @case, Func<T, Task<R>> function) item)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Func<object, bool>, Func<object, Task<R>>>>();
            }
            patterns.AddRange(item.@case.AsMaybe().FlatMap(c =>
                item.function.AsMaybe().Map(f => rec(func((object o) => o.SafeEquals(c)), func((object o) => f(c))).ToImmutableList())
            ).GetOrEmpty());
        }
        
        public void Add<T>((Func<T, bool> predicate, Func<T, Task<R>> function) item)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Func<object, bool>, Func<object, Task<R>>>>();
            }
            patterns.AddRange(item.predicate.AsMaybe().FlatMap(p =>
                item.function.AsMaybe().Map(f => rec(func((object o) => o.SafeCast<T>().Map(p).GetOrDefault()), func((object o) => f((T)o))).ToImmutableList())
            ).GetOrEmpty());
        }

        public Task<Maybe<R>> Match(object value) => patterns.AsFirstOrDefault(i => i.Item1(value)).MapAsync(r => r.Item2.Apply(value));

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }

    public struct TypePattern<R> : IEnumerable
    {
        private List<Record<Func<object, bool>, Func<object, R>>> patterns;

        public void Add<T>(Func<T, R> function)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Func<object, bool>, Func<object, R>>>();
            }
            patterns.AddRange(function.AsMaybe().Map(f =>
                rec(func((object o) => o is T), func((object o) => function((T)o))).ToImmutableList()
            ).GetOrEmpty());
        }

        public Maybe<R> Match(object value) => patterns.AsFirstOrDefault(i => i.Item1(value)).Map(r => r.Item2.Apply(value));

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }

    public struct AsyncTypePattern<R> : IEnumerable
    {
        private List<Record<Func<object, bool>, Func<object, Task<R>>>> patterns;

        public void Add<T>(Func<T, Task<R>> function)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Func<object, bool>, Func<object, Task<R>>>>();
            }
            patterns.AddRange(function.AsMaybe().Map(f =>
                rec(func((object o) => o is T), func((object o) => function((T)o))).ToImmutableList()
            ).GetOrEmpty());
        }

        public Task<Maybe<R>> Match(object value) => patterns.AsFirstOrDefault(i => i.Item1(value)).MapAsync(r => r.Item2.Apply(value));

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }
}
