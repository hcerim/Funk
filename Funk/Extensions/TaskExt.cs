using System;
using System.Threading;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    public static class TaskExt
    {
        /// <summary>
        /// Creates a Task of Unit from Task.
        /// </summary>
        public static Task<Unit> WithResult(this Task task)
        {
            return run(async () =>
            {
                await task;
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
                await task;
                return result(Unit.Value);
            });
        }

        public static Task<T> ToTask<T>(this T item) => result(item);

        public static Task InvokeAsync(this Action action) => run(action);

        public static Task InvokeAsync(this Action action, CancellationToken token) => run(action, token);

        public static Task<T> InvokeAsync<T>(this Func<T> action) => run(action);

        public static Task<T> InvokeAsync<T>(this Func<T> action, CancellationToken token) => run(action, token);

        public static Task InvokeAsync(this Func<Task> action) => run(action);

        public static Task InvokeAsync(this Func<Task> action, CancellationToken token) => run(action, token);

        public static Task<T> InvokeAsync<T>(this Func<Task<T>> action) => run(action);

        public static Task<T> InvokeAsync<T>(this Func<Task<T>> action, CancellationToken token) => run(action, token);
    }
}
