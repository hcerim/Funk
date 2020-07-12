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
        internal List<Record<object, Func<object, R>>> patterns;

        public void Add<T>((T @case, Func<T, R> function) item)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<object, Func<object, R>>>();
            }
            patterns.AddRange(item.@case.AsMaybe().FlatMap(c => item.function.AsMaybe().Map(f => rec((object)c, func((object o) => f(c))).ToImmutableList())).GetOrEmpty());
        }

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }

    public struct AsyncPattern<R> : IEnumerable
    {
        internal List<Record<object, Func<object, Task<R>>>> patterns;

        public void Add<T>((T @case, Func<T, Task<R>> function) item)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<object, Func<object, Task<R>>>>();
            }
            patterns.AddRange(item.@case.AsMaybe().FlatMap(c => item.function.AsMaybe().Map(f => rec((object)c, func((object o) => f(c))).ToImmutableList())).GetOrEmpty());
        }

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }

    public struct TypePattern<R> : IEnumerable
    {
        internal List<Record<Type, Func<object, R>>> patterns;

        public void Add<T>(Func<T, R> function)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Type, Func<object, R>>>();
            }
            patterns.AddRange(function.AsMaybe().Map(f => rec<Type, Func<object, R>>(typeof(T), o => function((T)o)).ToImmutableList()).GetOrEmpty());
        }

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }

    public struct AsyncTypePattern<R> : IEnumerable
    {
        internal List<Record<Type, Func<object, Task<R>>>> patterns;

        public void Add<T>(Func<T, Task<R>> function)
        {
            if (patterns.IsNull())
            {
                patterns = new List<Record<Type, Func<object, Task<R>>>>();
            }
            patterns.AddRange(function.AsMaybe().Map(f => rec<Type, Func<object, Task<R>>>(typeof(T), o => function((T) o)).ToImmutableList()).GetOrEmpty());
        }

        IEnumerator IEnumerable.GetEnumerator() => patterns.GetEnumerator();
    }
}
