using System;
using System.Threading.Tasks;
using Funk.Exceptions;
using static Funk.Prelude;

namespace Funk.Extensions
{
    public static class ExcExt
    {
        /// <summary>
        /// Recover in case of the error during creation.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Exc<R, E> RecoverOnFailure<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, R> recoverOperation) where T : R where E : Exception
        {
            return operationResult.Match(
                _ => empty,
                v => new Exc<R, E>(v),
                e => Exc.Create<R, E>(_ => recoverOperation(e))
            );
        }

        /// <summary>
        /// Recover in case of the error during creation.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnFailure<T, E, R>(this Task<Exc<T, E>> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return await RecoverOnFailure(await operationResult, recoverOperation);
        }

        /// <summary>
        /// Recover in case of the error during creation.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnFailure<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception
        {
            if (operationResult.IsEmpty)
            {
                return empty;
            }
            return operationResult.IsSuccess ? new Exc<R, E>(operationResult.UnsafeGetFirst()) : await Exc.Create<R, E>(_ => recoverOperation(operationResult.UnsafeGetSecond()));
        }
    }
}
