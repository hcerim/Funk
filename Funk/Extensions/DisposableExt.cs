using System;
using System.Threading.Tasks;

namespace Funk
{
    public static class DisposableExt
    {
        public static R DisposeAfter<D, R>(this D disposable, Func<D, R> operation) where D : IDisposable
        {
            using (disposable) return operation(disposable);
        }

        public static async Task<R> DisposeAfterAsync<D, R>(this D disposable, Func<D, Task<R>> operation) where D : IDisposable
        {
            using (disposable) return await operation(disposable).ConfigureAwait(false);
        }

        public static void DisposeAfter<D>(this D disposable, Action<D> operation) where D : IDisposable
        {
            using (disposable) operation(disposable);
        }

        public static async Task DisposeAfterAsync<D>(this D disposable, Func<D, Task> operation) where D : IDisposable
        {
            using (disposable) await operation(disposable).ConfigureAwait(false);
        }
    }
}
