using System;
using System.Diagnostics.Contracts;
using Funk.Exceptions;

namespace Funk.Internal
{
    internal static class InternalExt
    {
        [Pure] 
        private static UnhandledValueException UnhandledException => new UnhandledValueException("Expression did not cover all possible input values.");

        internal static R Otherwise<R>(Func<Unit, R> otherwise = null, Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwise.AsMaybe().Match(
                _ =>
                {
                    otherwiseThrow.Throw(__ => UnhandledException);
                    return default;
                },
                o => o(Unit.Value)
            );
        }

        internal static void Execute(this Action<Unit> otherwise)
        {
            otherwise.AsMaybe().Match(
                _ => throw UnhandledException,
                e => e(Unit.Value)
            );
        }

        private static void Throw(this Func<Unit, Exception> otherwiseThrow, Func<Unit, FunkException> exception)
        {
            otherwiseThrow.AsMaybe().Match(
                _ => throw exception(_),
                e => throw e(Unit.Value)
            );
        }
    }
}
