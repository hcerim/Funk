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
        /// Executes the specified function with the disposable and disposes it afterward, returning the result.
        /// </summary>
        /// <typeparam name="R">The type of the result.</typeparam>
        /// <param name="operation">The function to execute with the disposable.</param>
        /// <returns>The result of the operation.</returns>
        public R DisposeAfter<R>(Func<D, R> operation)
        {
            using (disposable) return operation(disposable);
        }

        /// <summary>
        /// Executes the specified asynchronous function with the disposable and disposes it afterward, returning the result.
        /// </summary>
        /// <typeparam name="R">The type of the result.</typeparam>
        /// <param name="operation">The asynchronous function to execute with the disposable.</param>
        /// <returns>A Task containing the result of the operation.</returns>
        public Task<R> DisposeAfterAsync<R>(Func<D, Task<R>> operation) =>
            run(func(async () =>
            {
                using (disposable) return await operation(disposable).ConfigureAwait(false);
            }));

        /// <summary>
        /// Executes the specified action with the disposable and disposes it afterward.
        /// </summary>
        /// <param name="operation">The action to execute with the disposable.</param>
        public void DisposeAfter(Action<D> operation)
        {
            using (disposable) operation(disposable);
        }

        /// <summary>
        /// Executes the specified asynchronous action with the disposable and disposes it afterward.
        /// </summary>
        /// <param name="operation">The asynchronous action to execute with the disposable.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task DisposeAfterAsync(Func<D, Task> operation) =>
            run(func(async () =>
            {
                using (disposable) await operation(disposable).ConfigureAwait(false);
            }));
    }
}

#if !NETSTANDARD2_0
/// <summary>
/// Provides extension methods for safely working with IAsyncDisposable objects.
/// </summary>
public static class AsyncDisposableExt
{
    extension<D>(D disposable) where D : IAsyncDisposable
    {
        /// <summary>
        /// Executes the specified function with the disposable and disposes it afterward, returning the result.
        /// </summary>
        /// <typeparam name="R">The type of the result.</typeparam>
        /// <param name="operation">The function to execute with the disposable.</param>
        /// <returns>The result of the operation.</returns>
        public async Task<R> AsyncDisposeAfter<R>(Func<D, R> operation)
        {
            await using (disposable.ConfigureAwait(false)) return operation(disposable);
        }

        /// <summary>
        /// Executes the specified asynchronous function with the disposable and disposes it afterward, returning the result.
        /// </summary>
        /// <typeparam name="R">The type of the result.</typeparam>
        /// <param name="operation">The asynchronous function to execute with the disposable.</param>
        /// <returns>A Task containing the result of the operation.</returns>
        public Task<R> AsyncDisposeAfterAsync<R>(Func<D, Task<R>> operation) =>
            run(func(async () =>
            {
                await using (disposable.ConfigureAwait(false)) return await operation(disposable).ConfigureAwait(false);
            }));

        /// <summary>
        /// Executes the specified action with the disposable and disposes it afterward.
        /// </summary>
        /// <param name="operation">The action to execute with the disposable.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task AsyncDisposeAfter(Action<D> operation)
        {
            await using (disposable.ConfigureAwait(false)) operation(disposable);
        }

        /// <summary>
        /// Executes the specified asynchronous action with the disposable and disposes it afterward.
        /// </summary>
        /// <param name="operation">The asynchronous action to execute with the disposable.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task AsyncDisposeAfterAsync(Func<D, Task> operation) =>
            run(func(async () =>
            {
                await using (disposable.ConfigureAwait(false)) await operation(disposable).ConfigureAwait(false);
            }));
    }
}
#endif