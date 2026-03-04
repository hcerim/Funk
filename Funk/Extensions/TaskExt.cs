using System;
using System.Threading;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk;

/// <summary>
/// Provides extension methods for Task-based asynchronous operations.
/// </summary>
public static class TaskExt
{
    extension(Task task)
    {
        /// <summary>
        /// Creates a Task of Unit from Task.
        /// </summary>
        /// <returns>A Task containing Unit.</returns>
        public Task<Unit> WithResult()
        {
            return run(async () =>
            {
                await task.ConfigureAwait(false);
                return Unit.Value;
            });
        }

        /// <summary>
        /// Creates a Task with result from Task.
        /// </summary>
        /// <typeparam name="R">The type of the result.</typeparam>
        /// <param name="result">The function to produce the result after the Task completes.</param>
        /// <returns>A Task containing the result.</returns>
        public Task<R> WithResult<R>(Func<Unit, R> result)
        {
            return run(async () =>
            {
                await task.ConfigureAwait(false);
                return result(Unit.Value);
            });
        }
    }

    /// <summary>
    /// Wraps the specified value in a completed Task.
    /// </summary>
    /// <param name="item">The value to wrap.</param>
    /// <returns>A completed Task containing the value.</returns>
    public static Task<T> ToTask<T>(this T item) => result(item);

    extension(Action action)
    {
        /// <summary>
        /// Queues the specified action to run on the thread pool.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task ToTask() => run(action);

        /// <summary>
        /// Queues the specified action to run on the thread pool with cancellation support.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task ToTask(CancellationToken token) => run(action, token);
    }

    extension<T>(Func<T> action)
    {
        /// <summary>
        /// Queues the specified function to run on the thread pool and returns its result.
        /// </summary>
        /// <returns>A Task containing the result of the function.</returns>
        public Task<T> ToTask() => run(action);

        /// <summary>
        /// Queues the specified function to run on the thread pool and returns its result with cancellation support.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A Task containing the result of the function.</returns>
        public Task<T> ToTask(CancellationToken token) => run(action, token);
    }

    extension(Func<Task> action)
    {
        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task ToTask() => run(action);

        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool with cancellation support.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task ToTask(CancellationToken token) => run(action, token);
    }

    extension<T>(Func<Task<T>> action)
    {
        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool and returns its result.
        /// </summary>
        /// <returns>A Task containing the result of the asynchronous function.</returns>
        public Task<T> ToTask() => run(action);

        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool and returns its result with cancellation support.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A Task containing the result of the asynchronous function.</returns>
        public Task<T> ToTask(CancellationToken token) => run(action, token);
    }
}