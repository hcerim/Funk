using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Funk.Internal;
using static Funk.Prelude;

namespace Funk
{
    public struct Pattern<R> : IEnumerable
    {
        private List<Record<object, Func<object, R>>> patterns;

        public void Add<T>((T @case, Func<T, R> function) item)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<object, Func<object, R>>>();
            }
            patterns.AddRange(item.@case.AsMaybe().FlatMap(c => item.function.AsMaybe().Map(f => rec((object)c, func((object o) => f(c))).ToImmutableList())).GetOrEmpty());
        }

        public Maybe<R> Match(object value) => patterns.AsFirstOrDefault(i => value.SafeEquals(i.Item1)).FlatMap(r => value.AsMaybe().Map(v => r.Item2.Apply(v)));

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }

    public struct AsyncPattern<R> : IEnumerable
    {
        private List<Record<object, Func<object, Task<R>>>> patterns;

        public void Add<T>((T @case, Func<T, Task<R>> function) item)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<object, Func<object, Task<R>>>>();
            }
            patterns.AddRange(item.@case.AsMaybe().FlatMap(c => item.function.AsMaybe().Map(f => rec((object)c, func((object o) => f(c))).ToImmutableList())).GetOrEmpty());
        }

        public Task<Maybe<R>> Match(object value) => patterns.AsFirstOrDefault(i => value.SafeEquals(i.Item1)).FlatMapAsync(r => value.AsMaybe().MapAsync(v => r.Item2.Apply(v)));

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }

    public struct TypePattern<R> : IEnumerable
    {
        private List<Record<Type, Func<object, R>>> patterns;

        public void Add<T>(Func<T, R> function)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Type, Func<object, R>>>();
            }
            patterns.AddRange(function.AsMaybe().Map(f => rec<Type, Func<object, R>>(typeof(T), o => function((T)o)).ToImmutableList()).GetOrEmpty());
        }

        public Maybe<R> Match(object value) => patterns.AsFirstOrDefault(i => value.GetType().GetTypeInfo().IsAssignableFrom(i.Item1.GetTypeInfo())).FlatMap(r => value.AsMaybe().Map(v => r.Item2.Apply(v)));

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }

    public struct AsyncTypePattern<R> : IEnumerable
    {
        private List<Record<Type, Func<object, Task<R>>>> patterns;

        public void Add<T>(Func<T, Task<R>> function)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Type, Func<object, Task<R>>>>();
            }
            patterns.AddRange(function.AsMaybe().Map(f => rec<Type, Func<object, Task<R>>>(typeof(T), o => function((T)o)).ToImmutableList()).GetOrEmpty());
        }

        public Task<Maybe<R>> Match(object value) => patterns.AsFirstOrDefault(i => value.GetType().GetTypeInfo().IsAssignableFrom(i.Item1.GetTypeInfo())).FlatMapAsync(r => value.AsMaybe().MapAsync(v => r.Item2.Apply(v)));

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }
}
