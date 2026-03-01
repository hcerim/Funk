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
    public static Task<T> ToTask<T>(this T item) => result(item);

    extension(Action action)
    {
        /// <summary>
        /// Queues the specified action to run on the thread pool.
        /// </summary>
        public Task ToTask() => run(action);

        /// <summary>
        /// Queues the specified action to run on the thread pool with cancellation support.
        /// </summary>
        public Task ToTask(CancellationToken token) => run(action, token);
    }

    extension<T>(Func<T> action)
    {
        /// <summary>
        /// Queues the specified function to run on the thread pool and returns its result.
        /// </summary>
        public Task<T> ToTask() => run(action);

        /// <summary>
        /// Queues the specified function to run on the thread pool and returns its result with cancellation support.
        /// </summary>
        public Task<T> ToTask(CancellationToken token) => run(action, token);
    }

    extension(Func<Task> action)
    {
        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool.
        /// </summary>
        public Task ToTask() => run(action);

        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool with cancellation support.
        /// </summary>
        public Task ToTask(CancellationToken token) => run(action, token);
    }

    extension<T>(Func<Task<T>> action)
    {
        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool and returns its result.
        /// </summary>
        public Task<T> ToTask() => run(action);

        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool and returns its result with cancellation support.
        /// </summary>
        public Task<T> ToTask(CancellationToken token) => run(action, token);
    }
}