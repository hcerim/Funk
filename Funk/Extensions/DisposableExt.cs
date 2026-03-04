using System;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk;

/// <summary>
/// Provides extension methods for safely working with IDisposable objects.
/// </summary>
public static class DisposableExt
{
    extension<D>(D disposable) where D : IDisposable
    {
        /// <summary>
        /// Executes the specified function with the disposable and disposes it afterwards, returning the result.
        /// </summary>
        /// <typeparam name="R">The type of the result.</typeparam>
        /// <param name="operation">The function to execute with the disposable.</param>
        /// <returns>The result of the operation.</returns>
        public R DisposeAfter<R>(Func<D, R> operation)
        {
            using (disposable) return operation(disposable);
        }

        /// <summary>
        /// Executes the specified asynchronous function with the disposable and disposes it afterwards, returning the result.
        /// </summary>
        /// <typeparam name="R">The type of the result.</typeparam>
        /// <param name="operation">The asynchronous function to execute with the disposable.</param>
        /// <returns>A Task containing the result of the operation.</returns>
        public Task<R> DisposeAfterAsync<R>(Func<D, Task<R>> operation)
        {
            return run(func(async () =>
            {
                using (disposable)
                {
                    return await operation(disposable).ConfigureAwait(false);
                }
            }));
        }

        /// <summary>
        /// Executes the specified action with the disposable and disposes it afterwards.
        /// </summary>
        /// <param name="operation">The action to execute with the disposable.</param>
        public void DisposeAfter(Action<D> operation)
        {
            using (disposable) operation(disposable);
        }

        /// <summary>
        /// Executes the specified asynchronous action with the disposable and disposes it afterwards.
        /// </summary>
        /// <param name="operation">The asynchronous action to execute with the disposable.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task DisposeAfterAsync(Func<D, Task> operation)
        {
            return run(act(async () =>
            {
                using (disposable)
                {
                    await operation(disposable).ConfigureAwait(false);
                }
            }));
        }
    }
}