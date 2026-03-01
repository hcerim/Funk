using System;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    /// <summary>
    /// Provides extension methods for safely working with IDisposable objects.
    /// </summary>
    public static class DisposableExt
    {
        /// <summary>
        /// Executes the specified function with the disposable and disposes it afterwards, returning the result.
        /// </summary>
        public static R DisposeAfter<D, R>(this D disposable, Func<D, R> operation) where D : IDisposable
        {
            using (disposable) return operation(disposable);
        }

        /// <summary>
        /// Executes the specified asynchronous function with the disposable and disposes it afterwards, returning the result.
        /// </summary>
        public static Task<R> DisposeAfterAsync<D, R>(this D disposable, Func<D, Task<R>> operation) where D : IDisposable
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
        public static void DisposeAfter<D>(this D disposable, Action<D> operation) where D : IDisposable
        {
            using (disposable) operation(disposable);
        }

        /// <summary>
        /// Executes the specified asynchronous action with the disposable and disposes it afterwards.
        /// </summary>
        public static Task DisposeAfterAsync<D>(this D disposable, Func<D, Task> operation) where D : IDisposable
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
