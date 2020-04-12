using System;
using System.Threading.Tasks;
using static Funk.Prelude;

namespace Funk
{
    public static class DisposableExt
    {
        public static R DisposeAfter<D, R>(this D disposable, Func<D, R> operation) where D : IDisposable
        {
            using (disposable) return operation(disposable);
        }

        public static Task<R> DisposeAfterAsync<D, R>(this D disposable, Func<D, Task<R>> operation) where D : IDisposable
        {
            return run(func(async () =>
            {
                using (disposable)
                {
                    return await operation(disposable);
                }
            }));
        }

        public static void DisposeAfter<D>(this D disposable, Action<D> operation) where D : IDisposable
        {
            using (disposable) operation(disposable);
        }

        public static Task DisposeAfterAsync<D>(this D disposable, Func<D, Task> operation) where D : IDisposable
        {
            return run(act(async () =>
            {
                using (disposable)
                {
                    await operation(disposable);
                }
            }));
        }
    }
}
