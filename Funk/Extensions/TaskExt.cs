using System;
using System.Threading.Tasks;

namespace Funk
{
    public static class TaskExt
    {
        /// <summary>
        /// Creates a Task of Unit from Task.
        /// </summary>
        public static async Task<Unit> WithResult(this Task task)
        {
            return await Task.Run(async () =>
            {
                await task.ConfigureAwait(false);
                return Unit.Value;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a Task with result from Task.
        /// </summary>
        public static async Task<R> WithResult<R>(this Task task, Func<Unit, R> result)
        {
            return await Task.Run(async () =>
            {
                await task.ConfigureAwait(false);
                return result(Unit.Value);
            }).ConfigureAwait(false);
        }
    }
}
