﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Funk.Internal;
using static Funk.Internal.InternalExt;
using static Funk.Prelude;

namespace Funk
{
    public static class ObjectExt
    {
        public static Task<T> ToTask<T>(this T item) => result(item);

        public static async Task InvokeAsync(this Action action) => await run(action).ConfigureAwait(false);

        public static async Task InvokeAsync(this Action action, CancellationToken token) => await run(action, token).ConfigureAwait(false);

        public static async Task<T> InvokeAsync<T>(this Func<T> action) => await run(action).ConfigureAwait(false);

        public static async Task<T> InvokeAsync<T>(this Func<T> action, CancellationToken token) => await run(action, token).ConfigureAwait(false);

        public static async Task InvokeAsync(this Func<Task> action) => await run(action).ConfigureAwait(false);

        public static async Task InvokeAsync(this Func<Task> action, CancellationToken token) => await run(action, token).ConfigureAwait(false);

        public static async Task<T> InvokeAsync<T>(this Func<Task<T>> action) => await run(action).ConfigureAwait(false);

        public static async Task<T> InvokeAsync<T>(this Func<Task<T>> action, CancellationToken token) => await run(action, token).ConfigureAwait(false);

        public static Maybe<R> SafeCast<R>(this object item)
        {
            if (item is R valid)
            {
                return Maybe.Create(valid);
            }
            return Maybe.Empty<R>();
        }

        public static R Match<T, R>(
            this T obj,
            T case1, Func<T, R> selector1,
            Func<Unit, R> otherwise = null,
            Func<Unit, Exception> otherwiseThrow = null
        )
        {
            return obj.SafeEquals(case1) ? selector1(obj) : Otherwise(otherwise, otherwiseThrow);
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
    }
}
