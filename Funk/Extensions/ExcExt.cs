using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Funk.Internal;
using static Funk.Prelude;

namespace Funk
{
    public static class ExcExt
    {
        /// <summary>
        /// Checks whether Exc is successful and returns Maybe. If it is not, Maybe will be empty.
        /// </summary>
        public static Maybe<T> AsSuccess<T, E>(this Exc<T, E> exceptional) where E : Exception => exceptional.Match(_ => Maybe.Empty<T>(), Maybe.Create, e => Maybe.Empty<T>());

        /// <summary>
        /// Recover in case of the error during creation.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Exc<R, E> OnFailure<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, R> recoverOperation) where T : R where E : Exception
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
        public static async Task<Exc<R, E>> OnFailureAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return await OnFailureAsync(await operationResult, recoverOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Recover in case of the error during creation.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Task<Exc<R, E>> OnFailureAsync<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return operationResult.Match(
                _ => result(Exc.Empty<R, E>()),
                v => result(Exc.Success<R, E>(v)),
                e => Exc.CreateAsync<R, E>(_ => recoverOperation(e))
            );
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Exc<R, E> OnEmpty<T, E, R>(this Exc<T, E> operationResult, Func<Unit, R> recoverOperation) where T : R where E : Exception
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
        public static async Task<Exc<R, E>> OnEmptyAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return await OnEmptyAsync(await operationResult, recoverOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Recover in case of the empty exceptional.
        /// Note that recover does not work if the creation fails because of unhandled exception.
        /// </summary>
        public static Task<Exc<R, E>> OnEmptyAsync<T, E, R>(this Exc<T, E> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception
        {
            return operationResult.Match(
                _ => Exc.CreateAsync<R, E>(__ => recoverOperation(_)),
                v => result(Exc.Success<R, E>(v)),
                e => result(Exc.Failure<R, E>(e))
            );
        }

        /// <summary>
        /// Structure-preserving map.
        /// Continuation on successful result.
        /// Maps successful Exc to the new Exc specified by the selector. Otherwise returns failed Exc.
        /// Use FlatMap if you have nested Exc. 
        /// </summary>
        public static async Task<Exc<R, E>> MapAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<T, Task<R>> continueOperation) where E : Exception
        {
            return await (await operationResult).MapAsync(continueOperation).ConfigureAwait(false);
        }

        /// <summary>
        /// Structure-preserving map.
        /// Continuation on successful result.
        /// Maps successful Exc to the new Exc specified by the selector. Otherwise returns failed Exc.
        /// Use FlatMap if you have nested Exc. 
        /// </summary>
        public static async Task<Exc<R, E>> FlatMapAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<T, Task<Exc<R, E>>> continueOperation) where E : Exception
        {
            return await (await operationResult).FlatMapAsync(continueOperation).ConfigureAwait(false);
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
                return Exc.Failure<IImmutableList<T>, E>(split2.Item1.FlatMap(i => i.NestedFailures.GetOrEmpty()).ToEnumerableException(errorMessage));
            }
            return Exc.Empty<IImmutableList<T>, E>();
        }
    }
}
