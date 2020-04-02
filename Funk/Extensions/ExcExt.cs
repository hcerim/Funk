using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                v => Exc.Success<R, E>(v),
                e => Exc.Create<R, E>(_ => recoverOperation(e))
            );
        }

        /// <summary>
        /// Recover in case of the error during creation.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnFailureAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return await RecoverOnFailureAsync(await operationResult, recoverOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Recover in case of the error during creation.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnFailureAsync<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception
        {
            if (operationResult.IsEmpty)
            {
                return Exc.Empty<R, E>();
            }
            return operationResult.IsSuccess ? Exc.Success<R, E>(operationResult.UnsafeGetFirst()) : await Exc.CreateAsync<R, E>(_ => recoverOperation(operationResult.UnsafeGetSecond())).ConfigureAwait(false);
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Exc<R, E> RecoverOnEmpty<T, E, R>(this Exc<T, E> operationResult, Func<Unit, R> recoverOperation) where T : R where E : Exception
        {
            return operationResult.Match(
                _ => Exc.Create<R, E>(__ => recoverOperation(Unit.Value)),
                v => Exc.Success<R, E>(v),
                Exc.Failure<R, E>
            );
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnEmptyAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return await RecoverOnEmptyAsync(await operationResult, recoverOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> RecoverOnEmptyAsync<T, E, R>(this Exc<T, E> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception
        {
            if (operationResult.IsEmpty)
            {
                return await Exc.CreateAsync<R, E>(_ => recoverOperation(Unit.Value)).ConfigureAwait(false);
            }
            return operationResult.IsSuccess ? Exc.Success<R, E>(operationResult.UnsafeGetFirst()) : Exc.Failure<R, E>(operationResult.UnsafeGetSecond());
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
        public static async Task<Exc<R, E>> ContinueOnSuccessAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<T, Task<R>> continueOperation) where T : R where E : Exception
        {
            return await ContinueOnSuccessAsync(await operationResult, continueOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Continue if previous operation was successful.
        /// Note that continue does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static async Task<Exc<R, E>> ContinueOnSuccessAsync<T, E, R>(this Exc<T, E> operationResult, Func<T, Task<R>> continueOperation) where T : R where E : Exception
        {
            return await operationResult.MapAsync(continueOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Aggregates Exc with another Exc. If both are success the result will be Success of Collection of results.
        /// If there is any non-successful, Exc will be failure if any failures or will be empty if all are empty.
        /// </summary>
        public static Exc<IImmutableList<T>, E> Merge<T, E>(this Exc<T, E> first, Exc<T, E> second, string errorMessage = null) where E : Exception
        {
            return MergeRange(first, second.ToImmutableList(), errorMessage);
        }

        /// <summary>
        /// Aggregates Exc with collection of Exc. If all are success the result will be Success of Collection of results.
        /// If there is any non-successful, Exc will be failure if any failures or will be empty if all are empty.
        /// </summary>
        public static Exc<IImmutableList<T>, E> MergeRange<T, E>(this Exc<T, E> item, IEnumerable<Exc<T, E>> items, string errorMessage = null) where E : Exception
        {
            var list = item.ToImmutableList().SafeConcat(items);
            var split1 = list.ConditionalSplit(e => e.IsSuccess);
            if (split1.Item2.IsEmpty())
            {
                return Exc.Success<IImmutableList<T>, E>(split1.Item1.Map(i => i.Success.UnsafeGet()));
            }
            var split2 = split1.Item2.ConditionalSplit(i => i.IsFailure);
            if (split2.Item1.NotEmpty())
            {
                return Exc.Failure<IImmutableList<T>, E>(split2.Item1.FlatMap(i => i.NestedFailures.GetOr(__ => ImmutableList<E>.Empty.Map())).ToEnumerableException(errorMessage));
            }
            return Exc.Empty<IImmutableList<T>, E>();
        }
    }
}
