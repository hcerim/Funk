using System;
using System.Threading.Tasks;
using Funk.Exceptions;
using static Funk.Prelude;

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

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Exc<R, E> RecoverOnEmpty<T, E, R>(this Exc<T, E> operationResult, Func<Unit, R> recoverOperation) where T : R where E : Exception
        {
            return operationResult.Match(
                _ => Exc.Create<R, E>(__ => recoverOperation(Unit.Value)),
                v => new Exc<R, E>(v),
                e => new Exc<R, E>(e)
            );
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnEmpty<T, E, R>(this Task<Exc<T, E>> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return await RecoverOnEmpty(await operationResult, recoverOperation);
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnEmpty<T, E, R>(this Exc<T, E> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception
        {
            if (operationResult.IsEmpty)
            {
                return await Exc.Create<R, E>(_ => recoverOperation(Unit.Value));
            }
            return operationResult.IsSuccess ? new Exc<R, E>(operationResult.UnsafeGetFirst()) : new Exc<R, E>(operationResult.UnsafeGetSecond());
        }

        /// <summary>
        /// Continue if previous operation was successful.
        /// Note that continue does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Exc<R, E> ContinueOnSuccess<T, E, R>(this Exc<T, E> operationResult, Func<T, R> continueOperation) where T : R where E : Exception
        {
            return operationResult.Match(
                _ => empty,
                v => Exc.Create<R, E>(_ => continueOperation(v)),
                e => new Exc<R, E>(e)
            );
        }

        /// <summary>
        /// Continue if previous operation was successful.
        /// Note that continue does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> ContinueOnSuccess<T, E, R>(this Task<Exc<T, E>> operationResult, Func<T, Task<R>> continueOperation) where T : R where E : Exception
        {
            return await ContinueOnSuccess(await operationResult, continueOperation);
        }

        /// <summary>
        /// Continue if previous operation was successful.
        /// Note that continue does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> ContinueOnSuccess<T, E, R>(this Exc<T, E> operationResult, Func<T, Task<R>> continueOperation) where T : R where E : Exception
        {
            if (operationResult.IsEmpty)
            {
                return empty;
            }
            return operationResult.IsSuccess ? await Exc.Create<R, E>(_ => continueOperation(operationResult.UnsafeGetFirst())) : new Exc<R, E>(operationResult.UnsafeGetSecond());
        }
    }
}
