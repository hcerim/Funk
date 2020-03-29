using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Funk.Exceptions;

namespace Funk.Internal
{
    internal static class InternalExt
    {
        [Pure] 
        private static UnhandledValueException UnhandledException => new UnhandledValueException("Expression did not cover all possible cases.");

        internal static R Otherwise<R>(Func<Unit, R> otherwise = null, Func<Unit, Exception> otherwiseThrow = null)
        {
            return otherwise.AsMaybe().Match(
                _ =>
                {
                    if (otherwiseThrow.IsNotNull())
                    {
                        throw otherwiseThrow(Unit.Value);
                    }
                    throw UnhandledException;
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

        internal static Exc<T, E> TryCatch<T, E>(this Func<Unit, T> operation) where E : Exception
        {
            try
            {
                return new Exc<T, E>(operation(Unit.Value));
            }
            catch (E e)
            {
                return new Exc<T, E>(e);
            }
        }

        internal static async Task<Exc<T, E>> TryCatch<T, E>(this Func<Unit, Task<T>> operation) where E : Exception
        {
            try
            {
                return new Exc<T, E>(await operation(Unit.Value));
            }
            catch (E e)
            {
                return new Exc<T, E>(e);
            }
        }
    }
}
