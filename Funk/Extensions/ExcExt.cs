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
        /// Maps Exc Error type to the new type specified by the selector.
        /// </summary>
        public static Exc<T, E2> MapFailure<T, E1, E2>(this Exc<T, E1> exceptional, Func<EnumerableException<E1>, E2> selector) where E1 : Exception where E2 : Exception => exceptional.Match(_ => Exc.Empty<T, E2>(), success<T, E2>, f => failure<T, E2>(selector(f)));

        /// <summary>
        /// Maps Exc Error type to the new type specified by the selector.
        /// </summary>
        public static async Task<Exc<T, E2>> MapFailureAsync<T, E1, E2>(this Exc<T, E1> exceptional, Func<EnumerableException<E1>, Task<E2>> selector) where E1 : Exception where E2 : Exception => await exceptional.Match(_ => result(Exc.Empty<T, E2>()), s => result(success<T, E2>(s)), async f => failure<T, E2>(await selector(f)));

        /// <summary>
        /// Maps Exc Error type to the new type specified by the selector.
        /// </summary>
        public static async Task<Exc<T, E2>> MapFailureAsync<T, E1, E2>(this Task<Exc<T, E1>> exceptional, Func<EnumerableException<E1>, Task<E2>> selector) where E1 : Exception where E2 : Exception => await (await exceptional).MapFailureAsync(selector);

        /// <summary>
        /// Maps Exc Error type to the new type specified by the selector.
        /// </summary>
        public static Task<Exc<T, E2>> MapFailureAsync<T, E1, E2>(this Task<Exc<T, E1>> exceptional, Func<EnumerableException<E1>, E2> selector) where E1 : Exception where E2 : Exception => exceptional.MapFailureAsync(e => result(selector(e)));

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
            return item.ToImmutableList().SafeConcat(items).ConditionalSplit(e => e.IsSuccess).Match(
                (success, other) =>
                {
                    return other.AsNotEmptyList().Match(
                        _ => Exc.Success<IImmutableList<T>, E>(success.Map(i => i.Success).Flatten()),
                        o => o.ConditionalSplit(i => i.IsFailure).Match(
                            (failure, _) => failure.AsNotEmptyList().Match(
                                __ => empty,
                                f => Exc.Failure<IImmutableList<T>, E>(f.FlatMap(i => i.NestedFailures.GetOrEmpty()).ToEnumerableException(errorMessage))
                            )
                        )
                    );
                }
            );
        }
    }
}
