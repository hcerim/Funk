using System;
using System.Threading;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Provides extension methods for Task-based asynchronous operations.
    /// </summary>
    public static class TaskExt
    {
        /// <summary>
        /// Creates a Task of Unit from Task.
        /// </summary>
        public static Task<Unit> WithResult(this Task task)
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
        public static Task<R> WithResult<R>(this Task task, Func<Unit, R> result)
        {
            return run(async () =>
            {
                await task.ConfigureAwait(false);
                return result(Unit.Value);
            });
        }

        /// <summary>
        /// Wraps the specified value in a completed Task.
        /// </summary>
        public static Task<T> ToTask<T>(this T item) => result(item);

        /// <summary>
        /// Queues the specified action to run on the thread pool.
        /// </summary>
        public static Task ToTask(this Action action) => run(action);

        /// <summary>
        /// Queues the specified action to run on the thread pool with cancellation support.
        /// </summary>
        public static Task ToTask(this Action action, CancellationToken token) => run(action, token);

        /// <summary>
        /// Queues the specified function to run on the thread pool and returns its result.
        /// </summary>
        public static Task<T> ToTask<T>(this Func<T> action) => run(action);

        /// <summary>
        /// Queues the specified function to run on the thread pool and returns its result with cancellation support.
        /// </summary>
        public static Task<T> ToTask<T>(this Func<T> action, CancellationToken token) => run(action, token);

        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool.
        /// </summary>
        public static Task ToTask(this Func<Task> action) => run(action);

        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool with cancellation support.
        /// </summary>
        public static Task ToTask(this Func<Task> action, CancellationToken token) => run(action, token);

        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool and returns its result.
        /// </summary>
        public static Task<T> ToTask<T>(this Func<Task<T>> action) => run(action);

        /// <summary>
        /// Queues the specified asynchronous function to run on the thread pool and returns its result with cancellation support.
        /// </summary>
        public static Task<T> ToTask<T>(this Func<Task<T>> action, CancellationToken token) => run(action, token);
    }
}
