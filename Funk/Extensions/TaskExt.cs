using System;
using System.Threading.Tasks;

namespace Funk
{
    public static class TaskExt
    {
        /// <summary>
        /// Creates a Task of Unit from Task.
        /// </summary>
        public static Task<Unit> WithResult(this Task task)
        {
            return Task.Run(async () =>
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
            return Task.Run(async () =>
            {
                await task;
                return result(Unit.Value);
            });
        }
    }
}
