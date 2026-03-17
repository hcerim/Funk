using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Funk.Internal;
using static Funk.Prelude;

namespace Funk;

/// <summary>
/// Provides extension methods for the Exc type including recovery, mapping, and merging operations.
/// </summary>
public static class ExcExt
{
    /// <summary>
    /// Checks whether Exc is successful and returns Maybe. If it is not, Maybe will be empty.
    /// </summary>
    /// <typeparam name="T">The success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <returns>A Maybe containing the success value, or an empty Maybe.</returns>
    public static Maybe<T> AsSuccess<T, E>(this Exc<T, E> exceptional) where E : Exception => exceptional.Match(_ => Maybe.Empty<T>(), Maybe.Create, e => Maybe.Empty<T>());

    /// <summary>
    /// Flattens the nested exceptional into a single exceptional.
    /// </summary>
    /// <typeparam name="T">The success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <returns>The flattened Exc.</returns>
    public static Exc<T, E> Flatten<T, E>(this Exc<Exc<T, E>, E> exceptional) where E : Exception => exceptional.FlatMap(v => v);

    extension<T, E1>(Exc<T, E1> exceptional) where E1 : Exception
    {
        /// <summary>
        /// Maps Exc Error type to the new type specified by the selector.
        /// </summary>
        /// <typeparam name="E2">The new exception type.</typeparam>
        /// <param name="selector">The mapping function for the failure.</param>
        /// <returns>A new Exc with the mapped failure type.</returns>
        public Exc<T, E2> MapFailure<E2>(Func<EnumerableException<E1>, E2> selector) where E2 : Exception
        {
            return exceptional.Match(
                _ => Exc.Empty<T, E2>(),
                success<T, E2>,
                f => failure<T, E2>(selector(f))
            );
        }

        /// <summary>
        /// Maps Exc Error type to the new type specified by the selector.
        /// </summary>
        /// <typeparam name="E2">The new exception type.</typeparam>
        /// <param name="selector">The async mapping function for the failure.</param>
        /// <returns>A Task of a new Exc with the mapped failure type.</returns>
        public async Task<Exc<T, E2>> MapFailureAsync<E2>(Func<EnumerableException<E1>, Task<E2>> selector) where E2 : Exception
        {
            return await exceptional.Match(
                _ => result(Exc.Empty<T, E2>()),
                s => result(success<T, E2>(s)),
                async f => failure<T, E2>(await selector(f).ConfigureAwait(false))
            ).ConfigureAwait(false);
        }
    }

    extension<T, E1>(Task<Exc<T, E1>> exceptional) where E1 : Exception
    {
        /// <summary>
        /// Maps Exc Error type to the new type specified by the selector.
        /// </summary>
        /// <typeparam name="E2">The new exception type.</typeparam>
        /// <param name="selector">The async mapping function for the failure.</param>
        /// <returns>A Task of a new Exc with the mapped failure type.</returns>
        public async Task<Exc<T, E2>> MapFailureAsync<E2>(Func<EnumerableException<E1>, Task<E2>> selector) where E2 : Exception => await (await exceptional.ConfigureAwait(false)).MapFailureAsync(selector).ConfigureAwait(false);

        /// <summary>
        /// Maps Exc Error type to the new type specified by the selector.
        /// </summary>
        /// <typeparam name="E2">The new exception type.</typeparam>
        /// <param name="selector">The mapping function for the failure.</param>
        /// <returns>A Task of a new Exc with the mapped failure type.</returns>
        public Task<Exc<T, E2>> MapFailureAsync<E2>(Func<EnumerableException<E1>, E2> selector) where E2 : Exception => exceptional.MapFailureAsync(e => result(selector(e)));
    }

    /// <summary>
    /// Recover in case of the error during creation.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Exc to recover from.</param>
    /// <param name="recoverOperation">The recovery function.</param>
    /// <returns>The recovered Exc or the original success.</returns>
    public static Exc<R, E> OnFailure<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, R> recoverOperation) where T : R where E : Exception => operationResult.OnFlatFailure(e => Exc.Create<R, E>(_ => recoverOperation(e)));

    /// <summary>
    /// Recover in case of the error during creation.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Exc to recover from.</param>
    /// <param name="recoverOperation">The recovery function returning an Exc.</param>
    /// <returns>The recovered Exc or the original success.</returns>
    public static Exc<R, E> OnFlatFailure<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, Exc<R, E>> recoverOperation) where T : R where E : Exception
    {
        return operationResult.Match(
            _ => Exc.Empty<R, E>(),
            v => Exc.Success<R, E>(v),
            recoverOperation
        );
    }

    /// <summary>
    /// Recover in case of the error during creation.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Task of Exc to recover from.</param>
    /// <param name="recoverOperation">The async recovery function.</param>
    /// <returns>A Task of the recovered Exc or the original success.</returns>
    public static async Task<Exc<R, E>> OnFailureAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception => await OnFailureAsync(await operationResult.ConfigureAwait(false), recoverOperation).ConfigureAwait(false);

    /// <summary>
    /// Recover in case of the error during creation.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Task of Exc to recover from.</param>
    /// <param name="recoverOperation">The async recovery function returning an Exc.</param>
    /// <returns>A Task of the recovered Exc or the original success.</returns>
    public static async Task<Exc<R, E>> OnFlatFailureAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<EnumerableException<E>, Task<Exc<R, E>>> recoverOperation) where T : R where E : Exception => await (await operationResult.ConfigureAwait(false)).OnFlatFailureAsync(recoverOperation).ConfigureAwait(false);

    /// <summary>
    /// Recover in case of the error during creation.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Exc to recover from.</param>
    /// <param name="recoverOperation">The async recovery function.</param>
    /// <returns>A Task of the recovered Exc or the original success.</returns>
    public static Task<Exc<R, E>> OnFailureAsync<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, Task<R>> recoverOperation) where T : R where E : Exception => operationResult.OnFlatFailureAsync(e => Exc.CreateAsync<R, E>(_ => recoverOperation(e)));

    /// <summary>
    /// Recover in case of the error during creation.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Exc to recover from.</param>
    /// <param name="recoverOperation">The async recovery function returning an Exc.</param>
    /// <returns>A Task of the recovered Exc or the original success.</returns>
    public static Task<Exc<R, E>> OnFlatFailureAsync<T, E, R>(this Exc<T, E> operationResult, Func<EnumerableException<E>, Task<Exc<R, E>>> recoverOperation) where T : R where E : Exception
    {
        return operationResult.Match(
            _ => result(Exc.Empty<R, E>()),
            v => result(Exc.Success<R, E>(v)),
            recoverOperation
        );
    }

    /// <summary>
    /// Recover in case of the empty exceptional.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Exc to recover from.</param>
    /// <param name="recoverOperation">The recovery function.</param>
    /// <returns>The recovered Exc or the original success or failure.</returns>
    public static Exc<R, E> OnEmpty<T, E, R>(this Exc<T, E> operationResult, Func<Unit, R> recoverOperation) where T : R where E : Exception => operationResult.OnFlatEmpty(_ => Exc.Create<R, E>(recoverOperation));

    /// <summary>
    /// Recover in case of the empty exceptional.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Exc to recover from.</param>
    /// <param name="recoverOperation">The recovery function returning an Exc.</param>
    /// <returns>The recovered Exc or the original success or failure.</returns>
    public static Exc<R, E> OnFlatEmpty<T, E, R>(this Exc<T, E> operationResult, Func<Unit, Exc<R, E>> recoverOperation) where T : R where E : Exception
    {
        return operationResult.Match(
            recoverOperation,
            v => Exc.Success<R, E>(v),
            Exc.Failure<R, E>
        );
    }

    /// <summary>
    /// Recover in case of the empty exceptional.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Task of Exc to recover from.</param>
    /// <param name="recoverOperation">The async recovery function.</param>
    /// <returns>A Task of the recovered Exc or the original success or failure.</returns>
    public static async Task<Exc<R, E>> OnEmptyAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception => await (await operationResult.ConfigureAwait(false)).OnEmptyAsync(recoverOperation).ConfigureAwait(false);

    /// <summary>
    /// Recover in case of the empty exceptional.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Task of Exc to recover from.</param>
    /// <param name="recoverOperation">The async recovery function returning an Exc.</param>
    /// <returns>A Task of the recovered Exc or the original success or failure.</returns>
    public static async Task<Exc<R, E>> OnFlatEmptyAsync<T, E, R>(this Task<Exc<T, E>> operationResult, Func<Unit, Task<Exc<R, E>>> recoverOperation) where T : R where E : Exception => await (await operationResult.ConfigureAwait(false)).OnFlatEmptyAsync(recoverOperation).ConfigureAwait(false);

    /// <summary>
    /// Recover in case of the empty exceptional.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Exc to recover from.</param>
    /// <param name="recoverOperation">The async recovery function.</param>
    /// <returns>A Task of the recovered Exc or the original success or failure.</returns>
    public static Task<Exc<R, E>> OnEmptyAsync<T, E, R>(this Exc<T, E> operationResult, Func<Unit, Task<R>> recoverOperation) where T : R where E : Exception => operationResult.OnFlatEmptyAsync(_ => Exc.CreateAsync<R, E>(recoverOperation));

    /// <summary>
    /// Recover in case of the empty exceptional.
    /// Note that recover does not work if the creation fails because of unhandled exception.
    /// </summary>
    /// <typeparam name="T">The original success type.</typeparam>
    /// <typeparam name="E">The exception type.</typeparam>
    /// <typeparam name="R">The recovered success type.</typeparam>
    /// <param name="operationResult">The Exc to recover from.</param>
    /// <param name="recoverOperation">The async recovery function returning an Exc.</param>
    /// <returns>A Task of the recovered Exc or the original success or failure.</returns>
    public static Task<Exc<R, E>> OnFlatEmptyAsync<T, E, R>(this Exc<T, E> operationResult, Func<Unit, Task<Exc<R, E>>> recoverOperation) where T : R where E : Exception
    {
        return operationResult.Match(
            recoverOperation,
            v => result(Exc.Success<R, E>(v)),
            e => result(Exc.Failure<R, E>(e))
        );
    }

    extension<T, E>(Task<Exc<T, E>> operationResult) where E : Exception
    {
        /// <summary>
        /// Structure-preserving map.
        /// Continuation on successful result.
        /// Maps successful Exc to the new Exc specified by the selector. Otherwise returns failed Exc.
        /// Use FlatMap if you have nested Exc. 
        /// </summary>
        /// <typeparam name="R">The mapped success type.</typeparam>
        /// <param name="continueOperation">The async mapping function.</param>
        /// <returns>A Task of a new Exc with the mapped value or the original failure.</returns>
        public async Task<Exc<R, E>> MapAsync<R>(Func<T, Task<R>> continueOperation) => await (await operationResult.ConfigureAwait(false)).MapAsync(continueOperation).ConfigureAwait(false);

        /// <summary>
        /// Structure-preserving map.
        /// Continuation on successful result.
        /// Maps successful Exc to the new Exc specified by the selector. Otherwise returns failed Exc.
        /// Use FlatMap if you have nested Exc. 
        /// </summary>
        /// <typeparam name="R">The mapped success type.</typeparam>
        /// <param name="continueOperation">The async mapping function returning an Exc.</param>
        /// <returns>A Task of a new Exc with the mapped value or the original failure.</returns>
        public async Task<Exc<R, E>> FlatMapAsync<R>(Func<T, Task<Exc<R, E>>> continueOperation) => await (await operationResult.ConfigureAwait(false)).FlatMapAsync(continueOperation).ConfigureAwait(false);
    }

    extension<T, E>(Exc<T, E> first) where E : Exception
    {
        /// <summary>
        /// Aggregates Exc with another Exc. If both are success the result will be Success of Collection of results.
        /// If there is any non-successful, Exc will be failure if any failures or will be empty.
        /// </summary>
        /// <param name="second">The other Exc to merge with.</param>
        /// <param name="errorMessage">Optional error message for the merged failure.</param>
        /// <returns>An Exc containing a list of results or a merged failure.</returns>
        public Exc<IImmutableList<T>, E> Merge(Exc<T, E> second, string errorMessage = null) => first.MergeRange(second.ToImmutableList(), errorMessage);

        /// <summary>
        /// Aggregates Exc with the sequence of Exc. If all are success the result will be Success of Collection of results.
        /// If there is any non-successful, Exc will be failure if any failures or will be empty.
        /// </summary>
        /// <param name="items">The sequence of Exc values to merge with.</param>
        /// <param name="errorMessage">Optional error message for the merged failure.</param>
        /// <returns>An Exc containing a list of results or a merged failure.</returns>
        public Exc<IImmutableList<T>, E> MergeRange(IEnumerable<Exc<T, E>> items, string errorMessage = null)
        {
            return first.ToImmutableList().Concat(items).ConditionalSplit(e => e.IsSuccess).Match(
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