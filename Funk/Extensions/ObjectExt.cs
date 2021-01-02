using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Funk.Internal;
using static Funk.Internal.InternalExt;

namespace Funk
{
    public static class ObjectExt
    {
        /// <summary>
        /// Acts as a pipe that performs the operation and returns the argument that initiated the pipeline.
        /// </summary>
        public static T Do<T>(this T item, Action<T> action)
        {
            action(item);
            return item;
        }

        /// <summary>
        /// Acts as a pipe that returns the result of the function provided with the argument that initiated the pipeline.
        /// </summary>
        public static R Do<T, R>(this T item, Func<T, R> function) => function(item);

        /// <summary>
        /// Acts as a pipe that returns the result of the function provided with the argument that initiated the pipeline.
        /// </summary>
        public static async Task<T> DoAsync<T>(this T item, Func<T, Task> function)
        {
            await function(item).ConfigureAwait(false);
            return item;
        }

        /// <summary>
        /// Acts as a pipe that returns the result of the function provided with the argument that initiated the pipeline.
        /// </summary>
        public static async Task<T> DoAsync<T>(this Task<T> item, Func<T, Task> function)
        {
            var i = await item.ConfigureAwait(false);
            await function(i);
            return i;
        }

        /// <summary>
        /// Acts as a pipe that returns the result of the function provided with the argument that initiated the pipeline.
        /// </summary>
        public static Task<R> DoAsync<T, R>(this T item, Func<T, Task<R>> function) => function(item);

        /// <summary>
        /// Acts as a pipe that returns the result of the function provided with the argument that initiated the pipeline.
        /// </summary>
        public static async Task<R> DoAsync<T, R>(this Task<T> item, Func<T, Task<R>> function) => await function(await item).ConfigureAwait(false);

        /// <summary>
        /// Safely casts object to the specified type. Returns empty maybe if the casting fails.
        /// </summary>
        public static Maybe<R> SafeCast<R>(this object item)
        {
            if (item is R valid)
            {
                return Maybe.Create(valid);
            }
            return Maybe.Empty<R>();
        }
        
        public static Maybe<R> Match<T, R>(
            this T obj,
            params ValueTuple<T, Func<T, R>>[] patterns
        ) => patterns.AsFirstOrDefault(p => p.Item1.SafeEquals(obj)).Map(p => p.Item2.Apply(obj));
        
        public static Maybe<Unit> Match<T>(
            this T obj,
            params ValueTuple<T, Action<T>>[] patterns
        ) => patterns.AsFirstOrDefault(p => p.Item1.SafeEquals(obj)).Map(p => p.Item2.Apply(obj));
        
        public static Maybe<R> Match<T, R>(
            this T obj,
            params ValueTuple<Func<T, bool>, Func<T, R>>[] patterns
        ) => patterns.AsFirstOrDefault(p => p.Item1.Apply(obj)).Map(p => p.Item2.Apply(obj));
        
        public static Maybe<Unit> Match<T>(
            this T obj,
            params ValueTuple<Func<T, bool>, Action<T>>[] patterns
        ) => patterns.AsFirstOrDefault(p => p.Item1.Apply(obj)).Map(p => p.Item2.Apply(obj));
        
        public static Maybe<R> Match<T, R>(
            this T obj,
            params ValueTuple<IEnumerable<T>, Func<Unit, R>>[] patterns
        ) => patterns.AsFirstOrDefault(p => p.Item1.SafeAnyEquals(obj)).Map(p => p.Item2.Apply(Unit.Value));
        
        public static Maybe<Unit> Match<T>(
            this T obj,
            params ValueTuple<IEnumerable<T>, Action<Unit>>[] patterns
        ) => patterns.AsFirstOrDefault(p => p.Item1.SafeAnyEquals(obj)).Map(p => p.Item2.Apply(Unit.Value));

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            return obj.SafeEquals(case1) ? selector1(obj) : Otherwise(otherwise, otherwiseThrow);
        }
        
        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            return predicate1(obj) ? selector1(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            T case2, Func<T, R> selector2,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                return selector1(obj);
            }
            return obj.SafeEquals(case2) ? selector2(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<T, bool> predicate2, Func<T, R> selector2,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (predicate1(obj))
            {
                return selector1(obj);
            }
            return predicate2(obj) ? selector2(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            T case2, Action<T> selector2,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
                return;
            }
            if (obj.SafeEquals(case2))
            {
                selector2(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Func<T, bool> predicate2, Action<T> selector2,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
                return;
            }
            if (predicate2(obj))
            {
                selector2(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            T case2, Func<T, R> selector2,
            T case3, Func<T, R> selector3,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                return selector1(obj);
            }
            if (obj.SafeEquals(case2))
            {
                return selector2(obj);
            }
            return obj.SafeEquals(case3) ? selector3(obj) : Otherwise(otherwise, otherwiseThrow);
        }
        
        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<T, bool> predicate2, Func<T, R> selector2,
            Func<T, bool> predicate3, Func<T, R> selector3,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (predicate1(obj))
            {
                return selector1(obj);
            }
            if (predicate2(obj))
            {
                return selector2(obj);
            }
            return predicate3(obj) ? selector3(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            T case2, Action<T> selector2,
            T case3, Action<T> selector3,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
                return;
            }
            if (obj.SafeEquals(case2))
            {
                selector2(obj);
                return;
            }
            if (obj.SafeEquals(case3))
            {
                selector3(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Func<T, bool> predicate2, Action<T> selector2,
            Func<T, bool> predicate3, Action<T> selector3,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
                return;
            }
            if (predicate2(obj))
            {
                selector2(obj);
                return;
            }
            if (predicate3(obj))
            {
                selector3(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            T case2, Func<T, R> selector2,
            T case3, Func<T, R> selector3,
            T case4, Func<T, R> selector4,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                return selector1(obj);
            }
            if (obj.SafeEquals(case2))
            {
                return selector2(obj);
            }
            if (obj.SafeEquals(case3))
            {
                return selector3(obj);
            }
            return obj.SafeEquals(case4) ? selector4(obj) : Otherwise(otherwise, otherwiseThrow);
        }
        
        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<T, bool> predicate2, Func<T, R> selector2,
            Func<T, bool> predicate3, Func<T, R> selector3,
            Func<T, bool> predicate4, Func<T, R> selector4,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (predicate1(obj))
            {
                return selector1(obj);
            }
            if (predicate2(obj))
            {
                return selector2(obj);
            }
            if (predicate3(obj))
            {
                return selector3(obj);
            }
            return predicate4(obj) ? selector4(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            T case2, Action<T> selector2,
            T case3, Action<T> selector3,
            T case4, Action<T> selector4,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
                return;
            }
            if (obj.SafeEquals(case2))
            {
                selector2(obj);
                return;
            }
            if (obj.SafeEquals(case3))
            {
                selector3(obj);
                return;
            }
            if (obj.SafeEquals(case4))
            {
                selector4(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Func<T, bool> predicate2, Action<T> selector2,
            Func<T, bool> predicate3, Action<T> selector3,
            Func<T, bool> predicate4, Action<T> selector4,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
                return;
            }
            if (predicate2(obj))
            {
                selector2(obj);
                return;
            }
            if (predicate3(obj))
            {
                selector3(obj);
                return;
            }
            if (predicate4(obj))
            {
                selector4(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            T case2, Func<T, R> selector2,
            T case3, Func<T, R> selector3,
            T case4, Func<T, R> selector4,
            T case5, Func<T, R> selector5,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                return selector1(obj);
            }
            if (obj.SafeEquals(case2))
            {
                return selector2(obj);
            }
            if (obj.SafeEquals(case3))
            {
                return selector3(obj);
            }
            if (obj.SafeEquals(case4))
            {
                return selector4(obj);
            }
            return obj.SafeEquals(case5) ? selector5(obj) : Otherwise(otherwise, otherwiseThrow);
        }
        
        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<T, bool> predicate2, Func<T, R> selector2,
            Func<T, bool> predicate3, Func<T, R> selector3,
            Func<T, bool> predicate4, Func<T, R> selector4,
            Func<T, bool> predicate5, Func<T, R> selector5,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (predicate1(obj))
            {
                return selector1(obj);
            }
            if (predicate2(obj))
            {
                return selector2(obj);
            }
            if (predicate3(obj))
            {
                return selector3(obj);
            }
            if (predicate4(obj))
            {
                return selector4(obj);
            }
            return predicate5(obj) ? selector5(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            T case2, Action<T> selector2,
            T case3, Action<T> selector3,
            T case4, Action<T> selector4,
            T case5, Action<T> selector5,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
                return;
            }
            if (obj.SafeEquals(case2))
            {
                selector2(obj);
                return;
            }
            if (obj.SafeEquals(case3))
            {
                selector3(obj);
                return;
            }
            if (obj.SafeEquals(case4))
            {
                selector4(obj);
                return;
            }
            if (obj.SafeEquals(case5))
            {
                selector5(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Func<T, bool> predicate2, Action<T> selector2,
            Func<T, bool> predicate3, Action<T> selector3,
            Func<T, bool> predicate4, Action<T> selector4,
            Func<T, bool> predicate5, Action<T> selector5,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
                return;
            }
            if (predicate2(obj))
            {
                selector2(obj);
                return;
            }
            if (predicate3(obj))
            {
                selector3(obj);
                return;
            }
            if (predicate4(obj))
            {
                selector4(obj);
                return;
            }
            if (predicate5(obj))
            {
                selector5(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            T case2, Func<T, R> selector2,
            T case3, Func<T, R> selector3,
            T case4, Func<T, R> selector4,
            T case5, Func<T, R> selector5,
            T case6, Func<T, R> selector6,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                return selector1(obj);
            }
            if (obj.SafeEquals(case2))
            {
                return selector2(obj);
            }
            if (obj.SafeEquals(case3))
            {
                return selector3(obj);
            }
            if (obj.SafeEquals(case4))
            {
                return selector4(obj);
            }
            if (obj.SafeEquals(case5))
            {
                return selector5(obj);
            }
            return obj.SafeEquals(case6) ? selector6(obj) : Otherwise(otherwise, otherwiseThrow);
        }
        
        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<T, bool> predicate2, Func<T, R> selector2,
            Func<T, bool> predicate3, Func<T, R> selector3,
            Func<T, bool> predicate4, Func<T, R> selector4,
            Func<T, bool> predicate5, Func<T, R> selector5,
            Func<T, bool> predicate6, Func<T, R> selector6,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (predicate1(obj))
            {
                return selector1(obj);
            }
            if (predicate2(obj))
            {
                return selector2(obj);
            }
            if (predicate3(obj))
            {
                return selector3(obj);
            }
            if (predicate4(obj))
            {
                return selector4(obj);
            }
            if (predicate5(obj))
            {
                return selector5(obj);
            }
            return predicate6(obj) ? selector6(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            T case2, Action<T> selector2,
            T case3, Action<T> selector3,
            T case4, Action<T> selector4,
            T case5, Action<T> selector5,
            T case6, Action<T> selector6,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
                return;
            }
            if (obj.SafeEquals(case2))
            {
                selector2(obj);
                return;
            }
            if (obj.SafeEquals(case3))
            {
                selector3(obj);
                return;
            }
            if (obj.SafeEquals(case4))
            {
                selector4(obj);
                return;
            }
            if (obj.SafeEquals(case5))
            {
                selector5(obj);
                return;
            }
            if (obj.SafeEquals(case6))
            {
                selector6(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Func<T, bool> predicate2, Action<T> selector2,
            Func<T, bool> predicate3, Action<T> selector3,
            Func<T, bool> predicate4, Action<T> selector4,
            Func<T, bool> predicate5, Action<T> selector5,
            Func<T, bool> predicate6, Action<T> selector6,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
                return;
            }
            if (predicate2(obj))
            {
                selector2(obj);
                return;
            }
            if (predicate3(obj))
            {
                selector3(obj);
                return;
            }
            if (predicate4(obj))
            {
                selector4(obj);
                return;
            }
            if (predicate5(obj))
            {
                selector5(obj);
                return;
            }
            if (predicate6(obj))
            {
                selector6(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            T case2, Func<T, R> selector2,
            T case3, Func<T, R> selector3,
            T case4, Func<T, R> selector4,
            T case5, Func<T, R> selector5,
            T case6, Func<T, R> selector6,
            T case7, Func<T, R> selector7,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                return selector1(obj);
            }
            if (obj.SafeEquals(case2))
            {
                return selector2(obj);
            }
            if (obj.SafeEquals(case3))
            {
                return selector3(obj);
            }
            if (obj.SafeEquals(case4))
            {
                return selector4(obj);
            }
            if (obj.SafeEquals(case5))
            {
                return selector5(obj);
            }
            if (obj.SafeEquals(case6))
            {
                return selector6(obj);
            }
            return obj.SafeEquals(case7) ? selector7(obj) : Otherwise(otherwise, otherwiseThrow);
        }
        
        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<T, bool> predicate2, Func<T, R> selector2,
            Func<T, bool> predicate3, Func<T, R> selector3,
            Func<T, bool> predicate4, Func<T, R> selector4,
            Func<T, bool> predicate5, Func<T, R> selector5,
            Func<T, bool> predicate6, Func<T, R> selector6,
            Func<T, bool> predicate7, Func<T, R> selector7,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (predicate1(obj))
            {
                return selector1(obj);
            }
            if (predicate2(obj))
            {
                return selector2(obj);
            }
            if (predicate3(obj))
            {
                return selector3(obj);
            }
            if (predicate4(obj))
            {
                return selector4(obj);
            }
            if (predicate5(obj))
            {
                return selector5(obj);
            }
            if (predicate6(obj))
            {
                return selector6(obj);
            }
            return predicate7(obj) ? selector7(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            T case2, Action<T> selector2,
            T case3, Action<T> selector3,
            T case4, Action<T> selector4,
            T case5, Action<T> selector5,
            T case6, Action<T> selector6,
            T case7, Action<T> selector7,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
                return;
            }
            if (obj.SafeEquals(case2))
            {
                selector2(obj);
                return;
            }
            if (obj.SafeEquals(case3))
            {
                selector3(obj);
                return;
            }
            if (obj.SafeEquals(case4))
            {
                selector4(obj);
                return;
            }
            if (obj.SafeEquals(case5))
            {
                selector5(obj);
                return;
            }
            if (obj.SafeEquals(case6))
            {
                selector6(obj);
                return;
            }
            if (obj.SafeEquals(case7))
            {
                selector7(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Func<T, bool> predicate2, Action<T> selector2,
            Func<T, bool> predicate3, Action<T> selector3,
            Func<T, bool> predicate4, Action<T> selector4,
            Func<T, bool> predicate5, Action<T> selector5,
            Func<T, bool> predicate6, Action<T> selector6,
            Func<T, bool> predicate7, Action<T> selector7,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
                return;
            }
            if (predicate2(obj))
            {
                selector2(obj);
                return;
            }
            if (predicate3(obj))
            {
                selector3(obj);
                return;
            }
            if (predicate4(obj))
            {
                selector4(obj);
                return;
            }
            if (predicate5(obj))
            {
                selector5(obj);
                return;
            }
            if (predicate6(obj))
            {
                selector6(obj);
                return;
            }
            if (predicate7(obj))
            {
                selector7(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            T case2, Func<T, R> selector2,
            T case3, Func<T, R> selector3,
            T case4, Func<T, R> selector4,
            T case5, Func<T, R> selector5,
            T case6, Func<T, R> selector6,
            T case7, Func<T, R> selector7,
            T case8, Func<T, R> selector8,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                return selector1(obj);
            }
            if (obj.SafeEquals(case2))
            {
                return selector2(obj);
            }
            if (obj.SafeEquals(case3))
            {
                return selector3(obj);
            }
            if (obj.SafeEquals(case4))
            {
                return selector4(obj);
            }
            if (obj.SafeEquals(case5))
            {
                return selector5(obj);
            }
            if (obj.SafeEquals(case6))
            {
                return selector6(obj);
            }
            if (obj.SafeEquals(case7))
            {
                return selector7(obj);
            }
            return obj.SafeEquals(case8) ? selector8(obj) : Otherwise(otherwise, otherwiseThrow);
        }
        
        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<T, bool> predicate2, Func<T, R> selector2,
            Func<T, bool> predicate3, Func<T, R> selector3,
            Func<T, bool> predicate4, Func<T, R> selector4,
            Func<T, bool> predicate5, Func<T, R> selector5,
            Func<T, bool> predicate6, Func<T, R> selector6,
            Func<T, bool> predicate7, Func<T, R> selector7,
            Func<T, bool> predicate8, Func<T, R> selector8,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (predicate1(obj))
            {
                return selector1(obj);
            }
            if (predicate2(obj))
            {
                return selector2(obj);
            }
            if (predicate3(obj))
            {
                return selector3(obj);
            }
            if (predicate4(obj))
            {
                return selector4(obj);
            }
            if (predicate5(obj))
            {
                return selector5(obj);
            }
            if (predicate6(obj))
            {
                return selector6(obj);
            }
            if (predicate7(obj))
            {
                return selector7(obj);
            }
            return predicate8(obj) ? selector8(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            T case2, Action<T> selector2,
            T case3, Action<T> selector3,
            T case4, Action<T> selector4,
            T case5, Action<T> selector5,
            T case6, Action<T> selector6,
            T case7, Action<T> selector7,
            T case8, Action<T> selector8,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
                return;
            }
            if (obj.SafeEquals(case2))
            {
                selector2(obj);
                return;
            }
            if (obj.SafeEquals(case3))
            {
                selector3(obj);
                return;
            }
            if (obj.SafeEquals(case4))
            {
                selector4(obj);
                return;
            }
            if (obj.SafeEquals(case5))
            {
                selector5(obj);
                return;
            }
            if (obj.SafeEquals(case6))
            {
                selector6(obj);
                return;
            }
            if (obj.SafeEquals(case7))
            {
                selector7(obj);
                return;
            }
            if (obj.SafeEquals(case8))
            {
                selector8(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Func<T, bool> predicate2, Action<T> selector2,
            Func<T, bool> predicate3, Action<T> selector3,
            Func<T, bool> predicate4, Action<T> selector4,
            Func<T, bool> predicate5, Action<T> selector5,
            Func<T, bool> predicate6, Action<T> selector6,
            Func<T, bool> predicate7, Action<T> selector7,
            Func<T, bool> predicate8, Action<T> selector8,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
                return;
            }
            if (predicate2(obj))
            {
                selector2(obj);
                return;
            }
            if (predicate3(obj))
            {
                selector3(obj);
                return;
            }
            if (predicate4(obj))
            {
                selector4(obj);
                return;
            }
            if (predicate5(obj))
            {
                selector5(obj);
                return;
            }
            if (predicate6(obj))
            {
                selector6(obj);
                return;
            }
            if (predicate7(obj))
            {
                selector7(obj);
                return;
            }
            if (predicate8(obj))
            {
                selector8(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            T case2, Func<T, R> selector2,
            T case3, Func<T, R> selector3,
            T case4, Func<T, R> selector4,
            T case5, Func<T, R> selector5,
            T case6, Func<T, R> selector6,
            T case7, Func<T, R> selector7,
            T case8, Func<T, R> selector8,
            T case9, Func<T, R> selector9,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                return selector1(obj);
            }
            if (obj.SafeEquals(case2))
            {
                return selector2(obj);
            }
            if (obj.SafeEquals(case3))
            {
                return selector3(obj);
            }
            if (obj.SafeEquals(case4))
            {
                return selector4(obj);
            }
            if (obj.SafeEquals(case5))
            {
                return selector5(obj);
            }
            if (obj.SafeEquals(case6))
            {
                return selector6(obj);
            }
            if (obj.SafeEquals(case7))
            {
                return selector7(obj);
            }
            if (obj.SafeEquals(case8))
            {
                return selector8(obj);
            }
            return obj.SafeEquals(case9) ? selector9(obj) : Otherwise(otherwise, otherwiseThrow);
        }
        
        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<T, bool> predicate2, Func<T, R> selector2,
            Func<T, bool> predicate3, Func<T, R> selector3,
            Func<T, bool> predicate4, Func<T, R> selector4,
            Func<T, bool> predicate5, Func<T, R> selector5,
            Func<T, bool> predicate6, Func<T, R> selector6,
            Func<T, bool> predicate7, Func<T, R> selector7,
            Func<T, bool> predicate8, Func<T, R> selector8,
            Func<T, bool> predicate9, Func<T, R> selector9,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (predicate1(obj))
            {
                return selector1(obj);
            }
            if (predicate2(obj))
            {
                return selector2(obj);
            }
            if (predicate3(obj))
            {
                return selector3(obj);
            }
            if (predicate4(obj))
            {
                return selector4(obj);
            }
            if (predicate5(obj))
            {
                return selector5(obj);
            }
            if (predicate6(obj))
            {
                return selector6(obj);
            }
            if (predicate7(obj))
            {
                return selector7(obj);
            }
            if (predicate8(obj))
            {
                return selector8(obj);
            }
            return predicate9(obj) ? selector9(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            T case2, Action<T> selector2,
            T case3, Action<T> selector3,
            T case4, Action<T> selector4,
            T case5, Action<T> selector5,
            T case6, Action<T> selector6,
            T case7, Action<T> selector7,
            T case8, Action<T> selector8,
            T case9, Action<T> selector9,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
                return;
            }
            if (obj.SafeEquals(case2))
            {
                selector2(obj);
                return;
            }
            if (obj.SafeEquals(case3))
            {
                selector3(obj);
                return;
            }
            if (obj.SafeEquals(case4))
            {
                selector4(obj);
                return;
            }
            if (obj.SafeEquals(case5))
            {
                selector5(obj);
                return;
            }
            if (obj.SafeEquals(case6))
            {
                selector6(obj);
                return;
            }
            if (obj.SafeEquals(case7))
            {
                selector7(obj);
                return;
            }
            if (obj.SafeEquals(case8))
            {
                selector8(obj);
                return;
            }
            if (obj.SafeEquals(case9))
            {
                selector9(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Func<T, bool> predicate2, Action<T> selector2,
            Func<T, bool> predicate3, Action<T> selector3,
            Func<T, bool> predicate4, Action<T> selector4,
            Func<T, bool> predicate5, Action<T> selector5,
            Func<T, bool> predicate6, Action<T> selector6,
            Func<T, bool> predicate7, Action<T> selector7,
            Func<T, bool> predicate8, Action<T> selector8,
            Func<T, bool> predicate9, Action<T> selector9,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
                return;
            }
            if (predicate2(obj))
            {
                selector2(obj);
                return;
            }
            if (predicate3(obj))
            {
                selector3(obj);
                return;
            }
            if (predicate4(obj))
            {
                selector4(obj);
                return;
            }
            if (predicate5(obj))
            {
                selector5(obj);
                return;
            }
            if (predicate6(obj))
            {
                selector6(obj);
                return;
            }
            if (predicate7(obj))
            {
                selector7(obj);
                return;
            }
            if (predicate8(obj))
            {
                selector8(obj);
                return;
            }
            if (predicate9(obj))
            {
                selector9(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            T case2, Func<T, R> selector2,
            T case3, Func<T, R> selector3,
            T case4, Func<T, R> selector4,
            T case5, Func<T, R> selector5,
            T case6, Func<T, R> selector6,
            T case7, Func<T, R> selector7,
            T case8, Func<T, R> selector8,
            T case9, Func<T, R> selector9,
            T case10, Func<T, R> selector10,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                return selector1(obj);
            }
            if (obj.SafeEquals(case2))
            {
                return selector2(obj);
            }
            if (obj.SafeEquals(case3))
            {
                return selector3(obj);
            }
            if (obj.SafeEquals(case4))
            {
                return selector4(obj);
            }
            if (obj.SafeEquals(case5))
            {
                return selector5(obj);
            }
            if (obj.SafeEquals(case6))
            {
                return selector6(obj);
            }
            if (obj.SafeEquals(case7))
            {
                return selector7(obj);
            }
            if (obj.SafeEquals(case8))
            {
                return selector8(obj);
            }
            if (obj.SafeEquals(case9))
            {
                return selector9(obj);
            }
            return obj.SafeEquals(case10) ? selector10(obj) : Otherwise(otherwise, otherwiseThrow);
        }
        
        public static R Match<T, R>(
            this T obj,
            Func<T, bool> predicate1, Func<T, R> selector1,
            Func<T, bool> predicate2, Func<T, R> selector2,
            Func<T, bool> predicate3, Func<T, R> selector3,
            Func<T, bool> predicate4, Func<T, R> selector4,
            Func<T, bool> predicate5, Func<T, R> selector5,
            Func<T, bool> predicate6, Func<T, R> selector6,
            Func<T, bool> predicate7, Func<T, R> selector7,
            Func<T, bool> predicate8, Func<T, R> selector8,
            Func<T, bool> predicate9, Func<T, R> selector9,
            Func<T, bool> predicate10, Func<T, R> selector10,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            if (predicate1(obj))
            {
                return selector1(obj);
            }
            if (predicate2(obj))
            {
                return selector2(obj);
            }
            if (predicate3(obj))
            {
                return selector3(obj);
            }
            if (predicate4(obj))
            {
                return selector4(obj);
            }
            if (predicate5(obj))
            {
                return selector5(obj);
            }
            if (predicate6(obj))
            {
                return selector6(obj);
            }
            if (predicate7(obj))
            {
                return selector7(obj);
            }
            if (predicate8(obj))
            {
                return selector8(obj);
            }
            if (predicate9(obj))
            {
                return selector9(obj);
            }
            return predicate10(obj) ? selector10(obj) : Otherwise(otherwise, otherwiseThrow);
        }

        public static void Match<T>(
            this T obj,
            T case1, Action<T> selector1,
            T case2, Action<T> selector2,
            T case3, Action<T> selector3,
            T case4, Action<T> selector4,
            T case5, Action<T> selector5,
            T case6, Action<T> selector6,
            T case7, Action<T> selector7,
            T case8, Action<T> selector8,
            T case9, Action<T> selector9,
            T case10, Action<T> selector10,
            Action<Unit> otherwise = null
        )
        {
            if (obj.SafeEquals(case1))
            {
                selector1(obj);
                return;
            }
            if (obj.SafeEquals(case2))
            {
                selector2(obj);
                return;
            }
            if (obj.SafeEquals(case3))
            {
                selector3(obj);
                return;
            }
            if (obj.SafeEquals(case4))
            {
                selector4(obj);
                return;
            }
            if (obj.SafeEquals(case5))
            {
                selector5(obj);
                return;
            }
            if (obj.SafeEquals(case6))
            {
                selector6(obj);
                return;
            }
            if (obj.SafeEquals(case7))
            {
                selector7(obj);
                return;
            }
            if (obj.SafeEquals(case8))
            {
                selector8(obj);
                return;
            }
            if (obj.SafeEquals(case9))
            {
                selector9(obj);
                return;
            }
            if (obj.SafeEquals(case10))
            {
                selector10(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
        
        public static void Match<T>(
            this T obj,
            Func<T, bool> predicate1, Action<T> selector1,
            Func<T, bool> predicate2, Action<T> selector2,
            Func<T, bool> predicate3, Action<T> selector3,
            Func<T, bool> predicate4, Action<T> selector4,
            Func<T, bool> predicate5, Action<T> selector5,
            Func<T, bool> predicate6, Action<T> selector6,
            Func<T, bool> predicate7, Action<T> selector7,
            Func<T, bool> predicate8, Action<T> selector8,
            Func<T, bool> predicate9, Action<T> selector9,
            Func<T, bool> predicate10, Action<T> selector10,
            Action<Unit> otherwise = null
        )
        {
            if (predicate1(obj))
            {
                selector1(obj);
                return;
            }
            if (predicate2(obj))
            {
                selector2(obj);
                return;
            }
            if (predicate3(obj))
            {
                selector3(obj);
                return;
            }
            if (predicate4(obj))
            {
                selector4(obj);
                return;
            }
            if (predicate5(obj))
            {
                selector5(obj);
                return;
            }
            if (predicate6(obj))
            {
                selector6(obj);
                return;
            }
            if (predicate7(obj))
            {
                selector7(obj);
                return;
            }
            if (predicate8(obj))
            {
                selector8(obj);
                return;
            }
            if (predicate9(obj))
            {
                selector9(obj);
                return;
            }
            if (predicate10(obj))
            {
                selector10(obj);
            }
            else
            {
                otherwise.Execute();
            }
        }
    }
}
