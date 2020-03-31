using System;
using System.Threading.Tasks;
using Funk.Exceptions;

namespace Funk
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
                _ => Exc.Empty<R, E>(),
                v => Exc.Success<R, E>(_ => v),
                e => Exc.Create<R, E>(_ => recoverOperation(e))
            );
        }

        /// <summary>
        /// Recover in case of the error during creation.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnFailure<T, E, R>(this Task<Exc<T, E>> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return await RecoverOnFailure(await operationResult, recoverOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Recover in case of the error during creation.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnFailure<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception
        {
            if (operationResult.IsEmpty)
            {
                return Exc.Empty<R, E>();
            }
            return operationResult.IsSuccess ? Exc.Success<R, E>(_ => operationResult.UnsafeGetFirst()) : await Exc.Create<R, E>(_ => recoverOperation(operationResult.UnsafeGetSecond())).ConfigureAwait(false);
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Exc<R, E> RecoverOnEmpty<T, E, R>(this Exc<T, E> operationResult, Func<Unit, R> recoverOperation) where T : R where E : Exception
        {
            return operationResult.Match(
                _ => Exc.Create<R, E>(__ => recoverOperation(Unit.Value)),
                v => Exc.Success<R, E>(_ => v),
                e => Exc.Failure<R, E>(_ => e)
            );
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnEmpty<T, E, R>(this Task<Exc<T, E>> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return await RecoverOnEmpty(await operationResult, recoverOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnEmpty<T, E, R>(this Exc<T, E> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception
        {
            if (operationResult.IsEmpty)
            {
                return await Exc.Create<R, E>(_ => recoverOperation(Unit.Value)).ConfigureAwait(false);
            }
            return operationResult.IsSuccess ? Exc.Success<R, E>(_ => operationResult.UnsafeGetFirst()) : Exc.Failure<R, E>(_ => operationResult.UnsafeGetSecond());
        }

        /// <summary>
        /// Continue if previous operation was successful.
        /// Note that continue does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Exc<R, E> ContinueOnSuccess<T, E, R>(this Exc<T, E> operationResult, Func<T, R> continueOperation) where T : R where E : Exception
        {
            return operationResult.Map(continueOperation);
        }

        /// <summary>
        /// Continue if previous operation was successful.
        /// Note that continue does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> ContinueOnSuccess<T, E, R>(this Task<Exc<T, E>> operationResult, Func<T, Task<R>> continueOperation) where T : R where E : Exception
        {
            return await ContinueOnSuccess(await operationResult, continueOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Continue if previous operation was successful.
        /// Note that continue does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> ContinueOnSuccess<T, E, R>(this Exc<T, E> operationResult, Func<T, Task<R>> continueOperation) where T : R where E : Exception
        {
            return await operationResult.Map(continueOperation).ConfigureAwait(false);
        }
    }
}
